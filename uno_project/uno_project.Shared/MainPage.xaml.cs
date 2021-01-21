using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Device.Gpio;
using System.Threading;
using System.Device.Pwm;
using Iot.Device.ServoMotor;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uno_project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        PwmChannel pwmChannel1;
        ServoMotor servoMotor1;
        PwmChannel pwmChannel2;
        ServoMotor servoMotor2;

        public MainPage()
        {
            this.InitializeComponent();

            pwmChannel1 = PwmChannel.Create(0, 0, 50);
            servoMotor1 = new ServoMotor(
                pwmChannel1,
                180,
                700,
                2400);

            pwmChannel2 = PwmChannel.Create(0, 1, 50);
            servoMotor2 = new ServoMotor(
                pwmChannel2,
                180,
                700,
                2400);

            servoMotor1.Start();
            servoMotor2.Start();


        }

        private void servo1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Console.WriteLine(e.NewValue);
            MoveToAngle(servoMotor1, Convert.ToInt32(e.NewValue));
        }

        private void servo2_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Console.WriteLine(e.NewValue);
            MoveToAngle(servoMotor2, Convert.ToInt32(e.NewValue));
        }

        private void servo3_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Console.WriteLine(e.NewValue);
        }

        static void MoveToAngle(ServoMotor Servo, int Angle) {
            Servo.WriteAngle(Angle);            
        }
    }
}
