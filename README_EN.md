# JSB Ultimate Multiplayer Server

This is a project to launch a server for JSB Ultimate (JSB Port) for multiplayer gameplay.

## Contents

- [Installation](#installation)
  - [Android](#android)
  - [PC](#pc)
- [Starting the Server](#starting-the-server)
- [Configuring the Game Config](#configuring-the-game-config)
- [Connecting to the Server via a Local Server](#connecting-to-the-server-via-a-local-server)

## Installation

### Android

1. Download and install the Termux app from [Google Play](https://play.google.com/store/apps/details?id=com.termux) or [F-Droid](https://f-droid.org/packages/com.termux/).
2. Install the necessary packages in Termux by running the following commands:

   ```bash
   pkg update && pkg upgrade
   pkg install mono
   pkg install curl
   ```

3. Now you have a choice: either download the server files directly onto your device, or download the zip file directly in Termux using the `curl` command.

   - **Downloading to the Device:**
     - Download the zip file from the latest release containing the server files (not the source code!) and extract all files into a directory of your choice.
     - In Termux, run the following command (to grant access to device files):

       ```bash
       termux-setup-storage
       ```

   - **Downloading in Termux:**
     - Install the `curl` and `unzip` packages, then download and extract the archive (in the third command, replace `{VERSION}` with the latest version):

       ```bash
       pkg install unzip
       curl -L -o JSBUMultiplayerServer_dotnet31.zip https://github.com/SedTriHEX/JSBUMultiplayerServer/releases/download/{VERSION}/JSBUMultiplayerServer_dotnet31.zip
       mkdir ~/jsbuserver
       cd ~/jsbuserver
       unzip ~/JSBUMultiplayerServer_dotnet31.zip
       ```

### PC

1. Download and install .NET Core 3.1 from the [official Microsoft website](https://dotnet.microsoft.com/en-us/download/dotnet/3.1).
2. Download the zip file from the latest release (not the source code!) and extract all files into a directory of your choice.

## Starting the Server

- **Windows:** Simply run `JustShapesBeatsMultiplayerServer.exe`.
- **Linux:** Launch the server via the terminal with the command:

  ```bash
  dotnet JustShapesBeatsMultiplayerServer.dll
  ```

- **Android:** Start the server through Termux with the command:

  ```bash
  mono {full path to JustShapesBeatsMultiplayerServer.dll}
  ```

  - If you chose to download directly in Termux, the path will be:

    ```bash
    mono ~/jsbuserver/JustShapesBeatsMultiplayerServer.dll
    ```

  - If you chose to download to the device, the path will be:

    ```bash
    mono ~/storage/shared/{path to folder}/JustShapesBeatsMultiplayerServer.dll
    ```

## Configuring the Game Config

- **Android:** The config file is located at `/android/data/com.RayNick.JSABMobile/files/config.txt`.
- **Windows:** The config file is located in `game folder/JSB Ultimate_Data/gameData/config.txt`.

By default, multiplayer is disabled. To enable multiplayer, set the value of `enable-custom-levels` to `0`:

```plaintext
enable-custom-levels=0
```

Also by default, the IP for multiplayer is set to `127.0.0.1` and the port to `25545`. If you need to change the IP or port, set the values:

```plaintext
multiplayer-ip-address={IP}
multiplayer-port={Port}
```

## Connecting to the Server via a Local Server

- **For the Host:** Usually nothing needs to be changed. Simply start the server and enter the game.
- **For Android:**
  - Enable the hotspot and have your friends connect to it.
  - In Termux, while the server is running, execute the command `gameip`. The server will display all available IP addresses.
  - Choose one from the list and enter it into `config.txt` as the value for `multiplayer-ip-address`.
  - If the connection fails, try another IP from the list.
- **For Windows:**
  - If the server is running on Windows and you want to play via Android, disable the Windows firewall so that the connection to the server is allowed.
  - The IP will also need to be changed in `config.txt`, which can be determined by running the command `gameip`.
