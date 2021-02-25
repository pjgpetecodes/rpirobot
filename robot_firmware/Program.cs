﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using Iot.Device.ServoMotor;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace robot_firmware
{
    class Program
    {

        private static HubConnection connection;
            
        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello World!");

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

            connection = new HubConnectionBuilder()
                .WithUrl("https://192.168.1.162:5001/chathub",conf =>
                {
                    conf.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    };                    
                })
		.WithAutomaticReconnect()                
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
           

            //
            // Keep the code running...
            //
            while(true)
            {
                
            }

            servoMotor1.Stop();
            servoMotor2.Stop();
            servoMotor3.Stop();
        }      

        static void MoveToAngle(ServoMotor Servo, int Angle) {

            Servo.WriteAngle(Angle);            
        }

    }
}
