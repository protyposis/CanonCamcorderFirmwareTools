/*
 * Universal Canon Camcorder Firmware Decrypter
 * Copyright 2008, 2014 Mario Guggenberger <mg@protyposis.net>
 *
 * V1.1
 * - option to omit unecrypted file header
 *
 * V1.2
 * - option to display file info
 * - option to split output file into its sections
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
 * - new de-/encryption method which automatically extracts the key for unknown models
 * - support for HG20 / HG21
 * - support for XF 300 / XF 305
 * - support for HF S21
 * - support for XF 100 / XF 105
 * - support for XA10
 * - support for HF R30/R32/R36/R37/R38
 * - support for HF M50/M52/M56
 * - support for HF R40/R42/R46/R47/R48
 * - support for XA 35 / XA 20 / HF G30
 * - support for mini
 */
#include <stdio.h>
#include <stddef.h>
#include <getopt.h>
#include <string.h>

#define VERSION "2.0"

#define LITTLE_ENDIAN_SYS           // is the compile target little endian?

#define P_SIZE 64                   // packet size

/* 
 * These are the first 64/65 bytes for the 300D keys from which the camcorder key is computed.
 * Taken from the "Canon Firmware Decrypter" by Alex Bernstein, 2003
 * http://alexbernstein.com/wiki/canon-firmware-decrypter/
 */
char crypt1[] = {
  0x41, 0xCA, 0x42, 0xF9, 0x3C, 0x6D, 0x9D, 0xDF, 0xD8, 0x19, 0x84, 0x08, 0x50, 0xFB, 0x67, 0xA1,
  0xAD, 0x35, 0xF2, 0xCE, 0xDF, 0x9A, 0xF3, 0xC4, 0xF5, 0x22, 0x1A, 0x8A, 0xD5, 0x64, 0x08, 0x65,
  0xD3, 0x2C, 0x96, 0xB6, 0x08, 0x11, 0x3A, 0xB3, 0x90, 0x08, 0xE6, 0x2E, 0x88, 0xED, 0x24, 0xFB,
  0x24, 0x28, 0x82, 0x3D, 0x39, 0xA8, 0xF3, 0x52, 0x98, 0xF9, 0xA7, 0x75, 0x52, 0x63, 0x0A, 0xF9
};
char crypt2[] = {
  0xA5, 0xF5, 0x2A, 0xBE, 0x62, 0x66, 0x1C, 0x80, 0x87, 0x3F, 0x91, 0x01, 0x6E, 0xBC, 0x87, 0x02,
  0xD0, 0x2A, 0x3A, 0xC4, 0x65, 0x3C, 0x91, 0x35, 0xB5, 0xAE, 0x21, 0x21, 0xF5, 0x3F, 0xAC, 0x1F,
  0xBE, 0x92, 0xA6, 0xF2, 0xA8, 0x42, 0x39, 0x8F, 0x41, 0x95, 0x91, 0xBD, 0x4C, 0xAD, 0x85, 0xC7,
  0xB7, 0xD6, 0xAE, 0x1C, 0xE2, 0x62, 0xEB, 0xC2, 0x03, 0x5D, 0xD6, 0x4E, 0x7B, 0x59, 0x0F, 0xDD,
  0xB7
};

typedef struct ModelDef {
  char      *filemask;
  int       keyoffset;
  char      *name;
} Model;

