using System;
using System.Device.Gpio;
using System.Threading;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using Iot.Device.ServoMotor;

namespace robot_firmware
{
    class Program
    {
        static void Main(string[] args)
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

        }

        static void MoveToAngle(ServoMotor Servo, int Angle) {
            Servo.WriteAngle(Angle);            
        }
    }
}