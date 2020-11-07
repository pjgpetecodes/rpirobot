using System;
using System.Threading;
using System.Device.Pwm;
using Iot.Device.ServoMotor;

namespace robot1
{
    class Program
    {

        PwmChannel pwmChannel1;
        ServoMotor servoMotor1;
        PwmChannel pwmChannel2;
        ServoMotor servoMotor2;

        static void Main(string[] args)
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

            while(true)
            {
                MoveToAngle(servoMotor1, 10);
                Thread.Sleep(1000);
                MoveToAngle(servoMotor2, 10);
                Thread.Sleep(1000);
                MoveToAngle(servoMotor1, 150);
                Thread.Sleep(1000);
                MoveToAngle(servoMotor2, 150);
                Thread.Sleep(1000);
            }

            servoMotor1.Stop();
            servoMotor2.Stop();
        }

        static void MoveToAngle(ServoMotor Servo, int Angle) {

            Servo.WriteAngle(Angle);            
        }

    }
}
