/*
 * Canon HV30 Firmware Decrypter 1.0 (PAL 1.0.1.0 FW)
 * Copyright 2008 mg1984@gmx.at
 * 
 * This is a modified version of my HF10 V1.1 decrypter,
 * with adjusted values: ENCRYPTION_START, FIRST_P_OFFSET, decstart
 */
#include <stdio.h>
#include <stddef.h>
#include <getopt.h>

#include "300D_table.h"

#define ENCRYPTION_START 0x10010
#define P_SIZE 64					// packet size
#define FIRST_P_ROUND 50
#define FIRST_P_OFFSET 8

int dec(FILE *in, FILE *out, int i, int j, int length) {
  int x, val;
  
  //printf("i = %2i j = %2i length = %2i\n", i, j, length);
  for(x = 0; x < length; x++) {
    if((val = fgetc(in)) == EOF)
	  return x;
	
	fputc(val ^ crypt1[i + x] ^ crypt2[j + x], out);
  }
  
  return x;
}

int main(int argc, char *argv[])
{
  FILE *in, *out;
  int rc = FIRST_P_ROUND, bc;
  int decstart = ENCRYPTION_START;
  int omithead = 0, c;
  
  while ((c = getopt (argc, argv, "o")) != -1) {
        switch (c) {
           case 'o':
             omithead = 1;
             break;
        }
  }

  if (argc < 3 || argc > 4) {
    printf("Usage: dhv30 [-o] inputfile outfile\n");
	printf("options:\n");
	printf("    -o    omit unencrypted file head\n");
    return -1;
  }

  if ((in = fopen(argv[optind], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[optind]);
    return -1;
  }

  if ((out = fopen(argv[optind + 1], "wb")) == NULL) {
    printf("Cant't open file name %s\n", argv[optind + 1]);
    fclose(in);
    return -1;
  }
  
  // copy or omit unencrypted head
  if(omithead == 1)
    fseek(in, decstart, SEEK_SET);
  else {
    while(decstart-- > 0)
      fputc(fgetc(in), out);
  }

  // decrypt first packet
  printf("i1=%i, i2=%i, length=%i\n", 0 + FIRST_P_OFFSET, P_SIZE - rc + FIRST_P_OFFSET, rc + 1 - FIRST_P_OFFSET);
  dec(in, out, 0 + FIRST_P_OFFSET, P_SIZE - rc + FIRST_P_OFFSET, rc + 1 - FIRST_P_OFFSET);
  rc++;
  
  // decrypt other packets
  do {
    bc = dec(in, out, rc, 0, P_SIZE - rc);
	bc += dec(in, out, 0, P_SIZE - rc, rc + 1);
	rc = (rc + 1) % P_SIZE;
	//printf("rc: %5i bc: %5i\n", rc, bc);
  } while(bc != 0);
  
  fclose(out);
  fclose(in);
  
  return 0;
}