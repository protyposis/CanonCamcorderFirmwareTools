/*
 * XOR
 * XORs a file's content with itself at a specified offset (which should be the key length).
 * 
 * To get the XOR key, 
 *   1. XOR a firmware file with itself at the determined XOR key length, 
 *   2. look for an empty/zero section with a length >= the key length,
 *   3. calculate a fitting key start position (position_inside_the_empty_section % key_length == header_length)
 *   4. extract key_length bytes from the original firmware file at the calculated position
 *   5. the inverted extracted data is the XOR key
 *
 * Copyright 2014 Mario Guggenberger <mg@protyposis.net>
 */
#include <stdio.h>
#include <stddef.h>

int main(int argc, char *argv[])
{
  FILE *in, *out;
  int offset;
  int size = -1;
  int val;
  int index;
  char * fileData;


  if (argc != 4) {
    printf("Usage: xor inputfile offset outfile\n");
    return -1;
  }

  if ((in = fopen(argv[1], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[1]);
    return -1;
  }

  offset = atoi(argv[2]);

  if ((out = fopen(argv[3], "wb")) == NULL) {
    printf("Cant't open file name %s\n", argv[3]);
    fclose(in);
    return -1;
  }

  printf("offset %i bytes\n", offset);
  
  // read file
  fseek(in, 0, SEEK_END);
  size = ftell(in);
  printf("filesize %i bytes\n", size);
  fileData = (char *) calloc(size, sizeof(char));
  
  rewind(in);
  index = 0;
  while((val = fgetc(in)) != EOF) {
	fileData[index++] = val;
  }
  fclose(in);
  
  // xor & write file
  for(index = 0; index < size; index++) {
    fputc(fileData[index] ^ fileData[(index + offset) % size], out);
  }
  fclose(out);

  return 0;
}