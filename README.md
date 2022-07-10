# ARCHBLOXBootstrapper
A installer for ARCHBLOX. Game Launcher: https://github.com/Thomasluigi07/ARCHBLOXLauncher
## How this works
- First, it checks if there's the latest version installed already. If it is, it deletes the latest version and re-installs it.
- Then it gets the current version string from https://archblox.com/client/version.txt and then makes a folder in C:\ARCHBLOX\ with that text.
- Then it downloads the client .zip at https://archblox.com/client/(versionstring).zip and unzips it once the download is done.
I'm new to C# so I hope this will be good. My code is pretty oof.
