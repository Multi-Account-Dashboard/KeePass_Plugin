# Keepass-Plugin

Readme for the Multi-Account-Dashboard (short MAD) plugin for the Keepass password manager.

_____________________________________________________________________________________________________________________________________________________

1. If you want to test the plugin, refer to the following steps:

- Download the Keepass-X.XX-Setup.exe from https://keepass.info/download.html. Note that during development the newest version available was 2.49.

- Follow the provided instructions to install Keepass on your machine.

- Copy the two .dll files (MAD_Plugin.dll and Newtonsoft.Json.dll) provided with the plugin sourcecode into the "Plugins"-folder. It's found in the Keepass folder that is created during the installation process.

- Copy the "ProvidersDatabase.json"-file into the Keepass folder (where Keepass.exe is) and not into the plugin folder.

- Start Keepass, create a new masterpassword and set up a new database

- In the main window, click on "Tools" in the top menu bar. The last item in the dropdown list should be called "MAD_Plugin". Click on it and select "Open MAD" and you're good to go.

_____________________________________________________________________________________________________________________________________________________

2. If you want to have a look at the source code and build the project yourself, follow these steps:

- Download the Keepass-X.XX-Setup.exe from https://keepass.info/download.html. Note that during development the newest version available was 2.49.

- Open the solution file (.sln) in a compatible IDE, i.e. Visual Studio 2019.

- You may need to manually add outside references to the project. In Visual Studio this can be done via "Project"=>"Add Reference" or directly in Code

  - If you need to add references to KeePass libraries, navigate to "Project"=>"Add Reference"=>"Browse" and then select the KeePass.exe file found in the folder you downloaded in the beginning.

- You should now be able to build the project. By doing so you genereate a new "MAD_Plugin.dll" file in the "bin/debug" folder of the project that can be used as described in 1. to run the plugin.

