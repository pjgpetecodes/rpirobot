using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

class Program
{
        private static string languageKey;
        private static string languageEndpoint;
        private static string speechKey;
        private static string speechRegion;
        private static string cluProjectName;
        private static string cluDeploymentName;


    private static HubConnection connection;

    private static int _servo1Value = 0;
    private static int _servo2Value = 0;
    private static int _servo3Value = 0;

    public static int servo1Value
    {
        get { return _servo1Value; }
        set
        {
            if (value != _servo1Value)
            {
                _servo1Value = value;
                Console.WriteLine($"Servo 1 value set to {_servo1Value}");
            }
        }
    }

    public static int servo2Value
    {
        get { return _servo2Value; }
        set
        {
            if (value != _servo2Value)
            {
                _servo2Value = value;
                Console.WriteLine($"Servo 2 value set to {_servo2Value}");
            }
        }
    }

    public static int servo3Value
    {
        get { return _servo3Value; }
        set
        {
            if (value != _servo3Value)
            {
                _servo3Value = value;
                Console.WriteLine($"Servo 3 value set to {_servo3Value}");
            }
        }
    }

    async static Task Main(string[] args)
    {

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        languageKey = configuration.GetValue<string>("LanguageKey") ?? string.Empty;;
        languageEndpoint = configuration.GetValue<string>("LanguageEndpoint") ?? string.Empty;;
        speechKey = configuration.GetValue<string>("SpeechKey") ?? string.Empty;;
        speechRegion = configuration.GetValue<string>("SpeechRegion") ?? string.Empty;;
        cluProjectName = configuration.GetValue<string>("CluProjectName") ?? string.Empty;;
        cluDeploymentName = configuration.GetValue<string>("CluDeploymentName") ?? string.Empty;;

        connection = new HubConnectionBuilder()
                .WithUrl("https://bcsrobotdevuksapp.azurewebsites.net/chathub", conf =>
                {
                    conf.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    };
                })
                .WithAutomaticReconnect()
                .Build();

        Console.CancelKeyPress += async (sender, e) =>
        {
            Console.WriteLine("Disconnecting...");
            await connection.DisposeAsync();
            Environment.Exit(0);
        };

        try
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
           {

               if (user == "servo1")
               {

                   servo1Value = int.Parse(message);

                   Console.WriteLine($"Servo 1 value set to {message}");
               }
               else if (user == "servo2")
               {
                   servo2Value = int.Parse(message);

                   Console.WriteLine($"Servo 2 value set to {message}");
               }
               else if (user == "servo3")
               {

                   servo3Value = int.Parse(message);

                   Console.WriteLine($"Servo 3 value set to {message}");
               }
               Console.WriteLine($"{message} posted by: {user}");
           });

            await connection.StartAsync();

        }
        catch (System.Exception)
        {

            throw;
        }

        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
        speechConfig.SpeechRecognitionLanguage = "en-US";

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();


        while (true)
        {



            // Creates an intent recognizer in the specified language using microphone as audio input.
            using (var intentRecognizer = new IntentRecognizer(speechConfig, audioConfig))
            {
                var cluModel = new ConversationalLanguageUnderstandingModel(
                    languageKey,
                    languageEndpoint,
                    cluProjectName,
                    cluDeploymentName);
                var collection = new LanguageUnderstandingModelCollection();
                collection.Add(cluModel);
                intentRecognizer.ApplyLanguageModels(collection);

                Console.WriteLine("Speak into your microphone.");
                var recognitionResult = await intentRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (recognitionResult.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                    Console.WriteLine($"    Intent Id: {recognitionResult.IntentId}.");

                    var json = recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);

                    Console.WriteLine($"    Language Understanding JSON: {json}.");

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
                                
                            }
                        }

                        Console.WriteLine("Movement Type: " + movementType);
                        Console.WriteLine("Movement Value: " + movementValue);

                        switch (movementType)
                        {
                            case "rotate":

                                if ((servo1Value + movementValue < 180) && (servo1Value + movementValue > 0))
                                {
                                    servo1Value += movementValue;
                                    await connection.InvokeAsync("SendMessage", "servo1", servo1Value.ToString());
                                }
                                else
                                {
                                    Console.WriteLine("Cannot rotate that far");
                                }

                                break;

                            case "reach":

                                if ((servo2Value + movementValue < 180) && (servo2Value + movementValue > 0))
                                {
                                    servo2Value += movementValue;
                                    await connection.InvokeAsync("SendMessage", "servo2", servo2Value.ToString());
                                }
                                else
                                {
                                    Console.WriteLine("Cannot reach that far");
                                }
                                break;

                            case "grab":

                                if ((servo3Value + movementValue < 180) && (servo3Value + movementValue > 65))
                                {
                                    servo3Value += movementValue;
                                    await connection.InvokeAsync("SendMessage", "servo3", servo3Value.ToString());
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
                    Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (recognitionResult.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (recognitionResult.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(recognitionResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }

        }
    }
}