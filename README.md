Chrisjdobson.Websharper.SignalR
===============================

A SignalR extension for WebSharper.  Currently very early alpha version - more functionality to be added

[![NuGet](http://img.shields.io/badge/NuGet-0.1.0-orange.svg?style=flat)](http://www.nuget.org/packages/chrisjdobson.WebSharper.SignalR/)

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
s |> SignalR.Send<Msg> "chat" {User = "User1"; Message = "Hello"}
```

## Samples ##

