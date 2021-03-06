﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cannon
{
    public static class KeyTable
    {
        public static byte[] crypt1 = {
            0x41, 0xCA, 0x42, 0xF9, 0x3C, 0x6D, 0x9D, 0xDF, 0xD8, 0x19, 0x84, 0x08, 0x50, 0xFB, 0x67, 0xA1,
            0xAD, 0x35, 0xF2, 0xCE, 0xDF, 0x9A, 0xF3, 0xC4, 0xF5, 0x22, 0x1A, 0x8A, 0xD5, 0x64, 0x08, 0x65,
            0xD3, 0x2C, 0x96, 0xB6, 0x08, 0x11, 0x3A, 0xB3, 0x90, 0x08, 0xE6, 0x2E, 0x88, 0xED, 0x24, 0xFB,
            0x24, 0x28, 0x82, 0x3D, 0x39, 0xA8, 0xF3, 0x52, 0x98, 0xF9, 0xA7, 0x75, 0x52, 0x63, 0x0A, 0xF9,
            0xED, 0x7A, 0x82, 0x02, 0x1F, 0x94, 0x76, 0xE1, 0xBA, 0xB6, 0xB9, 0x7B, 0x6C, 0xA2, 0xDA, 0x16,
            0x5B, 0x58, 0x51, 0x20, 0xBC, 0xF1, 0xD8, 0x34, 0x36, 0xAA, 0xD1, 0xA5, 0xEF, 0xF4, 0xE8, 0x50,
            0xDD, 0x09, 0x95, 0x8D, 0xC9, 0x06, 0x8D, 0xF0, 0x2D, 0xA2, 0xF1, 0x0D, 0x5E, 0x14, 0x09, 0x10,
            0x3E, 0x9E, 0x58, 0xCA, 0x39, 0xAB, 0x11, 0x4F, 0x7F, 0x08, 0x8B, 0xCC, 0x4D, 0x9E, 0xD7, 0x20,
            0x1D, 0x8F, 0xCA, 0x46, 0xAE, 0x17, 0xB5, 0x6B, 0xE6, 0x0A, 0xE9, 0x9A, 0x6E, 0x4A, 0x4C, 0xCC,
            0xA6, 0xEB, 0xD8, 0x2B, 0xF1, 0xB0, 0x2F, 0xFB, 0x5F, 0xF7, 0x57, 0x06, 0xB3, 0x77, 0xFC, 0x1B,
            0x98, 0xD9, 0xBA, 0x5A, 0x5F, 0xA4, 0xE0, 0x1F, 0x74, 0x99, 0xF1, 0x69, 0xA3, 0x8A, 0x43, 0x99,
            0xCE, 0x77, 0x04, 0xCC, 0x47, 0x7D, 0xCB, 0x39, 0x6E, 0x2D, 0x5A, 0x59, 0xE4, 0x92, 0x91, 0x37,
            0x1D, 0x60, 0xAA, 0xE5, 0x4F, 0xF8, 0xFA, 0xC1, 0x59, 0x52, 0x2C, 0xAB, 0x09, 0x0F, 0xEE, 0x5E,
            0x5B, 0xC0, 0x64, 0xB3, 0xF5, 0xC2, 0xD3, 0xCC, 0x1E, 0x2C, 0xB4, 0x0B, 0xA8, 0xBF, 0x40, 0x77,
            0x08, 0xA0, 0xC9, 0x93, 0xDF, 0xA8, 0x79, 0x28, 0x41, 0x93, 0x37, 0x2B, 0xF7, 0xD4, 0xAD, 0x1B,
            0x18, 0x9E, 0xE0, 0xB4, 0x59, 0x60, 0x58, 0xEA, 0xAB, 0x2A, 0x6C, 0xD4, 0x07, 0xEE, 0xEF, 0x18,
            0x65, 0xFF, 0x7F, 0x6F, 0x1E, 0xDE, 0xF3, 0x83, 0xF4, 0xB6, 0xF8, 0x7E, 0x0E, 0xBD, 0xB3, 0x6A,
            0x91, 0x5D, 0x24, 0x9F, 0x97, 0x74, 0x60, 0xD6, 0xC9, 0xE2, 0xA6, 0x54, 0x49, 0xE8, 0xB2, 0x6B,
            0x4E, 0xBD, 0x41, 0x2C, 0xC9, 0x40, 0xCF, 0x85, 0x0A, 0xCA, 0x8E, 0x67, 0x99, 0x43, 0x2C, 0x9A,
            0x56, 0x3E, 0x26, 0x23, 0x80, 0xA5, 0x27, 0xD9, 0x05, 0xD4, 0x82, 0x6E, 0x65, 0xE9, 0x81, 0x4E,
            0xB9, 0x58, 0xE3, 0x3F, 0x6A, 0xEA, 0x5C, 0xE7, 0x83, 0x13, 0x2A, 0x63, 0x59, 0x60, 0x24, 0x7A,
            0x0C, 0x6D, 0x79, 0x80, 0x13, 0xED, 0x25, 0xA2, 0x50, 0x15, 0x4B, 0xB3, 0x03, 0x62, 0x33, 0xF5,
            0x60, 0x9D, 0x3F, 0xC5, 0xE2, 0xB7, 0xC8, 0xAF, 0xC8, 0xEB, 0x81, 0x76, 0x8C, 0x9D, 0xE9, 0x62,
            0xDA, 0x13, 0x99, 0x3B, 0xA5, 0xDE, 0x84, 0xD3, 0x20, 0x24, 0x5C, 0xAB, 0xD9, 0xCB, 0x63, 0xFB,
            0x16, 0x39, 0xB7, 0xB3, 0x91, 0xD3, 0xD3, 0xA8, 0x42, 0xA0, 0xE3, 0x27, 0xE8, 0x45, 0x74, 0xD8,
            0x24, 0xEC, 0xE2, 0xFE, 0x3B, 0x73, 0xF5, 0xDC, 0xBE, 0xB2, 0xED, 0xE6, 0xAD, 0x96, 0xBB, 0x23,
            0xD6, 0x7F, 0x32, 0x03, 0x71, 0xD0, 0x33, 0x85, 0xC1, 0x86, 0x80, 0xE4, 0x07, 0x3F, 0xF1, 0x79,
            0x85, 0xC1, 0xB9, 0xCB, 0x36, 0x8C, 0x9A, 0x01, 0xAD, 0x1E, 0xA2, 0xF1, 0x16, 0x45, 0x50, 0x30,
            0xE9, 0x52, 0x4A, 0x80, 0x88, 0xA7, 0x80, 0x27, 0x03, 0x3E, 0xF3, 0x1D, 0xD7, 0x86, 0x4B, 0xF1,
            0x9D, 0x69, 0x45, 0xE0, 0xD4, 0x31, 0x89, 0xDC, 0x47, 0x45, 0x94, 0xC5, 0x4D, 0x20, 0x22, 0xB4,
            0xAB, 0x71, 0x36, 0x6B, 0xBC, 0xAF, 0x74, 0x20, 0x0E, 0x6B, 0xB0, 0xB8, 0xB5, 0xF0, 0x34, 0xCA,
            0x02, 0xAD, 0x7B, 0x35, 0x3D, 0x50, 0x01, 0x9E, 0x30, 0x7B, 0x69, 0x2F, 0x6A, 0x79, 0xF4, 0xEF
            };

        public static byte[] crypt2 = {
            0xA5, 0xF5, 0x2A, 0xBE, 0x62, 0x66, 0x1C, 0x80, 0x87, 0x3F, 0x91, 0x01, 0x6E, 0xBC, 0x87, 0x02,
            0xD0, 0x2A, 0x3A, 0xC4, 0x65, 0x3C, 0x91, 0x35, 0xB5, 0xAE, 0x21, 0x21, 0xF5, 0x3F, 0xAC, 0x1F,
            0xBE, 0x92, 0xA6, 0xF2, 0xA8, 0x42, 0x39, 0x8F, 0x41, 0x95, 0x91, 0xBD, 0x4C, 0xAD, 0x85, 0xC7,
            0xB7, 0xD6, 0xAE, 0x1C, 0xE2, 0x62, 0xEB, 0xC2, 0x03, 0x5D, 0xD6, 0x4E, 0x7B, 0x59, 0x0F, 0xDD,
            0xB7, 0x3E, 0x22, 0x55, 0x45, 0x91, 0x81, 0x71, 0x71, 0x9F, 0xE8, 0x6D, 0x67, 0x40, 0x8C, 0x43,
            0x30, 0xAA, 0x43, 0x39, 0x3C, 0x35, 0x4E, 0xAF, 0x67, 0x50, 0xDA, 0xE5, 0xA3, 0xA8, 0x08, 0x9C,
            0x59, 0x3E, 0xEB, 0xD0, 0x01, 0xFB, 0xE5, 0x15, 0x59, 0xFF, 0x76, 0xD7, 0x10, 0x39, 0x88, 0x41,
            0x85, 0xE1, 0xF4, 0x43, 0xE3, 0x66, 0xFE, 0x29, 0x7E, 0x50, 0xD4, 0x82, 0x0F, 0x17, 0x38, 0x10,
            0x45, 0x54, 0x3A, 0x53, 0xF8, 0x1C, 0xA1, 0x08, 0x5B, 0x60, 0x25, 0xCA, 0xF7, 0xE7, 0x35, 0x3F,
            0xC3, 0xF9, 0x02, 0x58, 0x14, 0xE6, 0x71, 0x16, 0x03, 0x1C, 0x8E, 0x0F, 0x34, 0xFE, 0xF5, 0x0A,
            0x31, 0x03, 0x49, 0x41, 0x07, 0xB3, 0x63, 0x21, 0xED, 0x5B, 0x40, 0xF2, 0xE2, 0x24, 0x14, 0x63,
            0x8B, 0x45, 0x9D, 0xA0, 0x65, 0x7A, 0xCE, 0xDF, 0x90, 0xD6, 0xC5, 0x7B, 0x26, 0xE3, 0x38, 0xF1,
            0x31, 0x62, 0xB9, 0x60, 0xE7, 0x02, 0x23, 0xEE, 0x10, 0x44, 0x2B, 0x1B, 0x46, 0x07, 0x87, 0x04,
            0xD5, 0xB2, 0x33, 0x8B, 0x97, 0x75, 0xD7, 0x0F, 0x07, 0xB7, 0xBB, 0x83, 0xC8, 0x3E, 0xDA, 0xF4,
            0x5C, 0x70, 0x4D, 0xDF, 0x70, 0x43, 0xF4, 0x15, 0x6D, 0x6C, 0xB2, 0x59, 0x77, 0xEA, 0xB5, 0x86,
            0x4B, 0xF7, 0x95, 0x80, 0x2A, 0x74, 0x37, 0x04, 0x50, 0x94, 0xF4, 0x92, 0x6D, 0x45, 0x73, 0x49,
            0x90, 0xA8, 0x22, 0x03, 0xB8, 0x0A, 0x22, 0x2D, 0xBB, 0xAD, 0xDF, 0x54, 0xB5, 0x3D, 0xB7, 0x8C,
            0xDE, 0xE6, 0x2A, 0x7D, 0x31, 0xEB, 0x7E, 0x82, 0xFF, 0x1D, 0xFF, 0xF7, 0x70, 0xE6, 0x57, 0xAC,
            0x34, 0x40, 0x55, 0xEC, 0x75, 0x77, 0x03, 0xF2, 0x29, 0xD5, 0x4C, 0x75, 0xB8, 0xF7, 0x5B, 0x91,
            0x75, 0x8C, 0x54, 0xF3, 0xCB, 0x36, 0xBB, 0xA4, 0xF6, 0x83, 0xD4, 0x74, 0xE4, 0xA8, 0x57, 0xB0,
            0x2B, 0x95, 0x5E, 0x18, 0xFE, 0x66, 0xC1, 0x11, 0xF6, 0xCE, 0xAE, 0xBB, 0x93, 0xE6, 0x8B, 0xAB,
            0x61, 0xFC, 0x93, 0x56, 0xDA, 0x4F, 0x66, 0x12, 0xF1, 0x6A, 0x07, 0xDD, 0x6F, 0x6D, 0xBE, 0x76,
            0xC9, 0xA3, 0xCB, 0xB9, 0x96, 0xB7, 0x57, 0x70, 0x09, 0x3C, 0xE1, 0x93, 0xE9, 0xD3, 0x54, 0x1C,
            0xAA, 0xC4, 0xD1, 0x75, 0xC2, 0x99, 0x12, 0x12, 0xCA, 0x50, 0xB5, 0x32, 0xC7, 0x85, 0x75, 0x3E,
            0xD3, 0x08, 0x75, 0xEF, 0x25, 0xF8, 0xB5, 0xED, 0xB1, 0xC0, 0xD7, 0x25, 0xFB, 0x8A, 0x23, 0xEB,
            0xA0, 0xC1, 0x03, 0x0D, 0xFC, 0x1B, 0xAB, 0x89, 0xAA, 0x6B, 0x76, 0xC4, 0x31, 0x45, 0xA9, 0x1D,
            0x65, 0x3F, 0xFD, 0x08, 0x69, 0x34, 0x15, 0x13, 0x16, 0x42, 0x03, 0x6E, 0x81, 0x36, 0x97, 0x5F,
            0x93, 0x22, 0xFA, 0x50, 0x1F, 0x13, 0x82, 0x61, 0x78, 0xEA, 0x28, 0xD5, 0x68, 0x96, 0xA9, 0x63,
            0x55, 0x98, 0x2F, 0x24, 0xFB, 0x42, 0x76, 0xF9, 0x1F, 0xBF, 0x79, 0x28, 0x22, 0xC7, 0xC1, 0x17,
            0x36, 0x1D, 0xC0, 0xB5, 0x51, 0x42, 0x6D, 0x52, 0x3D, 0x56, 0x8C, 0x67, 0x25, 0xC8, 0x02, 0x04,
            0x26, 0xE2, 0xBC, 0x47, 0xD1, 0x5A, 0xB7, 0x99, 0xC2, 0x8E, 0x01, 0x2D, 0xC2, 0xFE, 0xF0, 0x02,
            0x4E, 0x94, 0x80, 0x14, 0x20, 0xF0, 0x55, 0xD4, 0x58, 0x93, 0x9E, 0x33, 0x5B, 0xA5, 0xE6, 0xC6,
            0x88};

        public static unsafe List<DecryptionResult> decryptTryAll(byte[] input, byte[] matchString, bool regex) {
            int i = 0, j = 0, x, y, ii, jj, pos;
            byte[] bytes_dec = new byte[input.Length];
            int count = 0;
            List<DecryptionResult> results = new List<DecryptionResult>(1);

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            Console.WriteLine("starting dec... " + input.Length + " bytes, matchstring: " + enc.GetString(matchString));
            Console.WriteLine("key 1 length: " + crypt1.Length + " key 2 length: " + crypt2.Length);

            for (i = 0; i < crypt1.Length; i++) {
                for (j = 0; j < crypt2.Length; j++) {
                    ii = i;
                    jj = j;
                    //dec
                    for (x = 0; x < input.Length; x++) {
                        bytes_dec[x] = (byte)(input[x] ^ crypt1[ii] ^ crypt2[jj]);
                        ii = (ii + 1) % (crypt1.Length - 1);
                        jj = (jj + 1) % (crypt2.Length - 1);
                    }
                    //break;
                    //match
                    string decstring = enc.GetString(bytes_dec);
                    string match = enc.GetString(matchString);
                    // regexp match
                    if (regex) {
                        System.Text.RegularExpressions.Match m;
                        m = System.Text.RegularExpressions.Regex.Match(decstring, match);
                        if (m.Success)
                            results.Add(new DecryptionResult(i, j, (byte[])bytes_dec.Clone(), m.Index, m.Length));
                    }
                    // binary match
                    else if(match.StartsWith("0x")) {
                        byte[] matchbytes = new byte[(match.Length - 2) / 2];
                        for (x = 0; x < matchbytes.Length; x++) {
                            matchbytes[x] = byte.Parse(match.Substring(2 + 2 * x, 2), System.Globalization.NumberStyles.HexNumber);
                        }
                        for(x = 0; x < bytes_dec.Length - matchbytes.Length; x++) {
                            for(y = 0; y < matchbytes.Length; y++) {
                                if(matchbytes[y] != bytes_dec[x + y])
                                    break;
                                if(y == matchbytes.Length - 1)
                                    results.Add(new DecryptionResult(i, j, (byte[])bytes_dec.Clone(), x, matchbytes.Length));
                            }
                        }
                    }
                    // string match
                    else if ((pos = decstring.IndexOf(match, StringComparison.Ordinal)) >= 0) {
                        //Console.WriteLine("match! i=" + i + " j=" + j + " data: " + enc.GetString(bytes_dec));
                        results.Add(new DecryptionResult(i, j, (byte[])bytes_dec.Clone(), pos, matchString.Length));
                    }
                    //for (y = 0; y < bytes_dec.Length - matchString.Length; y++) {
                    //    if (bytes_dec[y] != matchString[0])
                    //        continue;
                    //    else {
                    //        for (m = 1; m < matchString.Length; m++) {
                    //            if (matchString[m] != bytes_dec[y + m])
                    //                break;

                    //            if (m == matchString.Length - 1) {
                    //                Console.WriteLine("match! i=" + i + " j=" + j);
                    //                return new DecryptionResult(i, j, bytes_dec, true);
                    //            }
                    //        }
                    //    }
                    //}
                    //Console.WriteLine(enc.GetString(bytes_dec));
                    if(count % 10000 == 0)
                        Console.WriteLine(".");
                    count++;
                }
            }
            Console.WriteLine(results.Count + " match(es) found (" + count + " rounds)");
            return results;
        }

    }

    public struct DecryptionResult {
        public int i, j, result_position, result_length;
        public byte[] bytes;

        public DecryptionResult(int i, int j, byte[] bytes, int position, int length) {
            this.i = i;
            this.j = j;
            this.bytes = bytes;
            this.result_position = position;
            this.result_length = length;
        }
    }
}
