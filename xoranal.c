/*
 * XOR Analyzer
 * XORs a file's content with itself with a sequence of offsets to determine the XOR key length.
 *
 * Copyright 2008 mg1984@gmx.at
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

int write_to_file(char *fileData, int size, int round) {
	char name[15];
	FILE *f;
	int i;
	
	snprintf(name, 10, "%i.bin", round);
	if ((f = fopen(name, "wb")) == NULL) {
		printf("Cant't open file name %s\n", name);
		return -1;
	}
  
	for(i = 0; i < size; i++)
		fputc(fileData[i], f);
		
	fclose(f);
	
	return 0;
}

int main(int argc, char *argv[])
{
  FILE *in, *out;
  int size = -1;
  int val;
  
  char * fileData;
  char * fileDataTemp;
  int cCount[KEYSIZE_MAX];
  int round = 0;
  int count;
  int bestrounds[NUM_RESULTS];
  int forcount = 0;


  if (argc != 3) {
    printf("Usage: xoranal inputfile outfile\n");
    return -1;
  }

  if ((in = fopen(argv[1], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[1]);
    return -1;
  }

  if ((out = fopen(argv[2], "wb")) == NULL) {
    printf("Cant't open file name %s\n", argv[2]);
    fclose(in);
    return -1;
  }
  
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
  //write_to_file(fileData, size, 9999);
	

  while(++round <= KEYSIZE_MAX) {
	count = shift_count(fileData, fileDataTemp, size, round);
	fprintf(out, "%10i %10i %f%%\n", round, count, 100 * (float)count / (float)size);
	
	//write_to_file(fileDataTemp, size, round);
	
	if(count >= cCount[bestrounds[0]]) {
		for(forcount = NUM_RESULTS - 1; forcount > 0; forcount--)
			bestrounds[forcount] = bestrounds[forcount - 1];
		bestrounds[0] = round;
	}
	
	if(round % 10 == 0)
		printf("round %4i / %4i finished\n", round, KEYSIZE_MAX);
  }
  
  fclose(out);
  
  
  return 0;
}
