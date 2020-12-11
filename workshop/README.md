# Raspberry Pi Robot Workshop! #

## Introduction

Welcome to this workshop on creating a Robot Arm with Raspberry Pi, .NET 5, Blazor and SignalR!

In this workshop I'll take you through everything you need to build a robot arm with a Raspberry Pi, .NET 5 a Blazor App and SignalR.

We'll begin by installing .NET 5, then we'll connect up the various components to our Raspberry Pi.

Next we'll connect up some LEDs and Resistors to a breadboard, and  spin up a console application to explore how we can control the GPIO on the Pi.

We'll then wire up some Servos and get the code in place to start moving our Raspberry pi based Robot Arm.

Finally we'll build out a simple Blazor and SignalR app to control our robot remotely!

## Contents

| Step | Link |
| -------- | ---- |
| 01 | [Enable SSH](01-enable-ssh.md) | 
| 02 | [Create a Samba Share](02-create-samba-share.md) | 
| 03 | [Install .NET 5](03-install-dot-net-5.md) | 
| 04 | [Create Pi Robot Project](04-create-pi-robot-project.md) | 
| 05 | [Build the Circuit - LED and Button](05-build-circuit-led-and-button.md) | 
| 06 | [Flash LED](06-flash-led.md) | 
| 07 | [Read a Button](07-read-button.md) | 
| 08 | [Build the Circuit - Servos](08-build-circuit-servos.md) | 
| 09 | [Servo Demo](09-move-servos.md) | 
| 10 | [Add SignalR](10-add-signalr.md) | 
| 11 | [Create Blazor App](11-create-blazor-app.md) | 
| 12 | [Add Blazor Controls](12-add-blazor-controls.md) | 
| 13 | [Add Control Code](13-add-control-code.md) | 
| 14 | [Configure and Run](14-configure-and-run.md) | 

## Shopping List

For this workshop you'll need to buy some components before hand. The following is a list for guidance, but may be expanded on before the event!;

- Raspberry Pi (Preferably a 4Gb Pi 4 or 400).
- SD Card with Raspbian Installed and setup.

    https://thepihut.com/products/raspberry-pi-starter-kit?variant=20336446079038
    https://thepihut.com/products/raspberry-pi-400-personal-computer-kit

- MonkMakes Servo Kit;

    https://thepihut.com/products/servo-kit-for-raspberry-pi

- An extra Servo;

    https://thepihut.com/products/towerpro-servo-motor-sg90-analog

- 4 x AA Batteries;

    https://thepihut.com/products/alkaline-aa-batteries-lr6-4-pack

- Some extra jumper wires

    https://thepihut.com/products/premium-female-male-extension-jumper-wires-20-x-6?variant=27739700625 

- Some LEDs;

    https://thepihut.com/collections/adafruit-leds/products/diffused-5mm-led-pack-5-leds-each-in-5-colors-25-pack

- Some Resistors;

    https://thepihut.com/products/through-hole-resistors-220-ohm-5-1-4w-pack-of-25

- Some Buttons;

    https://thepihut.com/products/tactile-button-switch-6mm-x-20-pack?_pos=42&_sid=7091f62fe&_ss=r

- A breadboard;

    https://thepihut.com/products/raspberry-pi-breadboard-half-size

- Some Cardboard, sellotape and a gluegun if you want to build the components into a functioning Robot Arm!

- Or, you could 3d Print this...

    https://www.thingiverse.com/thing:1015238

This workshop will appeal to all knowledge levels. A working knowledge of programming will help, but all the code will be shared on GitHub!

## Let's get started!

Get started below;

[Step 1 - Enable SSH >](01-enable-ssh.md)
