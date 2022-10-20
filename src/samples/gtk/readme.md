# Meadow GTK Sample

## WSL2

- Ensure .NET is installed

```
$ wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
$ sudo dpkg -i packages-microsoft-prod.deb
$ rm packages-microsoft-prod.deb
$ sudo apt update; \
    sudo apt install -y apt-transport-https && \
    sudo apt update && \
    sudo apt install -y dotnet-sdk-6.0
```

sudo apt install - y gtk-sharp3-examples

