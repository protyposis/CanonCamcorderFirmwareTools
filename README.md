Canon Camcorder Firmware Tools
==============================

This is a collection of tools to decrypt, analyze and re-encrypt firmware update files (*.FIM) for Canon camcorders (it does not work for still camera or DSLR *.FIR update files). Development on this has started in 2008 with the decryption of the HF10 and HV30 updates, when parts of the code were first posted on the CHDK forum. It has since been extended to support all currently published FW updates.

Further info on Canon DV firmware:

 * [HF10 & HV30 (Digic DV II) decrypted!](http://chdk.setepontos.com/index.php?topic=1641.0)
 * [HF10/100 Firmware Analysis](http://chdk.wikia.com/wiki/HF10/100_Firmware_Analysis)
 * [HF10/HF100](http://chdk.wikia.com/wiki/HF10/HF100)
 * [HV30 Firmware Analysis](http://chdk.wikia.com/wiki/HV30_Firmware_Analysis)
 * [Official Firmware Hacking Thread (HV20 & HV30 Hack download)](http://hddv.net/showthread.php?20098-Official-Firmware-Hacking-Thread-(new)) (Specifically posts by _jollyrogerxp_ who writes about his incredible journey of analyzing the inner workings of the HV30, dumping the HV20, and releasing hacks for PAL/NTSC switching, display flip and no-gain)

Contents
--------

This repository contains a few C tools and some C# applications. The most important tool is **ud**, the universal firmware decrypter/encrypter.

 * **ud.c**: universal firmware decrypter/encrypter, which supports every released firmware update file [at least until Nov. 2014]
 * **addsum.c**: simple tool to calculate checksums
 * **xoranal.c**: tool to determine the XOR key length (which is 4160 for all known updates)
 * **xor.c**: tool to XOR a file with itself at a given offset to help extracting the XOR key (this is done automatically by the universal decrypter for unknown updates)

The C# tools in the _tools_ subfolder are:

 * **Can(n)on Firmware Encryption Analyzer**: A tool written to analyze the encryption algorithm and reconstruct the key calculation algorithm used to derive the 4160 byte key from the first 64/65 bytes of the 300D keys. It provides a hex view of a FW file where you can select a region and let the tool compute the results of XOR operations for all possible combinations of the 300D keys, that can be limited with a search string or a regular expression (e.g. to only show possible readable text strings).
 * **HF10 Bitmap Viewer**: A tool to visualize and browse bitmaps in a firmware file. Works for the HV30 too, other models not tested.
 * **LCD Data Interpreter**: Simple OCR tool to read text from the camera screen. Planned to be used for firmware extraction of the HV30 camera, but never finished because it became unnecessary.

### Universal Firmware Decrypter ###

The XOR key used to encrypt/decrypt the firmware update files is 4160 bytes long and calculated from the first 64 and 65 bytes of the two 300D encryption keys found by Alex Bernstein and [available on his website](http://alexbernstein.com/wiki/canon-firmware-decrypter/). The 64/65 byte source keys are part of every firmware update file and could theoretically be changed by future update files (which wouldn't solve the weakness of the XOR encryption though). The only parameter that changes from model to model is an offset into the 4160 byte key at which the de-/encryption routine starts, which is basically what seems to be an internal model number (e.g. D128 for the HF10).

The decrypter/encrypter currently supports and was tested with firmware update files of the following models:

 * HF10 / HF100
 * HV30
 * HG20 / HG21
 * HF11
 * XF 300 / XF 305
 * HF S21
 * XF 100 / XF 105
 * XA10
 * HF R30/R32/R36/R37/R38
 * HF M50/M52/M56
 * HF R40/R42/R46/R47/R48
 * XA 35 / XA 20 / HF G30
 * mini

It is also able to automatically determine the decryption offset parameter for updates of unknown models and should therefore the usable with every released firmware update file.

The file structure of the update files has changed over time, only the oldest one is currently known (HF10, HV30, HG20, HF11). For these updates, the decrypter can optionally split the file into the sections it is composed of, and also update the checksum to enable updating cameras with a modified FW update file.


#### Usage ####

First, compile **ud** with GCC (tested with 4.8.1/MinGW on Windows 8.1 x64), e.g. by executing `gcc ud.c -o ud.exe`. To **decrypt** a file, run **ud** on the encrypted file, e.g. `ud VEF1.FIM VEF1.DEC`. To **encrypt** an unencrypted file, apply **ud** similarly, e.g. `ud VEF1.DEC VEF1.FIM`. If you have modified any data or program logic, you need to update the checksums (`-c` option) which is currently only supported for the oldest models, else the update fails. To successfully apply a FW update on the camera, you also need to increment the version number (e.g. 1.0.1.0 to 1.0.1.1) or alternatively modify the update routine to skip the version check (like [here](http://hddv.net/showthread.php?20098-Official-Firmware-Hacking-Thread-(new)&p=213532&viewfull=1#post213532)).


License
-------

Copyright (C) 2008, 2014 Mario Guggenberger <mg@protyposis.net>.
This project is released under the terms of the GNU General Public License. See `LICENSE` for details.