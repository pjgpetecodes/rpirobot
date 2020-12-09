# Add Blazor C# Control Code #

We need to create the SignalR Hub to our Server Project.

- Create a new directory in the Server project called `Hubs`
- Add a new cs file called `ChatHub.cs`.
- Add the following code;

    ```cs
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    namespace BlazorSignalRApp.Server.Hubs
    {
        public class ChatHub : Hub
        {
            public async Task SendMessage(string user, string message)
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }
    }
    ```

- This code will allow us to send Messages to the SignalR Hub.

We now need to add the SignalR service to our Server App.

- Open the `Server\Startup.cs" file.
- Add the following to the usings section;

    ```cs
    using BlazorSignalRApp.Server.Hubs;
    ```

- Add the following to the top of the `ConfigureServices` sub;

    ```cs
    services.AddSignalR();
    ```

- Add the following to the `app.useEndpoint` section after the `endpoints.MapRazorPages();` line;

    ```cs
    endpoints.MapHub<ChatHub>("/chathub");
    ```

Now we can create the C# Code for our App.

- Open the `Client\Pages\Index.razor` File
- Create a `@code` section at the bottom of the file;

    ```cs
    @code {
    }
    ```

- Add some backing variables to keep track of the Servo Angles at the top of the `@code` section;

    ```cs
    private int _servo1 = 90;
    private int _servo2 = 90;
    ```

- Add a SignalR Hub Connection variable below the backing variables;

    ```cs
    private HubConnection hubConnection;
    ```

Now we can add Properties for our backing variables. These properties connect to our Slider Controls, and send the Slider Value as a message through the SignalR Hub. 

- Add the following section of code beneath the hubConnection;

    ```cs
    public int servo1
    {
        get => _servo1;
        set
        {
            _servo1 = value;
            this.StateHasChanged();
            hubConnection.SendAsync("SendMessage", "servo1", _servo1.ToString());
        }
    }

    public int servo2
    {
        get => _servo2;
        set
        {
            _servo2 = value;
            this.StateHasChanged();
            hubConnection.SendAsync("SendMessage", "servo2", _servo2.ToString());
        }
    }
    ```

Next we can create the SignalR Hub and set the Uri that services will connect to.

- Add the following section of code beneath the Servo Properties;

    ```cs
    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
            .Build();

        await hubConnection.StartAsync();
    }
    ```

- We now need to store the Connection Status of the SignalR Hub. Add the following beneath the `OnInitializedAsync` sub;

    ```cs
    public bool IsConnected =>
        hubConnection.State == HubConnectionState.Connected;
    ```

Finally, we can make sure we dispose of the SignalR Hub when the app stops

- Add the following section of code below the `IsConnected` line;

    ```cs
    public async ValueTask DisposeAsync()
    {
        await hubConnection.DisposeAsync();
    }
    ```

| Previous | Next |
| -------- | ---- |
| [< Step 12 - Add Blazor Controls](12-add-blazor-controls.md) | [Step 14 - Configure and Run >](14-configure-and-run.md) |