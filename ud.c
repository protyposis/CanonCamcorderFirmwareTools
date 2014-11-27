/*
 * Universal Canon Digic DV Firmware Decrypter
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
 * based on "Canon Firmware Decrypter" by Alex Bernstein, 2003
 */
#include <stdio.h>
#include <stddef.h>
#include <getopt.h>
#include <string.h>

#define VERSION "1.3c"

#include "300D_table.h"

#define LITTLE_ENDIAN_SYS			// is the compile target little endian?

#define START_PAD 0x10000
#define P_SIZE 64					// packet size
#define FIRST_P_ROUND 50
#define FIRST_P_OFFSET_HF10 0x10
#define FIRST_P_OFFSET_HV30 0x08


typedef struct ModelDef {
  char			*name;
  char			*filemask;
  char			*identifier;
  int			first_p_round;
  int			first_p_offset;
} Model;

/* if the filemask shouldn't be enough to identify further canon fw files, we should additionally
 * decode the payload for all models and compare the identifier string as well */
struct ModelDef Models[] = {
  {"HF10/100",  "VEEX", "D128  CanonDV", FIRST_P_ROUND, FIRST_P_OFFSET_HF10},
  {"HF11",      "VELX", "D128S CanonDV", FIRST_P_ROUND, FIRST_P_OFFSET_HF10},
  {"HV30",      "VEFX", "D120B CanonDV", FIRST_P_ROUND, FIRST_P_OFFSET_HV30}
};

typedef struct FWHeader {
  char 			pad_pre[START_PAD];
  char 			version[4];
  char 			filename_mask[4];
  char 			terminator[2];
  char 			spaces[6];
/*  char			identifier[14];
  char			terminator2[2];*/
} FWHeader;

typedef struct FWSectionHeader {
  char 			pad_pre[12];
  unsigned int 	end_offset;
  char 			pad_post[16];
} FWSectionHeader;

typedef struct FWSection {
  FWSectionHeader header;
  char			*data;
} FWSection;

typedef struct FWFooter {
  unsigned int 	num_sections;
  struct {
  	unsigned int size;
  	unsigned int checksum;
  } 			sections[5];
} FWFooter;

typedef struct FW {
  FWHeader 		*header;
  FWFooter		*footer;
  FWSection	 	*sections[];
} FW;

/* http://www.codeguru.com/forum/showthread.php?t=292902 */
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

int dec(int i, int j, int length, char *buffer, int offset) {
  int x;
  
  for(x = 0; x < length; x++) {
	buffer[offset + x] = buffer[offset + x] ^ crypt1[i + x] ^ crypt2[j + x];
  }
  
  return x;
}

struct KeygenState {
  int	length;
  char	*key;
};
struct KeygenState keystate;
void keygen_init() {
  int x, c = 0, i = 0, j = 0, rc = 0;
  
  keystate.length = P_SIZE * (P_SIZE + 1);
  keystate.key = (char *)malloc(keystate.length);

  for(rc = 0; rc < P_SIZE; rc++) {
    for(x = 0; x < P_SIZE - i; x++) {
      keystate.key[c] = crypt1[i + x] ^ crypt2[x];
	  c++;
	}
	for(x = 0; x < j + 1; x++) {
	  keystate.key[c] = crypt1[x] ^ crypt2[P_SIZE - j + x];
	  c++;
	}
	
	i++ % P_SIZE;
	j++ % P_SIZE;
  }
}

