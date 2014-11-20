Chrisjdobson.Websharper.SignalR
===============================

[![Build status](https://ci.appveyor.com/api/projects/status/6q6wyp87i1w2qhlj?svg=true)](https://ci.appveyor.com/project/ChrisDobby/chrisjdobson-websharper-signalr)

A SignalR extension for WebSharper.  Currently very early alpha version - more functionality to be added

[![NuGet](http://img.shields.io/badge/NuGet-0.3%20alpha-orange.svg?style=flat)](http://www.nuget.org/packages/chrisjdobson.WebSharper.SignalR/)

## Usage ##

Create a SignalR connection (to a hub called chatHub) like this:

``` fsharp
let s = SignalR.New "chatHub"
```

Add functions to receive like this:

``` fsharp
s |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> messageList.Add m)
  |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> JavaScript.Alert "Message Received")
```

And send like this:

``` fsharp
s |> SignalR.Send<Msg> 
		"chat" 
		{User = "User1"; Message = "Hello"}
        (fun _ -> ()) // called when successfully sent
        (fun e -> JavaScript.Alert e) // called when error sending
```

Configure connection:

``` fsharp
SignalRConnection.New() 
    |> SignalRConnection.WithLogging
    |> SignalRConnection.ConnectionError (fun e -> JavaScript.Alert e)
    |> SignalRConnection.Starting (fun _ -> JavaScript.Alert "Starting")
    |> SignalRConnection.Received (fun _ -> JavaScript.Alert "Received")
    |> SignalRConnection.ConnectionSlow (fun _ -> JavaScript.Alert "Slow connection")
    |> SignalRConnection.Reconnecting (fun _ -> JavaScript.Alert "Reconnecting")
    |> SignalRConnection.Reconnected (fun _ -> JavaScript.Alert "Reconnected")
    |> SignalRConnection.Disconnected (fun _ -> JavaScript.Alert "Disconnected")
	|> SignalRConnection.StateChanged (fun s -> JavaScript.Alert ("from " + StateText s.oldState + " to " + StateText s.newState))
    |> SignalRConnection.Start (fun _ -> ()) (fun e -> JavaScript.Alert ("connection error: " + e))
```

## Samples ##

See the ChatSample application in the solution for a simple example