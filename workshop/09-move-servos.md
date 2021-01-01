# Move the Servos #


- Return to your SSH Session
- Make sure you're in the `robot_firmware` directory
- Add the IoT Device Bindings Nuget Package to your project with;

    ```
    dotnet add package Iot.Device.Bindings
    ```

- You can find more information about the IoT Device Bindings here;

    https://github.com/dotnet/iot/tree/master/src/devices
    
- Enabling Hardware PWM on Raspberry Pi
    Edit the /boot/config.txt file and add the dtoverlay line in the file. You need root privileges for this:
    
    ```
    sudo nano /boot/config.txt
    ```
    | PWM | GPIO | Function | Alt | dtoverlay |
| --- | --- | --- | --- | --- |
| PWM0 | 12 | 4 | Alt0 | dtoverlay=pwm,pin=12,func=4 |
| PWM0 | 18 | 2 | Alt5 | dtoverlay=pwm,pin=18,func=2 |
| PWM1 | 13 | 4 | Alt0 | dtoverlay=pwm,pin=13,func=4 |
| PWM1 | 19 | 2 | Alt5 | dtoverlay=pwm,pin=19,func=2 |
Save the file with `ctrl + x` then `Y` then `enter`

Then reboot:

```bash
sudo reboot
```

You are all setup, the basic example should now work with the PWM and channel you have selected.
   
- Back in your project in VS Code
- Add the following using statements to the list of using statements;

    ```cs
    using System.Device.Pwm;
    using Iot.Device.ServoMotor;
    ```

- Here we've added usings for two nuget libraries, PWM and also ServoMotor. You can find more information and examples here;

    https://github.com/dotnet/iot/tree/master/src/devices/ServoMotor

- Remove the existing code you have added in the `static void main` sub and add in the following code after the `Console.WriteLine("Hello World")` Line;

    ```cs
    using PwmChannel pwmChannel1 = PwmChannel.Create(0, 0, 50);
    using ServoMotor servoMotor1 = new ServoMotor(
        pwmChannel1,
        180,
        700,
        2400);

    using PwmChannel pwmChannel2 = PwmChannel.Create(0, 1, 50);
    using ServoMotor servoMotor2 = new ServoMotor(
        pwmChannel2,
        180,
        700,
        2400);

    servoMotor1.Start();
    servoMotor2.Start();

    try
    {
        while(true)
        {
            MoveToAngle(servoMotor1, 70);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor2, 20);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor1, 150);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor2, 50);
            Thread.Sleep(2000);
        }
    }
    finally
    {
        servoMotor1.Stop();
        servoMotor2.Stop();
    }
    
    ```

- This section of code;
    - Create two `PwmCHannel` objects. These are Pulse Width Modulation Channels, one per Servo Motor we'll be driving.
    - Each PWM Channel is created passing in;
        - The Chip Number
        - The PWM Channel
        - The Frquency
        - Optionally, the Duty Cycle Percentage
    - We also then create two `ServoMotor` objects, passing in;
        - The PWM Channel we'll be using
        - The Maximum Angle of the Servo - We're using a 180 degree Servo, so we pass in 180 here
        - The Minimum Pulse Width
        - The Maximum Pulse Width
            - Both the Min and Max Pulse Widths take a certain amount of trial an error to work out.
            - These two paramaters will determine the rotation of the servo and how far around to each end stop you can reach.
    - We then Start both Servos.
    - Next we create a loop.
    - In the loop;
        - Move the First Servo to an angle of 50 degrees
        - Pause for 2 seconds
        - Move the Second Servo to an angle of 50 degrees
        - Pause for 2 seconds
        - Move the First Servo to an angle of 100
        - Pause for 2 seconds
        - Move the Second Servo to an angle of 100
    - A finally section then stops the servos before the program exits.

- Add the following sub after the Main sub;

    ```cs
    static void MoveToAngle(ServoMotor Servo, int Angle) {
        Servo.WriteAngle(Angle);            
    }
    ```

- This sub simply takes which Servo we want to move and the Angle we'd like to move it to
    - The `Servo.WriteAngle` sub can take either an angle or a fequency here. We're using an angle. 
- Save the file
- Run the application with;

    ```
    dotnet run
    ```

- Your Servos should now start moving!

- Exit the program with ctrl+c

| Previous | Next |
| -------- | ---- |
| [< Step 8 - Build the Circuit - Servos](08-build-circuit-servos.md) | [Step 10 - Add SignalR >](10-add-signalr.md) |
