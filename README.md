# StegoCoder
A Steganography En- and Decoder for .Net and Cmd

## [NuGet]()

[![NuGet](https://img.shields.io/nuget/dt/StegoCoder.svg?style=flat&label=StegoCoder&logo=nuget&color=#6A994E)](https://www.nuget.org/packages/StegoCoder/)

__How to install:__ (Visual Studio) 
1. Right-click your project in the Solution Explorer
2. Click 'Manage NuGet Packages...'
3. Select the 'Browse' tab
4. Enter 'StegoCoder' in the search bar
5. Select the result, click 'Install' and 'OK'
6. Add a `using static` statement at the top of your code


## [Cmd](https://github.com/1TheCrazy/StegoCoder/blob/main/CMD/cmd.zip)
__How to install:__ (Windows)
1. Download the [cmd.zip](https://github.com/1TheCrazy/StegoCoder/blob/main/CMD/cmd.zip)-file
2. Extract the cmd folder
3. Run the *install_cmd.bat*-file (grant admin privileges if asked)
4. Run *stegocoder* in a new CMD window

### How it works
This package uses the most common steganography tehnique: it manipulates the LSB (Least Significant Bit) of every pixels color bytes (RGBA). It converts a string of characters to bits according to the ASCII standart and loops through the images bytes, manipulating the LSB as needed.
When decoding, it reads the LSB of the image bytes, converting them back to a string.
At the end of every string encoded in an image, the program appends 25 in binary (00011001), signaling the End of Medium and when the program needs to stop parsing from bits to strings.

### Packages used
This program heavily uses ImageSharp by SixLabors, expecially when loading and looping through the images.
