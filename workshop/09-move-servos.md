# Move the Servos #


- Return to your SSH Session
- Make sure you're in the `robot_firmware` directory
- Add the IoT Device Bindings Nuget Package to your project with;

    ```
    dotnet add package Iot.Device.Bindings
    ```

- You can find more information about the IoT Device Bindings here;

    https://github.com/dotnet/iot/tree/master/src/devices

- Back in your project in VS Code
- Add the following using statements to the list of using statements;

    ```cs
    using System.Device.Pwm;
    using System.Device.Pwm.Drivers;
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

    using SoftwarePwmChannel pwmChannel3 = new SoftwarePwmChannel(27, 50, 0.5, true);
    using ServoMotor servoMotor3 = new ServoMotor(
        pwmChannel3,
        180,
        900,
        2100);

    servoMotor1.Start();
    servoMotor2.Start();
    servoMotor3.Start();

    try
    {
        while(true)
        {
            MoveToAngle(servoMotor1, 70);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor2, 20);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor3, 80);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor1, 150);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor2, 50);
            Thread.Sleep(2000);
            MoveToAngle(servoMotor3, 150);
            Thread.Sleep(2000);
            
        }
    }
    finally
    {
        servoMotor1.Stop();
        servoMotor2.Stop();
        servoMotor3.Stop();
    }
    
    ```

- Add the following sub after the Main sub;

    ```cs
    static void MoveToAngle(ServoMotor Servo, int Angle) {
        Servo.WriteAngle(Angle);            
    }
    ```

- This new code;
    - Creates two hardware `PwmChannel` objects and one `SoftwarePwmChannel` object. These are Pulse Width Modulation Channels.
    - Each Hardware PWM Channel is created passing in;
        - The Chip Number
        - The PWM Channel
        - The Frquency
        - Optionally, the Duty Cycle Percentage
    - The Software PWM Channel takes;
        - The Pin Number over which the PWM is sent
        - The Frequency
        - The Duty Cycle
        - Use a precision timer (Otherwise it's really jerky!)
    - We also then create three `ServoMotor` objects, passing in;
        - The PWM Channel we'll be using
        - The Maximum Angle of the Servo - We're using a 180 degree Servo, so we pass in 180 here
        - The Minimum Pulse Width
        - The Maximum Pulse Width
            - Both the Min and Max Pulse Widths take a certain amount of trial an error to work out.
            - These two paramaters will determine the rotation of teh servo and how far around to each end stop you can reach.
    - We then Start all three Servos.
    - Next we create a loop.
    - In the loop;
        - Move the First Servo to an angle of 50 degrees
        - Pause for 2 seconds
        - Move the Second Servo to an angle of 50 degrees
        - Pause for 2 seconds
        - Move the Third Servo to an angle of 80 degrees
        - Pause for 2 seconds
        - Move the First Servo to an angle of 100
        - Pause for 2 seconds
        - Move the Second Servo to an angle of 100
        - Pause for 2 seconds
        - Move the Third Servo to an angle of 150 degrees
        - Pause for 2 seconds
    - A section then stops the servos before the program exits.
    - Finally, a section of code which performs the servo moving
        - This sub simply takes which Servo we want to move and the Angle we'd like to move it to
        - The `Servo.WriteAngle` sub can take either an angle or a frequency here. We're using an angle. 

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