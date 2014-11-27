/*
 * Canon Firmware Decrypter
 * Copyright Alex Bernstein, 2003
 * http://alexbernstein.com/wiki/canon-firmware-decrypter/
 */
#include <stdio.h>
#include <stddef.h>
#include <getopt.h>

#include "300D_table.h"
//#include "20D_table.h"
//#include "40D_table.h"

#ifndef CRYPT1_SIZE 
  #define CRYPT1_SIZE (sizeof(crypt1)/sizeof(crypt1[0]))
#endif
#ifndef CRYPT2_SIZE 
  #define CRYPT2_SIZE (sizeof(crypt2)/sizeof(crypt2[0]))
#endif
#define SB_SIZE 100			// stringbuffer size
#define MAX_XOR_SHIFT 513
#define MIN_OUT_LEN 10		// smaller ones are pointless
//#define VERBOSE

int check_ascii(char num) {
  // bigacsii, smallascii, space
  if((num >= 65 && num <= 90) || (num >= 97 && num <= 122) || num == 32)
    return 1;
  return 0;
}

int analyze(char *in_name , char *log_name, int offset) {
	FILE *in, *log;
	int i = 0, j = 0, c, z, val;
	long lSize;
	char * buffer, * buffer_dec;
	size_t result;
	char stringbuffer[SB_SIZE];
	int stringbuffer_pos = 0;
	int xor_shift = 0;
	long s_start;
	int s_start_c1, s_start_c2;
	

	if ((in = fopen(in_name, "rb")) == NULL) {
		printf("Cant't open file name %s\n", in_name);
		return -1;
	}
  
	if(log_name !=  NULL) {
		if ((log = fopen(log_name, "w")) == NULL) {
			printf("Cant't open log file name %s\n", log_name);
			return -1;
		}
	}
	
	// obtain file size:
	fseek (in , 0 , SEEK_END);
	lSize = ftell (in);
	rewind (in);

	// allocate memory for input file
	buffer = (char*) malloc (sizeof(char)*lSize);
	if (buffer == NULL) {fputs ("Memory error",stderr); exit (2);}

	// copy input file into the buffer and close input file
	result = fread (buffer,1,lSize,in);
	if (result != lSize) {fputs ("Reading error",stderr); exit (3);}
	fclose(in);
	
	// allocate memory for decrypted file
	buffer_dec = (char*) malloc (sizeof(char)*lSize);
	if (buffer_dec == NULL) {fputs ("Memory error outputbuffer",stderr); exit (2);}
	
	for(xor_shift = 0; xor_shift <= MAX_XOR_SHIFT; xor_shift++) {
	  printf("shift: %i\n", xor_shift);
	  //i = 0;
	  //j = 0;
	  
	  if(log != NULL) fprintf (log, "shift: %i\n", xor_shift);
	  for(c = xor_shift + offset; c < lSize - xor_shift - offset; c++) { 
	    buffer_dec[c] = buffer[c] ^ crypt1[i] ^ crypt2[j];
		
		// if current char is ascii char
		if(check_ascii(buffer_dec[c]) != 0) {
			// if stringbuffer_pos == 0 a new string might begin, so init variables that save string begin pos, and keys
			if(stringbuffer_pos == 0) { s_start = c; s_start_c1 = i; s_start_c2 = j; }
			// add the char to stringbuffer
			stringbuffer[stringbuffer_pos++] = buffer_dec[c];
		}
		// if current char is no ascii char and stringbuffer is not empty
		else if(stringbuffer_pos > 0) {
		  // if buffered string >= MIN_OUT_LEN chars, print it and clear buffer
		  if(stringbuffer_pos >= MIN_OUT_LEN) {
		  	#ifdef VERBOSE
			printf("possible string: %s\n", stringbuffer);
			#endif
			if(log != NULL) fprintf (log, "%010x %3i %3i %s\n", s_start, s_start_c1, s_start_c2, stringbuffer);
		    memset((void*)&stringbuffer, 0, sizeof(char)*SB_SIZE);
		  }
		  stringbuffer_pos = 0;
		}
		
		i++;
	    j++;
	    if (i >= CRYPT1_SIZE) i=0;
	    if (j >= CRYPT2_SIZE) j=0;
	  }
	  
	} // xor_shift
	
	if(log != NULL)
		fclose(log);
	free(buffer);
	free(buffer_dec);
	
	return 0;
}

void print_usage() {
	printf("Usage: decrypt [options] inputfile outfile\n");
	printf("options:\n");
	printf("    -a            analysis mode (default: decryption mode)\n");
	printf("    -s <number>   decryption start offset (in decryption mode)\n");
}

int main(int argc, char *argv[])
{
  FILE *in;
  FILE *out;
  int i = 0, j = 0, val;
  
  int c;
  int analyzeflag = 0;
  
  int decstart = 0;
  
  while ((c = getopt (argc, argv, "as:hi:j:")) != -1) {
        switch (c) {
           case 'a':
             analyzeflag = 1;
             break;
           case 's':
             decstart = atoi(optarg);
             break;
		   case 'i':
             i = atoi(optarg);
             break;
		   case 'j':
             j = atoi(optarg);
             break;
		   
		   case 'h':
		     print_usage();
			 return 0;
        }
  }
  
  if(analyzeflag != 0) {
    if(argc > (optind + 1))
	  analyze(argv[optind], argv[optind + 1], decstart);
	else
	  analyze(argv[optind], NULL, decstart);
	  
	return 0;
  }

  if (argc - optind + 1 != 3) {
    print_usage();
    return -1;
  }

  if ((in = fopen(argv[optind], "rb")) == NULL) {
    printf("Cant't open input file name %s\n", argv[optind]);
    return -1;
  }
  
  if ((out = fopen(argv[optind + 1], "wb")) == NULL) {
    printf("Cant't open output file name %s\n", argv[optind + 1]);
    fclose(in);
    return -1;
  }
  
  // forward to start byte by offset
  if(decstart > 0)
    printf("forwarding to byte %i\n", decstart);
	
  while(decstart-- > 0)
    fputc(fgetc(in), out);
	
  while ((val = fgetc(in)) != EOF) {
    fputc(val ^ crypt1[i] ^ crypt2[j], out);
    i++;
    j++;
    if (i >= CRYPT1_SIZE) i=0;
    if (j >= CRYPT2_SIZE) j=0;
  }
  fclose(in);
  fclose(out);
}
