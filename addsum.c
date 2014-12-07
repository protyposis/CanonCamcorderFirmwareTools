#include <stdio.h>
#include <stddef.h>

/*
 * Calculates the checksum of a file as a simple sum of integers.
 * Now working! (endianness was wrong before)
 */

/* http://www.codeguru.com/forum/showthread.php?t=292902 */
unsigned int endian_swap(unsigned int x) {
    return (x>>24) | 
        ((x<<8) & 0x00FF0000) |
        ((x>>8) & 0x0000FF00) |
        (x<<24);
}

int main(int argc, char *argv[]) {
  FILE *in;
  long filesize;
  unsigned int sum = 0;
  unsigned int buffer;
  int count = 0;
  int start = 0;
  int end = 0;

  if (argc < 2) {
    printf("Usage: addsum inputfile [startoffset [endoffset]]\n");
    return -1;
  }

  if ((in = fopen(argv[1], "rb")) == NULL) {
    printf("Cant't open file name %s\n", argv[1]);
    return -1;
  }

  if(argc >= 3) {
    start = atoi(argv[2]);
    if(argc >= 4) {
      end = atoi(argv[3]);
    }
  }
  
  // determine file size
  fseek(in, 0, SEEK_END);
  filesize = ftell(in);
  rewind(in);
  printf("file size: %d (0x%08X) bytes\n", filesize, filesize);

  if(end == 0) {
    end = filesize;
  } else if(end < 0) {
    end = filesize - end;
  }

  printf("calc range: %d - %d = %d bytes (0x%08X - 0x%08X = 0x%08X)\n", 
    start, end, end - start, start, end, end - start);
  
  fseek(in, start, SEEK_CUR);
  for(count = 0; count < (end - start) / 4; count++) {
    fread(&buffer,4,1,in);
    sum += endian_swap(buffer);
  }
  printf("checksum = 0x%08X\n", sum, count * 4);

  fclose(in);
  
  return 0;
}
