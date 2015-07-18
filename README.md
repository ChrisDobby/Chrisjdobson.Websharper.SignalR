Chrisjdobson.Websharper.SignalR
===============================

[![Build status](https://ci.appveyor.com/api/projects/status/6q6wyp87i1w2qhlj?svg=true)](https://ci.appveyor.com/project/ChrisDobby/chrisjdobson-websharper-signalr)

A SignalR extension for WebSharper.

[![NuGet](http://img.shields.io/badge/NuGet-0.6-orange.svg?style=flat)](http://www.nuget.org/packages/chrisjdobson.WebSharper.SignalR/)

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
        (fun e -> JavaScript.Alert e.Message) // called when error sending
```

Configure connection:

``` fsharp
let startup = StartupConfig()
// To make a cross domain connection use Connection.New("www.abc.com")
Connection.New() 
    |> Connection.WithLogging
    |> Connection.ConnectionError (fun e -> JavaScript.Alert e)
    |> Connection.Starting (fun _ -> JavaScript.Alert "Starting")
    |> Connection.Received (fun _ -> JavaScript.Alert "Received")
    |> Connection.ConnectionSlow (fun _ -> JavaScript.Alert "Slow connection")
    |> Connection.Reconnecting (fun _ -> JavaScript.Alert "Reconnecting")
    |> Connection.Reconnected (fun _ -> JavaScript.Alert "Reconnected")
    |> Connection.Disconnected (fun _ -> JavaScript.Alert "Disconnected")
	|> Connection.StateChanged (fun s -> JavaScript.Alert ("from " + StateText s.OldState + " to " + StateText s.NewState))
    |> Connection.Start startup (fun _ -> ()) (fun e -> JavaScript.Alert ("connection error: " + e.Message))
```

## Samples ##

See the ChatSample and ProcessSample applications in the solution for simple examples

