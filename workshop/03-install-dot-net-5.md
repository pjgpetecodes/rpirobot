# Install .NET 5 #

- On your local machine, if you don't already have it, download and install .NET 5 from;

    https://dotnet.microsoft.com/download/dotnet/5.0
    
- Download and install Visual Studio Code if you don't already have it;

    https://code.visualstudio.com/

- On the "Select Additional Tasks" page of the VS Code installer, make sure to check all of the checkboxes.

- Open an SSH Session to your Raspberry pi using PUTtY if you don't already have one open.
- Update your Pi Using;

    ```
    sudo apt-get update
    sudo apt-get upgrade
    ```

- Install .NET 5 by running the following command;

    ```
    wget -O - https://raw.githubusercontent.com/pjgpetecodes/dotnet5pi/master/install.sh | sudo bash
    ```

- Reboot your pi

    ```
    sudo reboot
    ```

<p align="center">
    <img src="images/03-install-dot-net-5.gif" width="500px" >
</p>

| Previous | Next |
| -------- | ---- |
| [< Step 2 - Create Samba Share](02-create-samba-share.md) | [Step 4 - Create Pi Robot Project >](04-create-pi-robot-project.md) |
