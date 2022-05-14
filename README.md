# Blizzard Client Installer

The Blizzard Client Installer is a version of the Blizzard Updater that can install the full client, an expansion, or a minor update. It is a lot more heavyweight than other versions of the installer but has a lot more functionality.

The UI is driven through XML files that define that layout. All images and audio are included. Dialog forms are embedded in the executable file. You can set and change the registry values it uses, the start menu and program data it creates, etc.

Using this I have created a ~12GB installer that can install a custom 3.3.5 WOTLK client. The reason my client is smaller is because the Blizzlike client actually has a lot of duplicated files stored in the MPQ archives because old versions of the files exist in some MPQs with a lower load priority.

![installer1](https://i.imgur.com/T44lxdv.png)
![installer2](https://i.imgur.com/ZmiKADz.png)
![installer3](https://i.imgur.com/yKhc4M4.png)
![installer4](https://i.imgur.com/WOd4P8Q.png)
![installer5](https://i.imgur.com/LL1MzEW.png)

## How I created my installer

There is a lot of functionality that the installer provides, I cannot document it all. Instead I document how I went about creating the installer for my version of the client.

I downloaded the original Blizzlike installers (not the new battle.net versions) for the vanilla client and for the WOTLK installation. After reverse engineering both of these installers I determined they both worked very similarly but the WOTLK version had more functionality. As such, I opted to run with the WOTLK installer.

You can find the original WOTLK installer in this repository, in the `WOTLK_Installer` folder. It does not contain any of the client data, only the installer data.

The `Installer.exe` reads `Installer Tome.mpq` for all the configuration data. I have unpacked that MPQ in this repository.

`Installer Tome 1..*.mpq` holds all the files that will be installed into the client, but these MPQs can be called anything. For example, the WOTLK installer actually puts the cinematic files in a `Movies.mpq`.

### Installer Tome.mpq

Inside the `Installer Tome.mpq` is all the configuration data that the installer uses.

| File       | Description |
|------------|-------------|
| InstallCD\InstallerFileList\InstallerFileList.xml | This is the install instructions for the entire installer and can be over 200,000 lines long in a full client install. |
| InstallCD\ProductDefs.xml | Some misc data, defines the primary locale for the installer. |
| InstallCD\WLK\Global\Unpack | Contains most of the images, audio, and UI data. The XML files are like HTML files, but it is all proprietary. |
| InstallCD\WLK\enUS\Unpack | Contains locale specific UI data. |
| InstallCD\WLK\enUS\Unpack\ProductData.xml | This locale specific file contains a bunch of installer config data. This one is important to edit. |
| InstallCD\WLK\enUS\Unpack\Localization.xml | This locale specific file contains most of the strings that the installer uses. |
| InstallCD\WLK\enUS\EULA | The EULA text. |
| InstallCD\WLK\enUS\ExternalLinks\Support | The locale specific manual and read me that the Installer displays. |

#### InstallerFileList.xml

< To do - how to generate and edit the instructions. >

### Disabling the create account popup.

When the installation completes it pops up with a dialog asking if you want to create or upgrade an account:

![dialogpopup](https://i.imgur.com/4l1DClJ.png)

These dialogs are embedded in the `Installer.exe` and can be edited with an appropiate tool. I used Resource Tuner, and just deleted this popup altogether:

![deletepopup](https://i.imgur.com/i3vFbL9.png)

### Registry Settings

By default, the Installer will detect if WoW has already been installed by reading the registry settings (configurable in installer data). Depending on your use case you could leave this alone, or change it to use a custom path, or disable the registry check altogether. I opted for the last option, and now it will always let you install a client even if it is already installed.

To achieve this I needed to edit the layout XML files to disable the registry check. I also needed to disable a check that tries to find and download a more up to date installer.

