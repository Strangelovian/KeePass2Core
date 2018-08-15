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
The in-memory data protection has to be changed for dot net standard. The ProtectedData API is Windows only. So, I used instead [Microsoft.AspNetCore.DataProtection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-2.1), which is not platform dependant:
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

## Command line samples:
Dummy kdbx files and the unlocking dummy pass phrase are provided in the DummyDatabases folder.
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

## Performance considerations
### Argon2 KDF, same performance
For KDBX files saved with the Argon2 key derivation function (the newer one), the loading/saving time is identical the the regular KeePass2 program. This is because Argon2 uses only managed code, even in KeePass2.

### AES KDF, possibly slower
However, for KDBX files saved with the AES KDF, the loading and saving time can be several times longer than with the regular KeePass2 program (e.g. 500% longer). This is because KeePass2 uses native libraries that are much faster than the dot net [RijndaelManaged](https://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged(v=vs.110).aspx) class. When the native libraries are not there (KeePassLibC32.dll / KeePassLibC64.dll), KeePass2 fall backs to pure managed code (RijndaelManaged). In which case the performances are identical with the dot net standard lib. 
