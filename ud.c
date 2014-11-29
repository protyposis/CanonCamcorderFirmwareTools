/*
 * Universal Canon Camcorder Firmware Decrypter
 * Copyright 2008, 2014 Mario Guggenberger <mg@protyposis.net>
 *
 * V1.1
 * - option to omit unecrypted file header
 *
 * V1.2
 * - option to display file info
 * - option to split output file into it's sections
 * - removed omit option
 * - option to skip enc/decryption
 *
 * V1.3
 * - option to calculate checksum
 * - supports HF10/100 and HV30
 *
 * V1.3b
 * - crypting mechanism simplified
 *
 * V1.3c
 * - support for HF11 added
 *
 * V2.0
 * - new de-/encryption method which extracts the key by itself
 *   this means the 300D keys are no longer necessary, also newer 
 *   firmware updates don't seem to use the 300D keys (or the 
 *   derivation function is different)
 * - supports HF S21
 * - supports XA 35/XA 20/HF G30
 * - supports mini
 */
#include <stdio.h>
#include <stddef.h>
#include <getopt.h>
#include <string.h>

#define VERSION "2.0"

#define LITTLE_ENDIAN_SYS           // is the compile target little endian?

#define START_PAD 0x10000


typedef struct ModelDef {
  char      *filemask;
  char      *name;
  int       invert_key;
  char      *identifier;
} Model;

struct ModelDef Models[] = {
  {"VEEX", "HF10/100",            0, "D128  CanonDV"},
  {"VEFX", "HV30",                0, "D120B CanonDV"},
  {"VELX", "HF11",                0, "D128S CanonDV"},
  {"VGDX", "HF S21",              0, NULL},
  {"VJEX", "XA 35/XA 20/HF G30",  1, NULL}, // invert key
  {"VJFX", "mini",                0, NULL},
};

typedef struct FWHeader {
  char            pad_pre[START_PAD];
  char            version[4];
  char            filename_mask[4];
  char            terminator[2];
  char            spaces[6];
/*  char      identifier[14];
  char      terminator2[2];*/
} FWHeader;

typedef struct FWSectionHeader {
  char            pad_pre[12];
  unsigned int    end_offset;
  char            pad_post[16];
} FWSectionHeader;

typedef struct FWSection {
  FWSectionHeader header;
  char            *data;
} FWSection;

typedef struct FWFooter {
  unsigned int    num_sections;
  struct {
    unsigned int    size;
    unsigned int    checksum;
  }               sections[5];
} FWFooter;

typedef struct FW {
  FWHeader        *header;
  FWFooter        *footer;
  FWSection       *sections[];
} FW;

typedef struct Key {
  int   length;
  char  *key;
} Key;
Key key;

typedef struct FWHeader2 {
  char            filename_mask[4];
  char            version[4];
} FWHeader2;


unsigned int endian_swap(unsigned int x) {
#ifdef LITTLE_ENDIAN_SYS
  return (x>>24) | 
    ((x<<8) & 0x00FF0000) |
    ((x>>8) & 0x0000FF00) |
    (x<<24);
#else
  return x;
#endif
}

int extract_key(char* data, int offset, int length, int keylength) {
  int i;
  char temp;
  int key_candidate_block_pos;
  int key_candidate_block_length = 0;
  int key_found = 0;

  // create an XORed buffer with the key length
  // and search for a zeroed block of at least the key length
  for(i = offset; i < length; i++) {
    temp = data[i] ^ data[(i + keylength) % length];

    if(temp == 0x00) {
      if(key_candidate_block_length == 0) {
        key_candidate_block_pos = i;
      }
      key_candidate_block_length++;
    } else {
      key_candidate_block_length = 0;
    }

    if(key_candidate_block_length >= keylength) {
      key_found = 1;
      printf("key found at 0x%08X\n", key_candidate_block_pos);
      break;
    }
  }

  if(key_found == 1) {
    int key_offset = key_candidate_block_pos % keylength;

    key.length = keylength;
    key.key = (char *)malloc(keylength);

    for(i = 0; i < keylength; i++) {
      key.key[(i + key_offset) % key.length] = ~data[key_candidate_block_pos + i];
    }
  }

  return key_found;
}

