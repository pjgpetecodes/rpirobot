# Add SignalR to Robot #

- Return to your SSH Session
- Make sure you're in the `robot_firmware` directory
- Add the SignalR Client Nuget Package to your project with;

    ```
    dotnet add package Microsoft.AspNetCore.SignalR.Client
    ```

- Back in your project in VS Code
- Add the following using statements to the list of using statements;

    ```cs
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR.Client;
    using System.Net.Http;
    ```

- Add the following variable declaration above the `static void Main` sub which will house our SignalR Hub Connection;

    ```cs
    private static HubConnection connection;
    ```

- Change the `static void Main` declaration to be async, by replacing that line with;

    ```cs
    static async Task Main(string[] args)
    ```

- Remove the existing code you have added in the `static async Task Main` sub and add in the following code after the `Console.WriteLine("Hello World")` Line;

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
    - As before, creates the PWM and Servo Objects and initialises them with the same settings as previously an starts the Servos.
    - Next we create a new SignalR HubConnectionBuilder
    - Using the WithUrl extension method, we pass in to the HubConnectionBuilder, the URL of the SignalR service we'll be connecting to.
        - We'll be returning to this later to populate the URL, once we've create it in our Blazor App.
    - We also pass in a Configuration object to the HubConnectionBuilder. This configuration allows us ignore any certificate errors. I found that I wasn't able to connect without this.
    - Next we hook up an event handler to the `connection.On` `ReceiveMessage` event;
        - This event takes any number of parameters. We'll be sending a "User" and a "Message" from the Blazor App.
    - We use the `user` parameter to determine which servo we want to control.
    - We use the `message` parameter to set the angle we'd like to move the servo to.
    - We then start the SignalR connection.
    - Next, we simply loop forever, meaning we can handle the `connection.On` event whenever it occurrs.
    - Finally, we stop the servos at the end of the application.

- Exit the program with ctrl+c

| Previous | Next |
| -------- | ---- |
| [< Step 9 - Move Servos](09-move-servos.md) | [Step 11 - Create Blazor App >](11-create-blazor-app.md) |