void keygen_crypt(char* buffer_src, char* buffer_dst, int offset, int length, int key_offset) {
  int x;
  
  buffer_src = (char*)(buffer_src + offset);
  buffer_dst = (char*)(buffer_dst + offset);
  for(x = 0; x < length; x++)
	buffer_dst[x] = buffer_src[x] ^ keystate.key[(key_offset + x) % keystate.length];
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
  long decstart, decsize;
  long filesize, temp, temp2;
  char o_split = 0, o_skip_ed = 0, o_calc_checksum = 0;
  int c;
  char *buffer;
  
  FWHeader *header;
  FWFooter *footer;
  //FWSection *sections[5];
  Model *model;
  
  keygen_init();
	
  printf("Universal Canon Digic DV Firmware Decrypter V%s\n\n", VERSION);
  
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
	printf("    -s    write sections into separate output files\n");
	printf("    -k    skip en-/decryption\n");
	printf("    -c    calculate and insert checksum (pointless for decryption!)\n");
    return -1;
  }

  if ((in = fopen(argv[optind], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[optind]);
    return -1;
  }
  printf("input file:\t\t%s\n", argv[optind]);
  
  fseek(in, 0, SEEK_END);
  filesize = ftell(in);
  fseek(in, 0, SEEK_SET);
  buffer = malloc(filesize);
  decstart = sizeof(FWHeader);
  decsize = filesize - sizeof(FWHeader) - sizeof(FWFooter);
  
  printf("filesize:\t\t%ld bytes\n", filesize);
  printf("encoded area:\t\t0x%08x - 0x%08x\n", decstart, decstart + decsize);
  
  fread(buffer, sizeof(char), filesize, in);
  fclose(in);
  
  header = (FWHeader *) buffer;
  footer = (FWFooter *)(buffer + decstart + decsize);
  
  printf("\n ---  HEADER  --- \n");
  printf("version:\t\t%d.%d.%d.%d\n", header->version[0], header->version[1], header->version[2], header->version[3]);
  printf("filemask:\t\t%c%c%c%c\n", header->filename_mask[0], header->filename_mask[1], header->filename_mask[2], header->filename_mask[3]);

  printf("\n ---  FOOTER  --- \n");
  printf("sections:\t\t%d\n", endian_swap(footer->num_sections));
  for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
	printf("section %d size 0x%08x checksum 0x%08x\n", 
		temp, endian_swap(footer->sections[temp].size), endian_swap(footer->sections[temp].checksum));
  printf("\n");
  
  printf("trying to identify camera model... (%d known models)\n", sizeof(Models) / sizeof(Model));
  if((model = identify_model(header->filename_mask)) == NULL) {
	printf("cannot identify camera model\n");
	return -2;
  }
  printf("camera model:\t\t%s\n\n", model->name);
  
  if(!o_skip_ed) {
	  keygen_crypt(buffer, buffer, decstart, decsize, P_SIZE * (model->first_p_round + 1) + model->first_p_offset);
  }
  
  //for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
  //	sections[temp] = (FWSection *)(buffer + endian_swap(footer->sections[temp].offset));
	
  /*printf("\n --- SECTIONS --- \n");
  for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
    printf("section %d end_offset 0x%08x\n", temp, endian_swap(sections[temp]->header.end_offset));
  printf("\n");*/
  
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
	    printf("Cant't open file name %s\n", filename);
	    return -1;
	  }
	  if(temp == 0)
		fwrite(buffer, 1, endian_swap(footer->sections[temp].size), out);
	  else
	    fwrite((char*)(buffer + endian_swap(footer->sections[temp - 1].size)), 1, endian_swap(footer->sections[temp].size), out);
	  fclose(out);	  
    }
	
	// write footer
	sprintf(filename, "%s.%03d", argv[optind + 1], temp);
	if ((out = fopen(filename, "wb")) == NULL) {
	  printf("Cant't open file name %s\n", filename);
	  return -1;
	}
	fwrite((char *)footer, 1, sizeof(FWFooter), out);
	fclose(out);	 
  }
  else {
    if ((out = fopen(argv[optind + 1], "wb")) == NULL) {
	  printf("Cant't open file name %s\n", argv[optind + 1]);
	  return -1;
	}
	fwrite(buffer, 1, filesize, out);
	fclose(out);
  }

  free(buffer);
  free(keystate.key);
  printf("finished\n");
  return 0;
}
