/*
 * XOR Analyzer
 * XORs a file's content with itself with a sequence of offsets to determine the XOR key length.
 * 
 * The offset that equals the key length will result in a considerably higher count/percentage than the rest.
 *
 * Copyright 2008, 2014 Mario Guggenberger <mg@protyposis.net>
 */
#include <stdio.h>
#include <stddef.h>

#define KEYSIZE_MIN 1
#define KEYSIZE_MAX 5000
#define NUM_RESULTS 10

//#define getArraySize(x)  (sizeof(x)/sizeof(x[0]))


int count(char *data, int size) {
	int i;
	int c = 0;
	int l = size;
	
	for (i = 0; i < l; i++) {
		if (data[i] == (unsigned char)0)
			c++;
	}
	return c;
}

void shiftxor(char *fileData, char *fileDataTemp, int size, int shift) {
	int l = size;
	int i;
	
	//printf("%i %i\n", size, shift);
	
	for(i = 0; i < l; i++) {
		fileDataTemp[i] = fileData[i] ^ fileData[(i + shift) % l];
	}
}

int shift_count(char *fileData, char *fileDataTemp, int size, int shift) {
	
	shiftxor(fileData, fileDataTemp, size, shift);
	return count(fileDataTemp, size);
}


int main(int argc, char *argv[])
{
  FILE *in, *out;
  char outFileName[255];
  int size = -1;
  int val;
  
  char * fileData;
  char * fileDataTemp;
  int round = 0;
  int count;
  int forcount = 0;


  if (argc != 2 && argc != 3) {
    printf("Usage: xoranal inputfile [outfile]\n");
    return -1;
  }

  if ((in = fopen(argv[1], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[1]);
    return -1;
  }

  // generate output name from input name if no output name given
  if (argc == 2) {
  	snprintf(outFileName, 200, "%s.xoranal.txt", argv[1]);
  } else {
  	strcpy(outFileName, argv[2]);
  }
  if ((out = fopen(outFileName, "wb")) == NULL) {
    printf("Cant't open file name %s\n", outFileName);
    fclose(in);
    return -1;
  }

  printf("%s length %i\n", outFileName, strlen(outFileName));
  
  // read file
  fseek(in, 0, SEEK_END);
  size = ftell(in);
  printf("filesize %i bytes\n", size);
  fileData = (char *) calloc(size, sizeof(char));
  fileDataTemp = (char *) calloc(size, sizeof(char));
  
  rewind(in);
  forcount = 0;
  while((val = fgetc(in)) != EOF) {
	fileData[forcount++] = val;
  }
  fclose(in);

  while(++round <= KEYSIZE_MAX) {
	count = shift_count(fileData, fileDataTemp, size, round);
	fprintf(out, "%10i %10i %f%%\n", round, count, 100 * (float)count / (float)size);
	
	if(round % 10 == 0)
		printf("round %4i / %4i finished\n", round, KEYSIZE_MAX);
  }
  
  fclose(out);
  
  printf("finished, see %s for results\n", outFileName);
  
  return 0;
}
