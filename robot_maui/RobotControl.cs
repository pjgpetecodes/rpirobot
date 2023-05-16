using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace robot_maui
{
    internal class RobotControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static HubConnection hubConnection;

        private double _servo1 = 90;
        private double _servo2 = 90;
        private double _servo3 = 135;
       
        private bool _servo1_enabled = true;
        private bool _servo2_enabled = true;
        private bool _servo3_enabled = true;

        private IDispatcherTimer disableServo1Timer;
        private IDispatcherTimer disableServo2Timer;
        private IDispatcherTimer disableServo3Timer;

        public double servo1
        {
            get => _servo1;
            set
            {
                _servo1 = value;
                OnPropertyChanged("servo1");

                var servo1Int = (int)_servo1;

                Console.WriteLine($"Rotation Servo Now: {servo1Int}");

                if (Servo1_Enabled == true)
                {
                    hubConnection.SendAsync("SendMessage", "servo1", servo1Int.ToString());
                }

            }
        }

        public double servo2
        {
            get => _servo2;
            set
            {
                _servo2 = value;
                OnPropertyChanged("servo2");

                var servo2Int = (int)_servo2;

                Console.WriteLine($"Rotation Servo Now: {servo2Int}");

                if (Servo2_Enabled == true)
                {
                    hubConnection.SendAsync("SendMessage", "servo2", servo2Int.ToString());
                }
            }
        }

        public double servo3
        {
            get => _servo3;
            set
            {
                _servo3 = value;
                OnPropertyChanged("servo3");

                var servo3Int = (int)_servo3;

                Console.WriteLine($"Rotation Servo Now: {servo3Int}");

                if (Servo3_Enabled == true)
                {
                    hubConnection.SendAsync("SendMessage", "servo3", servo3Int.ToString());
                }
            }
        }

        public bool Servo1_Enabled
        {
            get => _servo1_enabled;
            set
            {
                _servo1_enabled = value;
                OnPropertyChanged("Servo1_Enabled");

            }
        }

        public bool Servo2_Enabled
        {
            get => _servo2_enabled;
            set
            {
                _servo2_enabled = value;
                OnPropertyChanged("Servo2_Enabled");
            }
        }

        public bool Servo3_Enabled
        {
            get => _servo3_enabled;
            set
            {
                _servo3_enabled = value;
                OnPropertyChanged("Servo3_Enabled");

            }
        }


        public RobotControl()
        {
            SetupConnection();

            disableServo1Timer = Application.Current.Dispatcher.CreateTimer();
            disableServo1Timer.IsRepeating = false;
            disableServo1Timer.Interval = TimeSpan.FromSeconds(1);
            disableServo1Timer.Tick += (s, e) => {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Re-enabling Servo 1");
                    Servo1_Enabled = true;
                });
            };

            disableServo2Timer = Application.Current.Dispatcher.CreateTimer();
            disableServo2Timer.IsRepeating = false;
            disableServo2Timer.Interval = TimeSpan.FromSeconds(1);
            disableServo2Timer.Tick += (s, e) => {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Re-enabling Servo 2");
                    Servo2_Enabled = true;

                });
            };

            disableServo3Timer = Application.Current.Dispatcher.CreateTimer();
            disableServo3Timer.IsRepeating = false;
            disableServo3Timer.Interval = TimeSpan.FromSeconds(1);
            disableServo3Timer.Tick += (s, e) => {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Re-enabling Servo 3");
                    Servo3_Enabled = true;

                });
            };

        }

        private async void SetupConnection()
        {
            hubConnection = new HubConnectionBuilder()
                   .WithUrl("https://bcsrobotdevuksapp.azurewebsites.net/chathub", conf =>
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
                hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {

                    if (user == "servo1")
                    {
                        Servo1_Enabled = false;
                        disableServo1Timer.Stop();
                        disableServo1Timer.Start();
                        servo1 = double.Parse(message);
                    }
                    else if (user == "servo2")
                    {
                        Servo2_Enabled = false;
                        disableServo2Timer.Stop();
                        disableServo2Timer.Start();
                        servo2 = double.Parse(message);
                    }
                    else if (user == "servo3")
                    {
                        Servo3_Enabled = false;
                        disableServo3Timer.Stop();
                        disableServo3Timer.Start();
                        servo3 = double.Parse(message);
                    }
                    Console.WriteLine($"{message} posted by: {user}");
                });

                await hubConnection.StartAsync();
            }
            catch (System.Exception)
            {

                throw;
            }

        }
    }
}
