using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using Microsoft.CognitiveServices.Speech;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;

namespace robot_speech_maui
{
    internal class RobotControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ICommand NewCommand { private set; get; }
        public ICommand ListenCommand { private set; get; }
        public ICommand CancelCommand { private set; get; }

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

        private bool _listenEnabled = true;
        private string _listeningText = "";
        private string _recognisedText = "";
        private string _actionText = "";
        private string _amountText = "";

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

        public string ListeningText
        {
            get { return _listeningText; }
            set
            {
                if (_listeningText != value)
                {
                    _listeningText = value;
                    OnPropertyChanged("ListeningText");
                }
            }
        }

        public string RecognisedText
        {
            get { return _recognisedText; }
            set
            {
                if (_recognisedText != value)
                {
                    _recognisedText = value;
                    OnPropertyChanged("RecognisedText");
                }
            }
        }

        public string ActionText
        {
            get { return _actionText; }
            set
            {
                if (_actionText != value)
                {
                    _actionText = value;
                    OnPropertyChanged("ActionText");
                }
            }
        }

        public string AmountText
        {
            get { return _amountText; }
            set
            {
                if (_amountText != value)
                {
                    _amountText = value;
                    OnPropertyChanged("AmountText");
                }
            }
        }


        public bool ListenEnabled
        {
            get { return _listenEnabled; }
            set
            {
                if (_listenEnabled != value)
                {
                    _listenEnabled = value;
                    OnPropertyChanged("ListenEnabled");
                }
            }
        }


        public RobotControl()
        {
            SetupConnection();

            ListenCommand = new Command(
                execute: () =>
                {
                    BeginListening();
                }
            );


            disableServo1Timer = Application.Current.Dispatcher.CreateTimer();
            disableServo1Timer.IsRepeating = false;
            disableServo1Timer.Interval = TimeSpan.FromSeconds(1);
            disableServo1Timer.Tick += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Re-enabling Servo 1");
                    Servo1_Enabled = true;
                });
            };

            disableServo2Timer = Application.Current.Dispatcher.CreateTimer();
            disableServo2Timer.IsRepeating = false;
            disableServo2Timer.Interval = TimeSpan.FromSeconds(1);
            disableServo2Timer.Tick += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Re-enabling Servo 2");
                    Servo2_Enabled = true;

                });
            };

            disableServo3Timer = Application.Current.Dispatcher.CreateTimer();
            disableServo3Timer.IsRepeating = false;
            disableServo3Timer.Interval = TimeSpan.FromSeconds(1);
            disableServo3Timer.Tick += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Re-enabling Servo 3");
                    Servo3_Enabled = true;

                });
            };

        }

        private async void BeginListening()
        {
            try
            {

                ListenEnabled = false;
                ListeningText = "";
                RecognisedText = "";
                ActionText = "";
                AmountText = "";

                var speechConfig = SpeechConfig.FromSubscription(AppSettings.SpeechKey, AppSettings.SpeechRegion);
                speechConfig.SpeechRecognitionLanguage = "en-US";

                using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

                // Creates an intent recognizer in the specified language using microphone as audio input.
                using (var intentRecognizer = new IntentRecognizer(speechConfig, audioConfig))
                {
                    var cluModel = new ConversationalLanguageUnderstandingModel(
                        AppSettings.LanguageKey,
                        AppSettings.LanguageEndpoint,
                        AppSettings.CluProjectName,
                        AppSettings.CluDeploymentName);
                    var collection = new LanguageUnderstandingModelCollection();
                    collection.Add(cluModel);
                    intentRecognizer.ApplyLanguageModels(collection);

                    ListeningText = "Speak into your microphone.";
                    var recognitionResult = await intentRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

                    // Checks result.
                    if (recognitionResult.Reason == ResultReason.RecognizedIntent)
                    {
                        ListeningText = $"\n RECOGNIZED: Text={recognitionResult.Text}";
                        ListeningText += $"\n    Intent Id: {recognitionResult.IntentId}.";

                        var json = recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);

                        RecognisedText = $"    Language Understanding JSON: {json}.";

                        JsonDocument doc = JsonDocument.Parse(json);

                        JsonElement root = doc.RootElement;
                        JsonElement result = root.GetProperty("result");
                        JsonElement prediction = result.GetProperty("prediction");
                        JsonElement entities = prediction.GetProperty("entities");

                        try
                        {
                            string movementType = "";
                            int movementValue = 0;

                            // Mapping of word representation to numeric values
                            Dictionary<string, int> wordToNumberMap = new Dictionary<string, int>()
                        {
                            { "zero", 0 },
                            { "one", 1 },
                            { "two", 2 },
                            { "three", 3 },
                            { "four", 4 },
                            { "five", 5 },
                            { "six", 6 },
                            { "seven", 7 },
                            { "eight", 8 },
                            { "nine", 9 },
                        };

                            foreach (JsonElement entity in entities.EnumerateArray())
                            {
                                string category = entity.GetProperty("category").GetString();
                                string text = entity.GetProperty("text").GetString();

                                if (category == "Movement")
                                {
                                    movementType = text;
                                    ActionText = text;
                                }
                                else if (category == "Amount")
                                {
                                    text = text.ToLower().Replace("degrees", "");
                                    text = text.ToLower().Replace("minus", "-");
                                    text = text.Replace(" ", "");

                                    if (wordToNumberMap.TryGetValue(text, out int numericValue))
                                    {
                                        movementValue = numericValue;
                                    }
                                    else
                                    {
                                        movementValue = int.Parse(text);
                                    }

                                    AmountText = movementValue.ToString();

                                }
                            }

                            Console.WriteLine("Movement Type: " + movementType);
                            Console.WriteLine("Movement Value: " + movementValue);

                            switch (movementType)
                            {
                                case "rotate":

                                    if ((servo1 + movementValue < 180) && (servo1 + movementValue > 0))
                                    {
                                        servo1 += movementValue;
                                        await hubConnection.InvokeAsync("SendMessage", "servo1", servo1.ToString());
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cannot rotate that far");
                                    }

                                    break;

                                case "reach":

                                    if ((servo2 + movementValue < 180) && (servo2 + movementValue > 0))
                                    {
                                        servo2 += movementValue;
                                        await hubConnection.InvokeAsync("SendMessage", "servo2", servo2.ToString());
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cannot reach that far");
                                    }
                                    break;

                                case "grab":

                                    if ((servo3 + movementValue < 180) && (servo3 + movementValue > 65))
                                    {
                                        servo3 += movementValue;
                                        await hubConnection.InvokeAsync("SendMessage", "servo3", servo3.ToString());
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cannot grab that far");
                                    }
                                    break;

                            }
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                    else if (recognitionResult.Reason == ResultReason.RecognizedSpeech)
                    {
                        ListeningText = $"RECOGNIZED: Text={recognitionResult.Text}";
                        ListeningText += $"    Intent not recognized.";
                    }
                    else if (recognitionResult.Reason == ResultReason.NoMatch)
                    {
                        ListeningText = $"NOMATCH: Speech could not be recognized.";
                    }
                    else if (recognitionResult.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(recognitionResult);
                        ListeningText = $"CANCELED: Reason={cancellation.Reason}";

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            ListeningText += $"CANCELED: ErrorCode={cancellation.ErrorCode}";
                            ListeningText += $"CANCELED: ErrorDetails={cancellation.ErrorDetails}";
                            ListeningText += $"CANCELED: Did you update the subscription info?";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            ListenEnabled = true;

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
