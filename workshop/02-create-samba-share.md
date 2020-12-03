# Pre-Requisites! #

## Step 2 - Create Samba Share ##

- Connect to the Raspberry Pi using SSH (PUTtY for instance).
- Install Samba;

    ```
    sudo apt-get install samba samba-common-bin
    ```

- Make a shared directory

    ```
    sudo mkdir -m 1777 /home/pi/share
    ```

- Add the following to the bottom of the file and save;

    ```    
    [share]
    Comment = Pi shared folder
    Path = /home/pi/share
    Browseable = yes
    Writeable = Yes
    only guest = no
    create mask = 0777
    directory mask = 0777
    Public = yes
    Guest ok = yes
    ```

- Create a Samba Password (you can use the same password as logging in to your pi for now);

    ```
    sudo smbpasswd -a pi
    ```

- Restart the Pi;

    ```
    sudo reboot
    ```
- Reconnect to the Raspberry Pi using SSH (PUTtY for instance).
- Browse to the new `share` directory on the Pi at `\\<Raspberry Pi Hostname>\share`

| Previous | Next |
| -------- | ---- |
| [< Step 1 - Enable SSH](01-enable-ssh.md) | [Step 3 - Install .NET 5 >](03-install-dot-net-5.md) |