void crypt(char* buffer_src, char* buffer_dst, int offset, int length) {
  int x;
  int key_offset = offset % key.length;
  
  buffer_src = (char*)(buffer_src + offset);
  buffer_dst = (char*)(buffer_dst + offset);
  
  for(x = 0; x < length; x++)
    buffer_dst[x] = buffer_src[x] ^ key.key[(key_offset + x) % key.length];
}

int file_write(char *filename, char *buffer, int length) {
  FILE *out;

  if((out = fopen(filename, "wb")) == NULL) {
    return 0;
  }
  fwrite(buffer, 1, length, out);
  fclose(out);

  return 1;
}

char* file_read(char *filename, int *filesize) {
  FILE *in;
  char* buffer;

  if ((in = fopen(filename, "rb")) == NULL) {
    return NULL;
  }
  
  // determine filesize
  fseek(in, 0, SEEK_END);
  *filesize = (int)ftell(in);
  fseek(in, 0, SEEK_SET);
  
  // read file to buffer
  buffer = malloc(*filesize);
  fread(buffer, sizeof(char), *filesize, in);
  fclose(in);

  return buffer;
}

unsigned int checksum(unsigned int *buffer, unsigned int offset, unsigned int length) {
  unsigned int x, result = 0;

  buffer = (unsigned int*)(buffer + offset);
  for (x = 0; x < length; x++) {
    result += endian_swap(buffer[x]);
  }

  return result;
}

Model* identify_model(char *data) {
  int x;
  
  for(x = 0; x < sizeof(Models) / sizeof(Model); x++) {
    if(memcmp(Models[x].filemask, data, 4) == 0)
      return (Model*)&Models[x];
  }
  
  return NULL;
}

