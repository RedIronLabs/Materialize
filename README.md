# Sponsor
I spent dozens of hours improving this, feel free to sponsor, but no problem if you dont.
# Materialize
Materialize is a program for converting images to materials for use mainly but not restrict to video games.

## Motivation
I decided to port materialize to linux, since the original is for windows only. I will keep improving it.

## Contact
For sugestions, doubts or anything related to this port.
- Email : mk2play.materialize@gmail.com

## Better Suportted On

- Ubuntu 16.04, 18.04
- CentOS 7
- x86-64 architecture
- Gnome desktop environment running on top of X11 windowing system
- Nvidia official proprietary graphics driver, AMD Mesa graphics driver
- Desktop form factors, running on device/hardware without emulation or compatibility layer

## Using
To use, unity is not necessary, you can use like a normal linux application.

## Building
I'm developing using Unity 2018.3.3f, then, is recomended to use it also. You can try to downgrade or upgrade the package, but mainly downgrading, something can go wrong.

## Added features
### Paste Images from clipboard on Linux
- You can copy a file in your file browser (Tested with nautilus) and then press  the "P" close to the slot you want to paste.
- **Highlight** - You can also press copy image on browser and it will paste also. This make it fast to take a image from internet
### Hide Gui while Rotating / Panning
- The GUI is hidden when panning/rotating the material plane.
### Native File Picker
- Added a new native file/folder picker - Unity Standalone File Browser - https://github.com/gkngkc/UnityStandaloneFileBrowser - Thanks to @gkngkc for the amazing work.
 
## Changed from original
### Save and Load Project
- When you save your project, every map will be saved in the same place, with them respective types, ex:myTexture_Diffuse.png.
- The extension used will be the one set in the GUI Panel.
#### Suported extensions
##### Save
- jpg
- png
- tga
- exr

##### Load
- jpg
- png
- tga
- exr
- bmp

## Not implemented
- QuickSave - Will implement in settings, then you can set the folder to save the texture. This will be a persistent setting, that means you can close and open the program without lose the Quick Save path. *Planed for v0.4*.
- Copy to clipboard. *Planed for v0.4*.
