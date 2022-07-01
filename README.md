# Sako-Inverter-Monitor


sudo nano /lib/udev/rules.d/99-systemd.rules

```
KERNEL=="hidraw0", SYMLINK="hidraw0", TAG+="systemd"
```

sudo nano /etc/systemd/system/invertermon.service

```
[Unit]
Description=Hybrid Inverter Monitor
After=dev-hidraw0.device

[Service]
Type=simple
User=root
Group=root
UMask=000

WorkingDirectory=/home/djnitehawk/inverter
ExecStart=/home/djnitehawk/inverter/InverterMon.Server

Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

sudo systemctl enable invertermon
sudo systemctl start invertermon
sudo systemctl status invertermon