using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace robot_speech_maui;

public partial class MainPage : ContentPage
{
    RobotControl robotControl = new RobotControl();

    public MainPage()
	{
		InitializeComponent();
        
        BindingContext = robotControl;
        
    }	

}

