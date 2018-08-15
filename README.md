# KeePass2 lib as a dot net standard library
This was accomplished with changes as small as possible to the original keepass2 source code.

This repository contains two visual studio solutions:
- NetStd.sln:
  - KeePassLibStd.csproj: builds a read / write capable dot net standard version of the KeePass lib
  - Kdbx2xml: command line tool used 
- KeePass.sln: same as the original KeePass2 solution, with key signing removed for convenience

Dummy kdbx files are provided in the DummyDatabases folder.

## Command line samples:
### Export to plain text XML
```
> dotnet Kdbx2XmlConsole.dll --kdbx kdf-argon2_cipher-chacha20.kdbx --password "Her face was large as that of Memphian sphinx"
1,1097866s to export [kdf-argon2_cipher-chacha20.kdbx] to [kdf-argon2_cipher-chacha20.xml]
```

### Load and save the same KDBX file, adding a .dup.kdbx extenstion
```
>dotnet Kdbx2XmlConsole.dll --kdbx "kdf-argon2_cipher-chacha20.kdbx" --password "Her face was large as that of Memphian sphinx" --mode dup
1,06377s to load [kdf-argon2_cipher-chacha20.kdbx]
1,06377s to save [kdf-argon2_cipher-chacha20.kdbx] to [kdf-argon2_cipher-chacha20.dup.kdbx]
```