Model Models[] = {
  /* The offset of the key to decrypt the file is derived from the internal 
   * camera ID number, but the camera ID is only visible after decrypting */
  // keyoffset = cameraID#
  {"VEEX", 128, "HF10 / HF100"},            // D128  CanonDV (@ 0x10004)
  {"VEFX", 120, "HV30"},                    // D120B CanonDV (@ 0x10004)
  {"VEGX", 130, "HG20 / HG21"},             // D130  CanonDV (@ 0x10004)
  {"VELX", 128, "HF11"},                    // D128S CanonDV (@ 0x10004)
  // new file structure
  // keyoffset = cameraID# - 8 [unencrypted header size?]
  {"VGAX", 133, "XF 300 / XF 305"},         // D141 CanonDV (@ 0x100C)
  {"VGDX", 136, "HF S21"},                  // D144 CanonDV (@ 0x1F7DF0)
  {"VHAX", 137, "XF 100 / XF 105"},         // D145 CanonDV (@ 0x3A4830)
  {"VHEX", 142, "XA10"},                    // D150 CanonDV (@ 0x4E54A0)
  {"VIBX", 153, "HF R30/R32/R36/R37/R38"},  // D161  CanonDV (@ 0x100C) (FW 1.1.1.0 header says VIBX, decoded data says VGAX)
  {"VIDX", 154, "HF M50/M52/M56"},          // D162  CanonDV (@ 0x100C) (FW 1.1.1.0 header says VIDX, decoded data says VGAX)
  // new file structure
  // keyoffset = cameraID# - 12 [unencrypted header size?]
  {"VJCX", 153, "HF R40/R42/R46/R47/R48"},  // 165 (@ 0x200C) 153
  {"VJEX", 154, "XA 35 / XA 20 / HF G30"},  // 166 (@ 0x200C)
  {"VJFX", 156, "mini"},                    // 168 (@ 0x200C)
};

typedef struct FWHeader {
  char            pad_pre[0x10000];
  char            version[4];
  char            filename_mask[4];
  char            terminator[2];
  char            spaces[6];
} FWHeader;

typedef struct FWSectionHeader {
  char            pad_pre[12];
  unsigned int    end_offset;
  char            pad_post[16];
} FWSectionHeader;

typedef struct FWFooter {
  unsigned int    num_sections;
  struct {
    unsigned int    size;
    unsigned int    checksum;
  }               sections[5];
} FWFooter;

typedef struct Key {
  int   length;
  char  *key;
} Key;

typedef struct FWHeader2 {
  char            filename_mask[4];
  char            version[4];
} FWHeader2;

typedef struct FWHeader3 {
  char            filename_mask[4];
  char            version[4];
  char            zeropad[4];
} FWHeader3;

Key key;


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

// Computes the camcorder key from the 300D keys
void key_init() {
  int x, c = 0, i = 0, j = 0, rc = 0;
  
  key.length = P_SIZE * (P_SIZE + 1);
  key.key = (char *)malloc(key.length);

  for(rc = 0; rc < P_SIZE; rc++) {
    for(x = 0; x < P_SIZE - i; x++) {
      key.key[c] = crypt1[i + x] ^ crypt2[x];
      c++;
    }
    for(x = 0; x < j + 1; x++) {
      key.key[c] = crypt1[x] ^ crypt2[P_SIZE - j + x];
      c++;
    }
  
    i++ % P_SIZE;
    j++ % P_SIZE;
  }
}

// Searches for a possible key and extracts it into *key
int extract_key(char* data, int offset, int length, Key* key) {
  int i;
  char temp;
  int key_candidate_block_pos;
  int key_candidate_block_length = 0;
  int key_found = 0;

  // create an XORed buffer with the key length
  // and search for a zeroed block of at least the key length
  for(i = offset; i < length; i++) {
    temp = data[i] ^ data[(i + key->length) % length];

    if(temp == 0x00) {
      if(key_candidate_block_length == 0) {
        key_candidate_block_pos = i;
      }
      key_candidate_block_length++;
    } else {
      key_candidate_block_length = 0;
    }

    if(key_candidate_block_length >= key->length) {
      key_found = 1;
      //printf("key found at 0x%08X\n", key_candidate_block_pos);
      break;
    }
  }

  if(key_found == 1) {
    int key_offset = key_candidate_block_pos % key->length;

    for(i = 0; i < key->length; i++) {
      key->key[(i + key_offset) % key->length] = data[key_candidate_block_pos + i];
    }

    return key_candidate_block_pos;
  }

  return -1;
}

