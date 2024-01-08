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

        public ICommand ListenCommand { private set; get; }

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

        private string _listButtonText = "Begin Listening";
        private bool _listenEnabled = true;
        private string _listeningText = "";
        private string _recognisedText = "";
        private string _actionText = "";
        private string _amountText = "";

        /// <summary>
        /// The Servo 1 Position
        /// </summary>
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

        /// <summary>
        /// The Servo 2 Position
        /// </summary>
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

        /// <summary>
        /// The Servo 3 Position
        /// </summary>
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

        /// <summary>
        /// Whether the Servo 1 Slider is Enabled 
        /// </summary>
        public bool Servo1_Enabled
        {
            get => _servo1_enabled;
            set
            {
                _servo1_enabled = value;
                OnPropertyChanged("Servo1_Enabled");

            }
        }

        /// <summary>
        /// Whether the Servo 2 Slider is Enabled 
        /// </summary>
        public bool Servo2_Enabled
        {
            get => _servo2_enabled;
            set
            {
                _servo2_enabled = value;
                OnPropertyChanged("Servo2_Enabled");
            }
        }

        /// <summary>
        /// Whether the Servo 3 Slider is Enabled 
        /// </summary>
        public bool Servo3_Enabled
        {
            get => _servo3_enabled;
            set
            {
                _servo3_enabled = value;
                OnPropertyChanged("Servo3_Enabled");

            }
        }

        /// <summary>
        /// The Listening Label Text
        /// </summary>
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

        /// <summary>
        /// The Speech Recognition Result Text
        /// </summary>
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

        /// <summary>
        /// The CLU Action REsult
        /// </summary>
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

        /// <summary>
        /// The CLU Amount Text
        /// </summary>
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

        /// <summary>
        /// Whether the Listen Button is Enabled
        /// </summary>
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

        /// <summary>
        /// The Listen Button Text
        /// </summary>
        public string ListenButtonText
        {
            get { return _listButtonText; }
            set
            {
                if (_listButtonText != value)
                {
                    _listButtonText = value;
                    OnPropertyChanged("ListenButtonText");
                }
            }
        }

        /// <summary>
        /// The Robot Control Class Constructor
        /// </summary>
        public RobotControl()
        {
            //
            // Setup the SignalR Connection
            //
            SetupConnection();

            //
            // Setup the handler for the Listen Button
            //
            ListenCommand = new Command(
                execute: () =>
                {
                    BeginListening();
                }
            );

            //
            // Create the Disable Servo 1 Slider Timer
            //
            CreateDisableSlidersTimers();

        }

        /// <summary>
        /// Create the Disable Sliders Timers
        /// </summary>
        private void CreateDisableSlidersTimers()
        {
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

            //
            // Create the Disable Servo 1 Slider Timer
            //
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

        /// <summary>
        /// Begin Listening using the Speech API and deliver to CLU
        /// </summary>
        private async void BeginListening()
        {
            try
            {
                //
                // Default Variables
                //
                ListenButtonText = "Listening";
                ListenEnabled = false;
                ListeningText = "";
                RecognisedText = "";
                ActionText = "";
                AmountText = "";

                //
                // Setup the Speech API
                //
                var speechConfig = SpeechConfig.FromSubscription(AppSettings.SpeechKey, AppSettings.SpeechRegion);
                speechConfig.SpeechRecognitionLanguage = "en-US";

                //
                // Use the Default Microphone
                //
                using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

                //
                // Create an intent recognizer in the specified language using microphone as audio input.
                //
                using (var intentRecognizer = new IntentRecognizer(speechConfig, audioConfig))
                {

                    //
                    // Create the Conversational Language Understanding Model
                    //
                    var cluModel = new ConversationalLanguageUnderstandingModel(
                        AppSettings.LanguageKey,
                        AppSettings.LanguageEndpoint,
                        AppSettings.CluProjectName,
                        AppSettings.CluDeploymentName);

                    //
                    // Create a CLU Collection
                    //
                    var collection = new LanguageUnderstandingModelCollection();
                    collection.Add(cluModel);
                    intentRecognizer.ApplyLanguageModels(collection);

                    ListeningText = "Speak into your microphone.";

                    //
                    // Listen for Speech and Recognise the result
                    //
                    var recognitionResult = await intentRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

                    //
                    // If the Speech API has recognised one of th defined Intents...
                    //
                    if (recognitionResult.Reason == ResultReason.RecognizedIntent)
                    {
                        //
                        // An Intent was recognised by the Speech API
                        //
                        ListeningText = $"RECOGNIZED: Text={recognitionResult.Text}";
                        ListeningText += $"\n    Intent Id: {recognitionResult.IntentId}.";

                        //
                        // Get the CLU JSON Result Text
                        //
                        var json = recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);

                        RecognisedText = $"    Language Understanding JSON: {json}.";

                        //
                        // Parse the JSOn elements of the CLU Result 
                        //
                        JsonDocument doc = JsonDocument.Parse(json);

                        //
                        // Move the Robot based on the Speech and CLU results
                        //
                        MoveRobotBySpeech(doc);
                        
                    }
                    else if (recognitionResult.Reason == ResultReason.RecognizedSpeech)
                    {
                        ListeningText = $"RECOGNIZED: Text={recognitionResult.Text}";
                        ListeningText += $"\n    Intent not recognized.";
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
                            ListeningText += $"\nCANCELED: ErrorCode={cancellation.ErrorCode}";
                            ListeningText += $"\nCANCELED: ErrorDetails={cancellation.ErrorDetails}";
                            ListeningText += $"\nCANCELED: Did you update the subscription info?";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            ListenButtonText = "Begin Listening";
            ListenEnabled = true;

        }

        /// <summary>
        /// Move the Robot based on the Speech and CLU results
        /// </summary>
        /// <param name="doc"></param>
        private async void MoveRobotBySpeech(JsonDocument doc)
        {
            JsonElement root = doc.RootElement;
            JsonElement result = root.GetProperty("result");
            JsonElement prediction = result.GetProperty("prediction");
            JsonElement entities = prediction.GetProperty("entities");

            //
            // Parse the Intent results and determine the Robot Arm Movement Type and Amount
            //
            try
            {
                //
                // Default the Movement Type and Values
                //
                string movementType = "";
                int movementValue = 0;

                //
                // Sometimes CLU can return words instead of numbers... Normally just for numbers between 0 and 9 though.
                //
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

                //
                // Loop through each of the CLU Entities until we get the Movement and Amount
                //
                foreach (JsonElement entity in entities.EnumerateArray())
                {
                    //
                    // Get the Category and Text... For us we're after a Category of "Movement" or "Amount"
                    //
                    // These two categories are defined in the CLU Model
                    //
                    string category = entity.GetProperty("category").GetString();
                    string text = entity.GetProperty("text").GetString();

                    switch (category)
                    {
                        case "Movement":                    // The Movement Type (Rotate/Reach/Grab)

                            movementType = text;
                            ActionText = text;
                            break;

                        case "Amount":                      // The Amount to move by (in Degrees)

                            //
                            // Remove Degrees, convert the word "Minus" to a negative and remove any spaces
                            //
                            // Training the model better might 
                            //
                            text = text.ToLower().Replace("degrees", "");
                            text = text.ToLower().Replace("minus", "-");
                            text = text.Replace(" ", "");

                            //
                            // If this is a text representation of a number, then convert it
                            //
                            if (wordToNumberMap.TryGetValue(text, out int numericValue))
                            {
                                movementValue = numericValue;
                            }
                            else
                            {
                                movementValue = int.Parse(text);
                            }

                            AmountText = movementValue.ToString();
                            break;

                    }
                    
                }

                Console.WriteLine("Movement Type: " + movementType);
                Console.WriteLine("Movement Value: " + movementValue);

                //
                // Move the part of the Arm based on the Movement Type
                //
                // We also make sure that we don't try to move the Arm parts out of range
                //
                switch (movementType.ToLower())
                {
                    case "rotate":
                    case "rotates":

                        if ((servo1 + movementValue < 180) && (servo1 + movementValue > 0))
                        {
                            //
                            // Convert the Movement Type and Amount to Speech
                            //
                            SpeakMovementOperation(movementType, movementValue.ToString());

                            servo1 += movementValue;
                            await hubConnection.InvokeAsync("SendMessage", "servo1", servo1.ToString());
                        }
                        else
                        {
                            //
                            // Let the user know that it can't move that far
                            //
                            SpeakMovementOperation("fail", movementValue.ToString());
                            Console.WriteLine("Cannot rotate that far");
                        }

                        break;

                    case "reach":

                        if ((servo2 + movementValue < 180) && (servo2 + movementValue > 0))
                        {
                            //
                            // Convert the Movement Type and Amount to Speech
                            //
                            SpeakMovementOperation(movementType, movementValue.ToString());
                            
                            servo2 += movementValue;
                            await hubConnection.InvokeAsync("SendMessage", "servo2", servo2.ToString());
                        }
                        else
                        {
                            //
                            // Let the user know that it can't move that far
                            //
                            SpeakMovementOperation("fail", movementValue.ToString());
                            Console.WriteLine("Cannot reach that far");
                        }
                        break;

                    case "grab":

                        if ((servo3 + movementValue < 180) && (servo3 + movementValue > 65))
                        {
                            //
                            // Convert the Movement Type and Amount to Speech
                            //
                            SpeakMovementOperation(movementType, movementValue.ToString());
                            
                            servo3 += movementValue;
                            await hubConnection.InvokeAsync("SendMessage", "servo3", servo3.ToString());
                        }
                        else
                        {
                            //
                            // Let the user know that it can't move that far
                            //
                            SpeakMovementOperation("fail", movementValue.ToString());
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

        /// <summary>
        /// Convert the Movement Type and Value into Speech
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Amount"></param>
        private async void SpeakMovementOperation(string MovementType, string MovementAmount)
        {
            try {
                // Using the speech api, convert the direction and amount to speech...
                var speechConfig = SpeechConfig.FromSubscription(AppSettings.SpeechKey, AppSettings.SpeechRegion);

                // The language of the voice that speaks.
                speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";

                using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
                {

                    string text = "";

                    switch (MovementType)
                    {
                        case "rotate":
                        case "rotates":

                            text = $"Rotating the Arm by {MovementAmount} degrees";
                            break;

                        case "reach":

                            text = $"Reaching by {MovementAmount} degrees";
                            break;

                        case "grab":

                            text = $"Grabbing by {MovementAmount} degrees";
                            break;

                        case "fail":

                            text = $"Sorry, I can't move by as much as {MovementAmount} degrees";
                            break;

                    }

                    var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
                    OutputSpeechSynthesisResult(speechSynthesisResult, text);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// The Speech Output Result
        /// </summary>
        /// <param name="speechSynthesisResult"></param>
        /// <param name="text"></param>
        private static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Setup the SignalR Connection
        /// </summary>
        private async void SetupConnection()
        {

            //
            // Create a new connection to the SignalR Hub
            //
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
                //
                // Setup the callback for messages to control the Robot Arm
                //
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
