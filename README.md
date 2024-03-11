<p align="center">
  <img src="https://i.imgur.com/t0fYl26.png" alt="GameMaker Companion Logo" width="65%"/>
</p>

**GameMaker Companion** is a cross-platform companion application and a Discord Rich Presence module for GameMaker series of game engines. It is a small application that can run in background to provide features useful to a GameMaker developer and supply a profile currently logged in Discord application with information specific to GameMaker while it is running:

![Rich Presence Status Preview](https://i.imgur.com/KXYWn3H.png)

It is a standalone application and does not interact with GameMaker itself. Information it gathers comes from the process list and window title of GameMaker IDE supplied by the operating system.

Made using [.NET](https://dotnet.microsoft.com/en-us/) 8.0. and [Avalonia UI](https://github.com/AvaloniaUI/Avalonia), programmed solely in C#.

# Features
● Supported on Windows, macOS and Linux.

● Communicates with Discord API to supply the profile with Rich Presence:    
&nbsp;&nbsp;&nbsp;• Displays current GameMaker session time and names of opened projects or the number of them, depending on configuration.    
&nbsp;&nbsp;&nbsp;• Supports legacy and beta versions of GameMaker series of game engines.    
&nbsp;&nbsp;&nbsp;• Includes information about whether the GameMaker IDE is currently executing its application. Applicable only when it has only one project is open.

● Creates an icon in system tray to provide configuration and additional tools:    
&nbsp;&nbsp;&nbsp;• Configurable to start automatically when current user is logged into the operating system and to remove the icon from system tray.    
&nbsp;&nbsp;&nbsp;• Provides a menu with shortcuts to main pages on GameMaker websites.    
&nbsp;&nbsp;&nbsp;• Creates a notification displaying the total uptime time of GameMaker while this application was running.    
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Method of requesting the display is subject to cross-platform differences:    
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;→ Windows: Single left-clicking on the icon in system tray.    
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;→ macOS: Single-clicking an entry with subsidiary entries in system tray icon menu.    
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;→ Linux: Double left-clicking on the icon in system tray.

# Initialization
**GameMaker Companion** is supported on Windows 10, macOS 12 Monterey, Linux Ubuntu 20.04 and later versions of these operating systems. While it may be able to launch on other systems of these platforms, they are not explicitly supported and some features might not work in effect of that. Also note that:    
&nbsp;&nbsp;• This application is designed for default setups and default interface shells of above operating systems.    
&nbsp;&nbsp;• Following permissions are required within the executing operating system for this application to work properly: File access, Internet connection, access to process information (part of "Screen Recording" permission in macOS), autostart and notification creation.    
&nbsp;&nbsp;• While releases for processor architectures other than x64 are available, they are experimental and not explicitly supported.

Initialization is performed by following steps specific to each platform.

### Windows
1. Head to the [releases tab](https://github.com/Mtax-Development/GameMakerCompanion/releases) and download a `.7z` archive file with the release for processor architecture of the machine meant to execute **GameMaker Companion**.     
&nbsp;&nbsp;• Attempting to launch an application for incorrect processor architecture will result in an error message created by the operating system. If that happens: Download the `.7z` file for the other architecture.
2. Extract the downloaded archive by right-clicking it and selecting "Extract All".
3. Install [.NET 8.0 Runtime](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net80), necessary to launch this application, by following instructions in the link.
4. Launch `GameMaker Companion.exe` from the location it was extracted to. The application should create an icon in system tray and Discord Rich Presence should be set when Discord and GameMaker IDE applications are running.    
&nbsp;&nbsp;• In case of errors that do not produce explanation prompts: Execute the `GameMaker Companion.exe` file through Command Line and proceed by error messages output in it.

### macOS
1. Head to the [releases tab](https://github.com/Mtax-Development/GameMakerCompanion/releases) and download a `.dmg` image file with the release for processor architecture of the machine meant to execute **GameMaker Companion**.     
&nbsp;&nbsp;• Attempting to launch an application for incorrect processor architecture will result in an error message created by the operating system and the application icon will be crossed out. If that happens: Download the `.dmg` file for the other architecture.
2. Open the downloaded `.dmg` file by double-clicking it. It will display an icon for **GameMaker Companion** Application Bundle and a shortcut to the "Applications" folder. Extract **GameMaker Companion** Application Bundle by clicking and holding its icon to drag it to the "Applications" folder.    
&nbsp;&nbsp;• While **GameMaker Companion** can launch from folders other than "Applications", doing so can cause problems on some versions of macOS and should be avoided.
3. Install [.NET 8.0 Runtime](https://learn.microsoft.com/en-us/dotnet/core/install/macos?tabs=net80), necessary to launch this application, by following instructions in the link.
4. Launch the `GameMaker Companion` Application Bundle from the location it was extracted to. The application should create an icon in system tray and Discord Rich Presence should be set when Discord and GameMaker IDE applications are running.    
&nbsp;&nbsp;• If the operating system creates a prompt about it preventing execution of any file contained within the Application Bundle: Close that prompt, then go to "System Settings" → "Privacy & Security" and find the message stating that file was blocked, then click the "Open Anyway" button under it. Enter account password and confirm the file should be opened whenever prompted by the system. Then launch the Application Bundle again. This process might need to be repeated for several files on some versions of macOS.    
&nbsp;&nbsp;• If a prompt is displayed about permissions for Screen Recording: Go to "System Settings" → "Privacy & Security" → "Screen Recording". Grant the permission to **GameMaker Companion**. If **GameMaker Companion** is being executed through Terminal, permission should be granted for Terminal instead.    
&nbsp;&nbsp;• If the `GameMaker Companion` file is not executable or executes as text file: Execute the following command in Terminal: `chmod +x <path>`, where `<path>` is the path of the `GameMaker Companion` file. That file can be dragged into the Terminal to fill its path automatically.    
&nbsp;&nbsp;• In case of errors that do not produce explanation prompts: Execute the `GameMaker Companion` file through Terminal and proceed by error messages output in it.

### Linux Ubuntu
1. Head to the [releases tab](https://github.com/Mtax-Development/GameMakerCompanion/releases) and download a `.tar.gz` archive file with the release for processor architecture of the machine meant to execute **GameMaker Companion**.    
&nbsp;&nbsp;• Attempting to launch an application for incorrect processor architecture will not result in an error message unless the application is executed through the Terminal. After confirming that happened: Download the `.tar.gz` file for other architecture.
2. Extract the downloaded archive by right-clicking it and selecting either "Extract Here" or "Extract to..." and choosing the desired directory.
3. Install [.NET 8.0 Runtime](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu?tabs=net80#register-the-microsoft-package-repository), necessary to launch this application, by following instructions in the link.
4. Install [`xdotool`](https://github.com/jordansissel/xdotool), necessary for this application to access window titles, by executing the following command in Terminal: `sudo apt-get install xdotool`
5. Launch the `GameMaker Companion` file from the location it was extracted to. The application should create an icon in system tray and Discord Rich Presence should be set when Discord and GameMaker IDE applications are running.    
&nbsp;&nbsp;• If the `GameMaker Companion` file is not executable: Execute the following command in Terminal: `chmod +x <path>`, where `<path>` is the path of the `GameMaker Companion` file. That file can be dragged into the Terminal to fill its path automatically.    
&nbsp;&nbsp;• If that file is not executable despite using `chmod +x` on it: Execute it through Terminal. Autostart can be then set up for this application to launch automatically without use of Terminal.    
&nbsp;&nbsp;• In case of errors that do not produce explanation prompts: Execute the `GameMaker Companion` file through Terminal and proceed by error messages output in it.

# Credits
Created, documented and maintained by [Mtax](https://github.com/Mtax-Development).

Special thanks to creators and maintainers of dependencies used in this project:    
&nbsp;&nbsp;● [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp)    
&nbsp;&nbsp;● [Avalonia UI](https://github.com/AvaloniaUI/Avalonia)    
&nbsp;&nbsp;● [xdotool](https://github.com/jordansissel/xdotool)

Special thanks to [lithiumtoast](https://github.com/lithiumtoast) for help in making the macOS port possible and for creating an [example repository](https://github.com/lithiumtoast/pinvoke-macos-CGWindowListCreate) from which parts of code contained in this project were adapted.

GameMaker series of software, their logos and web resources are properties of YoYo Games.    
Discord is a property of Discord Inc.    
This is a free and open-source third-party project, not affiliated with either of them.    
This application performs connections to Discord servers via their API, solely to facilitate Discord Rich Presence.

**GameMaker Companion** was previously named **GMS2_RPC**.
