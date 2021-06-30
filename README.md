# This is a modified version of [Soembodi](https://github.com/Soembodi)'s [Localicer](https://github.com/Soembodi/Localicer)  

# Local[ice](https://frostytoolsuite.com/)r <img src = "src/Localicer/Frosty.png" width="20px">
Yet another frostbite localization editor. As all editors I have come across are tedious 
to work with and can't mass replace all entries at once I decided to make this.

This editor was made specifically for Battlefront 2, although it should work for any valid
localization chunk file.

## Getting started
To open a chunk file click the open button. All the entries from the file will be loaded
and their respective hashes and entries will be shown in the list. To edit the value of an entry
you can double click it which will open an editor. You can also search for occurrences of certain
text in the entry values. To save the file press the save file and 
export it to your favorite folder.

## Replacing entry values
The editor can mass replace the values of all entries in the list without you needing to
do it yourself. The replace textbox is the text you want to replace, the with textbox what
you want to replace it with. The ignore case flag tells the editor to ignore the case of the
text to be replaced. If ignore case is checked the "apply replaced text's case" option is enabled.
This tells the editor to ignore case when searching for text to be replaced, but the text
that will replace it will have the same case as the original text, **character for character**.
If the replacement text is longer than the original, the out of range characters' case will be the same
as the last in range character's case.
I.e. if you replace MauL with vader the replacement will be VadER.

## Replacing multiple entry values (new)
A file can be created with the following format:  

value1::newvalue1  
value2::newvalue2  

This file can be loaded and all values will be replaced. At the moment all replacements are case-sensitive and applied only to whole words

## Choose encoding (new)
You now have a choice between two encodings: default and cyrillic. Note that you have to reopen the file after changing the encoding.  
Warning: cyrillic encoding is not full and break some in-game text

### IMPORTANT
Entry values starting with "ID_" will NEVER be mass replaced to avoid problems with Star Cards in Battlefront 2.

# Used libraries
* [ObjectListView](http://objectlistview.sourceforge.net/) - For the entry list viewer
* [Metro Framework](https://thielj.github.io/MetroFramework/) - For application design
* [Costura.Fody](https://github.com/Fody/Costura) - For .dll embedding

**Please tell me if you find any bugs**
