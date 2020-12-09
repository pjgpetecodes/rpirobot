# Add Blazor Controls #

We now need to add the necessary sections of code to allow us to use MatBlazor

- Open the `Client\_Imports.razor` file.
- Add the following line to the end of the file;

    ```
    @using MatBlazor
    ```

- Open the `Client\wwwroot\index.html` file and add the following just before the `</head>` closing tag on line 12;

    ```html
    <script src="_content/MatBlazor/dist/matBlazor.js"></script>
    <link href="_content/MatBlazor/dist/matBlazor.css" rel="stylesheet" />
    ```

We now need to enable access to the server app from any machine on our network...

- Open the `Server\Properties\launchSettings.json` file.
- Change the `applicationUrl` line within the `robot_web.Server` section on line 24 to the following;

    ```json
    "applicationUrl": "https://0.0.0.0:5001;http://0.0.0.0:5000",
    ```
We can now add the markup and code to create a SignalR Hub, a couple of MatBlazor Sliders.

- Open the `Client\Pages\Index.razor` file.
- Remove everything but the `@page "/"` line.
- Add the following statements under the `@page "/"` line to add the SignalR Client references;

    ```cs
    @using Microsoft.AspNetCore.SignalR.Client
    @inject NavigationManager NavigationManager
    @implements IAsyncDisposable
    ```

Next, we can add the sliders to control the Servos.

- Add the following statements beneath the `@implements IAsyncDisposable` Line;

    ```html
    <div class="form-group row">
        <label>
            Servo 1 (@servo1):
        </label>
        <MatSlider @bind-Value="@servo1" Immediate="true" Step="1" EnableStep="true" ValueMin="0" ValueMax="180" Markers="true" Pin="true" TValue="int" Discrete="true"></MatSlider>
    </div>

    <div class="form-group row">
        <label>
            Servo 2 (@servo2):
        </label>
        <MatSlider @bind-Value="@servo2" Immediate="true" Step="1" EnableStep="true" ValueMin="0" ValueMax="180" Markers="true" Pin="true" TValue="int" Discrete="true"></MatSlider>
    </div>
    ```

| Previous | Next |
| -------- | ---- |
| [< Step 11 - Create Blazor App](11-create-blazor-app.md) | [Step 13 - Add Control Code >](13-add-control-code.md) |