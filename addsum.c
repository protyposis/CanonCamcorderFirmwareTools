#include <stdio.h>
#include <stddef.h>

/*
 * Calculates the checksum of a file as a simple sum of integers.
 * Doesn't match with the fw checksums :(
 */

int main(int argc, char *argv[])
{
  FILE *in;
  long filesize;
  int sum = 0;
  int buffer;
  int count = 0;

  if (argc != 2) {
    printf("Usage: addsum inputfile\n");
    return -1;
  }

  if ((in = fopen(argv[1], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[1]);
    return -1;
  }
  
  fseek (in , 0 , SEEK_END);
  filesize = ftell (in);
  rewind (in);

  
  do {
	//fputc(fgetc(in), out);
	fread(&buffer,4,1,in);
	sum = sum + buffer;
	count++;
	printf("int %x\n", buffer);
  } while(count / 4 < filesize);

  fclose(in);
  
  printf("sum %x\n", sum);
  
  return 0;
}
