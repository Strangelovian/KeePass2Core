# KeePass2Core
Attempt to refactor Keepass2 lib into dot net core.
For now this repo contains two sln files:
- KeePass.sln: same as the original KeePass2 solution, with key signing removed for convenience
- NetStd.sln:
  - KeePassLibStd.csproj: builds a read / write capable dot net standard version of the KeePass lib
  - Kdbx2xml: command line tool to demonstrate export of Kdbx to unencrypted Xml
