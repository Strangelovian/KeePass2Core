# KeePass2 lib, built as a dot net standard library
## This was accomplished with changes as small as possible to the original keepass2 source code.
### Example 1: CryptoRandom.cs
The following C# macros do the bulk of the job, at the beginning of the file:
```chsharp
#if NETSTANDARD2_0
#define KeePassUAP
#define KeePassLibSD
using System.Security.Cryptography;
#endif
```
The following code was removed, else compilation fails with macro KeePassUAP (DiagnosticsExt does not exist anywhere):
```chsharp
#if KeePassUAP
			pb = DiagnosticsExt.GetProcessEntropy();
			MemUtil.Write(ms, pb);
```
### Example 2: CryptoUtil.cs
The in-memory data protection has to be changed for dot net standard. The ProtectedData API is Windows only. So, I used instead Microsoft.AspNetCore.DataProtection, which is not platform dependant:
```chsharp
#if NETSTANDARD2_0
using Microsoft.AspNetCore.DataProtection;
#endif
```
Here's how the two methods ProtectData and UnprotectData are changed to use Microsoft.AspNetCore.DataProtection:
```chsharp
#if NETSTANDARD2_0
		public static byte[] ProtectData(byte[] pb, byte[] pbOptEntropy,
			DataProtectionScope s)
		{
			return g_obProtector.Protect(pb);
		}

		public static byte[] UnprotectData(byte[] pb, byte[] pbOptEntropy,
			DataProtectionScope s)
		{
			return g_obProtector.Unprotect(pb);
		}
#else
```

## This repository contains two visual studio solutions:
- NetStd.sln:
  - KeePassLibStd.csproj: builds a read / write capable dot net standard version of the KeePass lib
  - Kdbx2xml: command line tool used for testing purposes
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