// Compares two keys and returns the offset from the first to the second 
// if they are equal, else returns -1 if they do not match.
int key_compare(Key* key1, Key* key2) {
  int x, y;
  if(key1->length != key2->length) {
    printf("error: key lengths do not match\n");
    return -1;
  }

  for(x = 0; x < key1->length; x++) {
    for(y = 0; y < key1->length; y++) {
      if(key1->key[(x + y) % key2->length] != key2->key[y]) {
        break;
      }
    }
    if(y == key1->length) {
      // the key matches over its whole length with offset "x"
      return x;
    }
  }

  return -1;
}

// Searches for a valid key and outputs the offset so it can be added to the list of known models
int key_find(char* buffer, int length, Key* key_reference) {
  Key key2;
  int search_offset = 0;
  int key_pos = 0;
  int key_offset = 0;
  int x;

  key2.length = key_reference->length;
  key2.key = (char *)malloc(key2.length);

  while((key_pos = extract_key(buffer, search_offset, length, &key2)) >= 0) {
    printf("checking potential key at 0x%08X... ", key_pos);

    if((key_offset = key_compare(key_reference, &key2)) >= 0) {
      printf("yap! (offset %d)\n", key_offset);
      key = key2; // set discovered key as global key
      return 1;
    }

    // invert the key and try again (key could be found in an 0x00 or 0xFF section of the decrypted file)
    for(x = 0; x < key2.length; x++) {
      key2.key[x] = ~key2.key[x];
    }

    if((key_offset = key_compare(key_reference, &key2)) >= 0) {
      printf("yap! (inverted, offset %d)\n", key_offset);
      key = key2; // set discovered key as global key
      return 1;
    }

    printf("nope\n");

    search_offset = key_pos + key_reference->length;
  }

  return 0;
}

