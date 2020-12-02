# Install .NET 5 #

- Open an SSH Session using PUTtY if you don't already have one open.
- Update your Pi Using;

    ```
    sudo apt-get update
    sudo apt-get upgrade
    ```

- Install .NET 5 by running the following command;

    ```
    wget -O - https://raw.githubusercontent.com/pjgpetecodes/dotnet5pi/master/install.sh | sudo bash
    ```

<p align="center">
    <img src="images/03-install-dot-net-5.gif" width="500px" >
</p>

| Previous | Next |
| -------- | ---- |
| [< Step 2 - Enable SSH](/02-create-samba-share.md) | [Step 4 - Create Pi Robot Project >](04-create-pi-robot-project.md) |