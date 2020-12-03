# Move the Servos #


- Return to your SSH Session
- Make sure you're in the `pirobot` directory
- Add the SignalR Client Nuget Package to your project with;

    ```
    dotnet add package Microsoft.AspNetCore.SignalR.Client
    ```

- Back in your project in VS Code
- Add the following using statements to the list of using statements;

    ```cs
    using Microsoft.AspNetCore.SignalR.Client;
    using System.Net.Http;
    ```

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

    connection = new HubConnectionBuilder()
                    .WithUrl("https://<PC IP Address>:5001/chathub",conf =>
                    {
                        conf.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        };                    
                    })
                    .Build();
                
    try
    {
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {

            if (user == "servo1")
            {
                MoveToAngle(servoMotor1, Int32.Parse(message));
            }
            else if (user == "servo2")
            {
                MoveToAngle(servoMotor2, Int32.Parse(message));
            }
            else if (user == "servo3")
            {
                MoveToAngle(servoMotor3, Int32.Parse(message));
            }
            Console.WriteLine($"{message} posted by: {user}");
        });
        
        await connection.StartAsync();
    }
    catch (System.Exception)
    {
        
        throw;
    }

    try
    {
        while(true)
        {
            
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
            - These two paramaters will determine the rotation of teh servo and how far around to each end stop you can reach.
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
| [< Step 6 - Flash LED](05-build-circuit-led-and-button.md) | [Step 7 - Read Button >](07-read-button.md) |