// Encrypts/decrypts the buffer
void crypt(char* buffer_src, char* buffer_dst, int offset, int length, int key_offset) {
  int x;
  
  for(x = offset; x < offset + length; x++) {
    buffer_dst[x] = buffer_src[x] ^ key.key[(key_offset + x) % key.length];
  }
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

int main(int argc, char *argv[]) {
  FILE *in, *out;
  char keyfilename[255];
  int decstart, decsize;
  int filesize, temp, temp2;
  char o_split = 0, o_skip_ed = 0, o_calc_checksum = 0, o_keyfilemode = 0;
  int c;
  char *buffer;
  int file_format = 0;
  FWHeader *header;
  FWHeader2 *header2;
  FWHeader3 *header3;
  FWFooter *footer;
  Model *model;
  
  printf("Universal Canon Camcorder Firmware Decrypter V%s\n", VERSION);
  printf("http://github.com/protyposis/CanonCamcorderFirmwareTools\n\n");
  
  while ((c = getopt (argc, argv, "skcf")) != -1) {
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
      case 'f':
        o_keyfilemode = 1;
        break;
    }
  }

  if (argc < 3 || argc > 6) {
    printf("Usage: ud [options] inputfile outputfile\n");
    printf("options:\n");
    printf("  -s    split sections into separate output files\n");
    printf("  -k    skip en-/decryption\n");
    printf("  -c    calculate and update checksum\n");
    printf("  -f    keyfile mode (extract key to file or read key from file, even if model is known)\n");
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
  header3 = (FWHeader3 *) buffer;
  if(header3->filename_mask[0] == 'V' && header3->filename_mask[1] >= 'J') {
    printf("file format:\t\t3\n");
    printf("\n ---  HEADER  --- \n");
    printf("version:\t\t%d.%d.%d.%d\n", header3->version[0], header3->version[1], header3->version[2], header3->version[3]);
    printf("filemask:\t\t%c%c%c%c\n", header3->filename_mask[0], header3->filename_mask[1], header3->filename_mask[2], header3->filename_mask[3]);

    decstart = sizeof(FWHeader3);
    decsize = filesize - sizeof(FWHeader3);

    sprintf(keyfilename, "%c%c%c%c%02x%02x%02x%02x.key", 
      header3->filename_mask[0], header3->filename_mask[1], header3->filename_mask[2], header3->filename_mask[3], 
      header3->version[0], header3->version[1], header3->version[2], header3->version[3]);

    file_format = 3;
  }
  else if(header2->filename_mask[0] == 'V') {
    printf("file format:\t\t2\n");
    printf("\n ---  HEADER  --- \n");
    printf("version:\t\t%d.%d.%d.%d\n", header2->version[0], header2->version[1], header2->version[2], header2->version[3]);
    printf("filemask:\t\t%c%c%c%c\n", header2->filename_mask[0], header2->filename_mask[1], header2->filename_mask[2], header2->filename_mask[3]);

    decstart = sizeof(FWHeader2);
    decsize = filesize - sizeof(FWHeader2);

    sprintf(keyfilename, "%c%c%c%c%02x%02x%02x%02x.key", 
      header2->filename_mask[0], header2->filename_mask[1], header2->filename_mask[2], header2->filename_mask[3], 
      header2->version[0], header2->version[1], header2->version[2], header2->version[3]);

    file_format = 2;
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

    file_format = 1;
  }
  else {
    printf("unknown file format\n");
    return -1;
  }

  if(file_format == 1) {
    footer = (FWFooter *)(buffer + decstart + decsize);

    printf("\n ---  FOOTER  --- \n");
    printf("sections:\t\t%d\n", endian_swap(footer->num_sections));
  
    for(temp = 0; temp < endian_swap(footer->num_sections); temp++) {
      printf("section %d size 0x%08X checksum 0x%08X\n", temp, endian_swap(footer->sections[temp].size), endian_swap(footer->sections[temp].checksum));
    }
  
    printf("\n");
  }

  printf("encoded area:\t\t0x%08X - 0x%08X\n", decstart, decstart + decsize);
  printf("\n");


  // identify camera model
  printf("trying to identify camera model (%d known models)...\n", sizeof(Models) / sizeof(Model));
  if((model = identify_model(header->filename_mask)) == NULL && (model = identify_model(header2->filename_mask)) == NULL) {
    printf("cannot identify camera model\n");
  } else {
  	printf("camera model:\t\t%s\n", model->name);
  }
  printf("\n");

  key_init();


  // read the key from the keyfile if existing, else try to extract key and write to keyfile
  if(model == NULL || o_keyfilemode == 1) {
    if((key.key = file_read(keyfilename, &key.length)) == NULL) {
      printf("keyfile %s not found, trying to extract key...\n", keyfilename);
      key_init(); // init again because they key has just been NULLed
    
      if(key_find(buffer, filesize, &key) == 0) {
        printf("cannot extract the key\n");
        return -1;
      }
      else {
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
    printf("\n");
  }

  // encrypt / decrypt data
  if(!o_skip_ed) {
    crypt(buffer, buffer, decstart, decsize, (model == NULL || o_keyfilemode == 1) ? 0 : model->keyoffset);
  }
  
  // calculate checksum
  if(file_format == 1 && o_calc_checksum) {
    temp2 = 0;
  
    printf("calculating checksums...\n");
    for(temp = 0; temp < endian_swap(footer->num_sections); temp++) {
      footer->sections[temp].checksum = endian_swap(checksum((unsigned int*)buffer, temp2 / sizeof(unsigned int), 
        endian_swap(footer->sections[temp].size) / sizeof(unsigned int)));
      printf("new checksum for section %d is: 0x%08X\n", temp, endian_swap(footer->sections[temp].checksum));
      temp2 += endian_swap(footer->sections[temp].size);
    }
    printf("\n");
  }
  
  // split sections into separate files
  if(file_format == 1 && o_split) {
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
