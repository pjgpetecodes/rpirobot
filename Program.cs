using System;
using System.Threading;
using System.Threading.Tasks;
using System.Device.Pwm;
using Iot.Device.ServoMotor;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace robot1
{
    class Program
    {

        PwmChannel pwmChannel1;
        ServoMotor servoMotor1;
        PwmChannel pwmChannel2;
        ServoMotor servoMotor2;

        private static HubConnection connection;
            
        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            using PwmChannel pwmChannel1 = PwmChannel.Create(0, 0, 50);
            using ServoMotor servoMotor1 = new ServoMotor(
                pwmChannel1,
                160,
                700,
                2200);

            using PwmChannel pwmChannel2 = PwmChannel.Create(0, 1, 50);
            using ServoMotor servoMotor2 = new ServoMotor(
                pwmChannel2,
                160,
                700,
                2200);

            servoMotor1.Start();
            servoMotor2.Start();

            connection = new HubConnectionBuilder()
                .WithUrl("https://192.168.1.162:5001/chathub",conf =>
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
           

            
            while(true)
            {
                
            }

            servoMotor1.Stop();
            servoMotor2.Stop();
        }      

        static void MoveToAngle(ServoMotor Servo, int Angle) {

            Servo.WriteAngle(Angle);            
        }

    }
}
