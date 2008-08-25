/*
 * Canon HF10(0) Firmware Decrypter V1.2
 * Copyright 2008 mg1984@gmx.at
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
 * based on "Canon Firmware Decrypter" by Alex Bernstein, 2003
 */
#include <stdio.h>
#include <stddef.h>
#include <getopt.h>

#include "300D_table.h"

#define START_PAD 0x10000
#define P_SIZE 64					// packet size
#define FIRST_P_ROUND 50
#define FIRST_P_OFFSET 0x10

typedef struct FWHeader {
	char 			pad_pre[START_PAD];
	char 			version[4];
	char 			filename_mask[4];
	char 			terminator[2];
	char 			pad_post[6];
} FWHeader;

typedef struct FWSectionHeader {
	char 			pad_pre[12];
	unsigned int 	end_offset;
	char 			pad_post[16];
} FWSectionHeader;

typedef struct FWSection {
	FWSectionHeader header;
	char			*data_pointer;
} FWSection;

typedef struct FWFooter {
	unsigned int 	num_sections;
	struct {
		unsigned int size;
		unsigned int checksum;
	} 				sections[5];
} FWFooter;

typedef struct FW {
	FWHeader 		*header;
	FWFooter		*footer;
	FWSection	 	*sections[];
} FW;

/* http://www.codeguru.com/forum/showthread.php?t=292902 */
unsigned int endian_swap(unsigned int x)
{
    return (x>>24) | 
        ((x<<8) & 0x00FF0000) |
        ((x>>8) & 0x0000FF00) |
        (x<<24);
}

int dec(FILE *in, int i, int j, int length, char *buffer, int offset) {
  int x, val;
  
  //printf("i = %2i j = %2i length = %2i\n", i, j, length);
  for(x = 0; x < length; x++) {
    if((val = fgetc(in)) == EOF)
	  return x;
	
	buffer[offset + x] = val ^ crypt1[i + x] ^ crypt2[j + x];
  }
  
  return x;
}

int main(int argc, char *argv[])
{
  FILE *in, *out;
  int rc = FIRST_P_ROUND, bc;
  long decstart, decsize;
  long filesize, temp;
  char split = 0, skip_ed = 0;
  int c, offset;
  char *buffer;
  
  FWHeader *header;
  FWFooter *footer;
  FWSection *sections[5];
	
  
  while ((c = getopt (argc, argv, "sk")) != -1) {
        switch (c) {
		   case 's':
             split = 1;
             break;
		  case 'k':
             skip_ed = 1;
             break;
        }
  }

  if (argc < 3 || argc > 5) {
    printf("Usage: dhf10 [options] inputfile outputfile\n");
	printf("options:\n");
	printf("    -s    write sections into separate output files\n");
    return -1;
  }

  if ((in = fopen(argv[optind], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[optind]);
    return -1;
  }
  printf("input file: %s\n", argv[optind]);
  
  fseek(in, 0, SEEK_END);
  filesize = ftell(in);
  fseek(in, 0, SEEK_SET);
  buffer = malloc(filesize);
  decstart = sizeof(FWHeader);
  decsize = filesize - sizeof(FWHeader) - sizeof(FWFooter);
  
  printf("filesize:\t\t%ld bytes\n", filesize);
  printf("encoded area:\t\t0x%08x - 0x%08x\n", decstart, decstart + decsize);
  
  if(skip_ed) {
    for(offset = 0; offset < filesize; offset++)
		buffer[offset] = fgetc(in);
  }
  else {
	  // read unencrypted header
	  for(offset = 0; offset < decstart; offset++)
		buffer[offset] = fgetc(in);

	  // decrypt first packet
	  offset += dec(in, 0 + FIRST_P_OFFSET, P_SIZE - rc + FIRST_P_OFFSET, rc + 1 - FIRST_P_OFFSET, buffer, offset);
	  rc++;

	  // decrypt other packets
	  do {
	    bc = dec(in, rc, 0, P_SIZE - rc, buffer, offset);
		bc += dec(in, 0, P_SIZE - rc, rc + 1, buffer, offset + bc);
		rc = (rc + 1) % P_SIZE;
		offset += bc;
		//printf("rc: %5i bc: %5i\n", rc, bc);
	  } while(bc != 0 && offset <= filesize - P_SIZE - sizeof(FWFooter));
	  
	  // decrypt last incomplete packet before footer
	  temp = filesize - offset - sizeof(FWFooter);
	  if(temp >= P_SIZE - rc) {
		bc = dec(in, rc, 0, P_SIZE - rc, buffer, offset);
		temp -= bc;
		bc += dec(in, 0, P_SIZE - rc, temp, buffer, offset + bc);
		offset += bc;
	  }
	  else {
	    bc = dec(in, rc, 0, temp, buffer, offset);
		offset += bc;
	  }
	  
	  // read unencrypted footer
	  for(offset; offset < filesize; offset++)
	    buffer[offset] = fgetc(in);
  }
  fclose(in);
  
  header = (FWHeader *)buffer;
  footer = (FWFooter *)(buffer + decstart + decsize);
  //for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
  //	sections[temp] = (FWSection *)(buffer + endian_swap(footer->sections[temp].offset));
  
  printf("\n ---  HEADER  --- \n");
  printf("version:\t\t%d.%d.%d.%d\n", header->version[0], header->version[1], header->version[2], header->version[3]);
  printf("filemask:\t\t%c%c%c%c\n", header->filename_mask[0], header->filename_mask[1], header->filename_mask[2], header->filename_mask[3]);

  printf("\n ---  FOOTER  --- \n");
  printf("sections:\t\t%d\n", endian_swap(footer->num_sections));
  for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
	printf("section %d size 0x%08x checksum 0x%08x\n", 
		temp, endian_swap(footer->sections[temp].size), endian_swap(footer->sections[temp].checksum));
	
  /*printf("\n --- SECTIONS --- \n");
  for(temp = 0; temp < endian_swap(footer->num_sections); temp++)
    printf("section %d end_offset 0x%08x\n", temp, endian_swap(sections[temp]->header.end_offset));*/
  printf("\n");
  
  if(split) {
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
  return 0;
}
