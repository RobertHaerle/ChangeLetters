# ChangeLetters

change unknown letters in NAS file names to readable ones the file system can handle.
The NAS uses an ASUS tool, that can not create characters from outside the ASCII environment. Every charater is exchanged by a '?'
This is neither readable by the File Explorer of the Asustor NAS nor by the Windows File Explorer.
This may be a codepage problem of the software. But there is no possibility to set up anything.
ChangeLetters uses a database with vocabulary to exchange the characters into the correct ones.