#include <stdio.h>
#include <stddef.h>

/*
 * Calculates the checksum of a file as a simple sum of integers.
 * Now working! (endianness was wrong before)
 */

/* http://www.codeguru.com/forum/showthread.php?t=292902 */
unsigned int endian_swap(unsigned int x)
{
    return (x>>24) | 
        ((x<<8) & 0x00FF0000) |
        ((x>>8) & 0x0000FF00) |
        (x<<24);
}

int main(int argc, char *argv[])
{
  FILE *in;
  long filesize;
  unsigned int sum = 0;
  unsigned int buffer;
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
  printf("file size: %d (0x%08x) bytes\n", filesize, filesize);

  
  do {
	//fputc(fgetc(in), out);
	fread(&buffer,4,1,in);
	sum += endian_swap(buffer);
	count++;
	//printf("int %x swapped %x\n", buffer, endian_swap(buffer));
  } while(count * 4 < filesize);
  printf("checksum %x calculated over %d bytes\n", sum, count * 4);

  fclose(in);
  
  printf("sum %x\n", sum);
  
  return 0;
}