int main(int argc, char *argv[])
{
  FILE *in, *out;
  char keyfilename[255];
  int decstart, decsize;
  int filesize, temp, temp2;
  char o_split = 0, o_skip_ed = 0, o_calc_checksum = 0;
  int c;
  char *buffer;
  
  FWHeader *header;
  FWHeader2 *header2;
  FWFooter *footer;
  //FWSection *sections[5];
  Model *model;
  
  printf("Universal Canon Camcorder Firmware Decrypter V%s\n\n", VERSION);
  
  while ((c = getopt (argc, argv, "skc")) != -1) {
    switch (c) {
      case 's':
        o_split = 1;
        break;
      case 'k':
        o_skip_ed = 1;
        break;
      case 'c':
        o_calc_checksum = 1;
        break;
    }
  }

  if (argc < 3 || argc > 6) {
    printf("Usage: ud [options] inputfile outputfile\n");
    printf("options:\n");
    printf("  -s    split sections into separate output files\n");
    printf("  -k    skip en-/decryption\n");
    printf("  -c    calculate and update checksum\n");
    return -1;
  }


  // read input file
  if((buffer = file_read(argv[optind], &filesize)) == NULL) {
    printf("can't open file %s\n", argv[optind]);
    return -1;
  }
  printf("input file:\t\t%s\n", argv[optind]);
  printf("filesize:\t\t%ld bytes\n", filesize);


  // check header / figure out file format
  header = (FWHeader *) buffer;
  header2 = (FWHeader2 *) buffer;
  if(header2->filename_mask[0] == 'V') {
    printf("file format:\t\t2\n");
    printf("\n ---  HEADER  --- \n");
    printf("version:\t\t%d.%d.%d.%d\n", header2->version[0], header2->version[1], header2->version[2], header2->version[3]);
    printf("filemask:\t\t%c%c%c%c\n", header2->filename_mask[0], header2->filename_mask[1], header2->filename_mask[2], header2->filename_mask[3]);

    decstart = sizeof(FWHeader2);
    decsize = filesize - sizeof(FWHeader2);

    sprintf(keyfilename, "%c%c%c%c%02x%02x%02x%02x.key", 
      header2->filename_mask[0], header2->filename_mask[1], header2->filename_mask[2], header2->filename_mask[3], 
      header2->version[0], header2->version[1], header2->version[2], header2->version[3]);
  }
  else if(header->filename_mask[0] == 'V') {
    printf("file format:\t\t1\n");
    printf("\n ---  HEADER  --- \n");
    printf("version:\t\t%d.%d.%d.%d\n", header->version[0], header->version[1], header->version[2], header->version[3]);
    printf("filemask:\t\t%c%c%c%c\n", header->filename_mask[0], header->filename_mask[1], header->filename_mask[2], header->filename_mask[3]);

    decstart = sizeof(FWHeader);
    decsize = filesize - sizeof(FWHeader) - sizeof(FWFooter);

    sprintf(keyfilename, "%c%c%c%c%02x%02x%02x%02x.key", 
      header->filename_mask[0], header->filename_mask[1], header->filename_mask[2], header->filename_mask[3], 
      header->version[0], header->version[1], header->version[2], header->version[3]);
  }
  else {
    printf("unknown file format\n");
    return -1;
  }


  footer = (FWFooter *)(buffer + decstart + decsize);

  printf("\n ---  FOOTER  --- \n");
  printf("sections:\t\t%d\n", endian_swap(footer->num_sections));
  
  for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
    printf("section %d size 0x%08x checksum 0x%08x\n", temp, endian_swap(footer->sections[temp].size), endian_swap(footer->sections[temp].checksum));
  
  printf("\n");


  printf("encoded area:\t\t0x%08x - 0x%08x\n", decstart, decstart + decsize);
  printf("\n");


  // identify camera model
  printf("trying to identify camera model... (%d known models)\n", sizeof(Models) / sizeof(Model));
  if((model = identify_model(header->filename_mask)) == NULL && (model = identify_model(header2->filename_mask)) == NULL) {
    printf("cannot identify camera model\n");
  }
  printf("camera model:\t\t%s\n\n", model->name);


  // read the key from the keyfile if existing, else try to extract key and write to keyfile
  if((key.key = file_read(keyfilename, &key.length)) == NULL) {
    printf("keyfile %s not found, trying to extract key...\n", keyfilename);
    
    // all firmware updates seem to use a key length of 4160
    if(extract_key(buffer, decstart, filesize, 4160) == 0) {
      printf("cannot extract the key\n");
      return -1;
    }
    else {
      if(model != NULL && model->invert_key == 1) {
        for(temp = 0; temp < key.length; temp++) {
          key.key[temp] = ~key.key[temp];
        }
      }

      printf("key extraction successful\n");
      if(file_write(keyfilename, key.key, key.length) == 1) {
        printf("key written to %s\n", keyfilename);
      } else {
        printf("cannot write keyfile %s\n", keyfilename);
      }
    }
  } 
  else {
    printf("key loaded from %s\n", keyfilename);
  }

  
  if(!o_skip_ed) {
    crypt(buffer, buffer, decstart, decsize);
  }
  
  if(o_calc_checksum) {
    temp2 = 0;
  
    printf("calculating checksums...\n");
    for(temp = 0; temp < endian_swap(footer->num_sections); temp++) {
      footer->sections[temp].checksum = endian_swap(checksum((unsigned int*)buffer, temp2 / sizeof(unsigned int), 
        endian_swap(footer->sections[temp].size) / sizeof(unsigned int)));
      printf("new checksum for section %d is: 0x%08x\n", temp, endian_swap(footer->sections[temp].checksum));
      temp2 += endian_swap(footer->sections[temp].size);
    }
    printf("\n");
  }
  
  if(o_split) {
    char filename[sizeof(argv[optind + 1]) + 10];
  
    // write sections
    for(temp = 0; temp < endian_swap(footer->num_sections); temp++) {
      sprintf(filename, "%s.%03d", argv[optind + 1], temp);

      if ((out = fopen(filename, "wb")) == NULL) {
        printf("can't open file name %s\n", filename);
        return -1;
      }

      if(temp == 0)
        fwrite(buffer, 1, endian_swap(footer->sections[temp].size), out);
      else
        fwrite((char*)(buffer + endian_swap(footer->sections[temp - 1].size)), 1, 
          endian_swap(footer->sections[temp].size), out);
      
      fclose(out);    
    }
  
    // write footer
    sprintf(filename, "%s.%03d", argv[optind + 1], temp);
    
    if ((out = fopen(filename, "wb")) == NULL) {
      printf("can't open file name %s\n", filename);
      return -1;
    }

    fwrite((char *)footer, 1, sizeof(FWFooter), out);
    fclose(out);  
  }
  else {
    if(file_write(argv[optind + 1], buffer, filesize) == 0) {
      printf("can't open file name %s\n", argv[optind + 1]);
      return -1;
    }
  }

  free(buffer);
  free(key.key);

  printf("finished\n");
  return 0;
}
