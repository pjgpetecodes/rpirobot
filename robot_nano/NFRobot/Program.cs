using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using nanoFramework.SignalR.Client;
using System;
using System.Device.Pwm;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace NFRobot
{
    public class Program
    {
        // Set-up Esp32 Pins.
        private const int pwmPin1 = 17;
        private const int pwmPin2 = 18;
        private const int pwmPin3 = 20;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            const string Ssid = "";
            const string Password = "";
            // Give 60 seconds to the wifi join to happen
            CancellationTokenSource cs = new(60000);
            var success = WifiNetworkHelper.ConnectDhcp(Ssid, Password, requiresDateTime: true, token: cs.Token);
            if (!success)
            {
                // Something went wrong, you can get details with the ConnectionError property:
                Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                if (WifiNetworkHelper.HelperException != null)
                {
                    Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                }
            }
            else
            {
                // Otherwise, you are connected and have a valid IP and date
                Debug.WriteLine($"Connected to the network: {WifiNetworkHelper.Status}");
            }

            // Set-up the ESP32 PWM Channel on the required pin.
            Configuration.SetPinFunction(pwmPin1, DeviceFunction.PWM1);
            using PwmChannel pwmChannel1 = PwmChannel.CreateFromPin(pwmPin1, 50);
            // You can check then if it has created a valid one:
            if (pwmChannel1 is null)
                Debug.WriteLine("PWM Channel 1 failed to create...");

            using ServoMotor servoMotor1 = new ServoMotor(
                    pwmChannel1,
                    180,
                    700,
                    2400);

            // Set-up the ESP32 PWM Channel on the required pin.
            Configuration.SetPinFunction(pwmPin2, DeviceFunction.PWM3);
            using PwmChannel pwmChannel2 = PwmChannel.CreateFromPin(pwmPin2, 50);
            // You can check then if it has created a valid one:
            if (pwmChannel2 is null)
                Debug.WriteLine("PWM Channel 2 failed to create...");

            using ServoMotor servoMotor2 = new ServoMotor(
                    pwmChannel2,
                    180,
                    700,
                    2400);

            // Set-up the ESP32 PWM Channel on the required pin.
            Configuration.SetPinFunction(pwmPin3, DeviceFunction.PWM5);
            using PwmChannel pwmChannel3 = PwmChannel.CreateFromPin(pwmPin3, 50);
            // You can check then if it has created a valid one:
            if (pwmChannel3 is null)
                Debug.WriteLine("PWM Channel 3 failed to create...");

            using ServoMotor servoMotor3 = new ServoMotor(
                    pwmChannel3,
                    180,
                    700,
                    2400);

            servoMotor1.Start();
            servoMotor2.Start();
            servoMotor3.Start();

            //setup connection
            var options = new HubConnectionOptions() { Reconnect = true, Certificate = new X509Certificate(Resource.GetBytes(Resource.BinaryResources.DigiCertGlobalRootG2)) };
            HubConnection hubConnection = new HubConnection("https://bcsrobotdevuksapp.azurewebsites.net/chathub", options: options);

            hubConnection.Closed += HubConnection_Closed;

            try
            {
                hubConnection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, (sender, args) =>
                {
                    var user = args[0] as string;
                    var message = args[1] as string;

                    user = user.Trim('"');
                    message = message.Trim('"');

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

                //start connection
                hubConnection.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString());
                throw;
            }

            //
            // Keep the code running...
            //
            while (true)
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }

        private static void HubConnection_Closed(object sender, SignalrEventMessageArgs message)
        {
            Debug.WriteLine($"closed received with message: {message.Message}");
        }

        static void MoveToAngle(ServoMotor Servo, int Angle)
        {
            Servo.WriteAngle(Angle);
        }
    }
}
