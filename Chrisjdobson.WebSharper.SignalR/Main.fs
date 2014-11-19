namespace Chrisjdobson.WebSharper

open IntelliFactory.WebSharper

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html

[<Require(typeof<Dependencies.SignalRJs>)>]
[<Require(typeof<Dependencies.SignalRConnection>)>]
type SignalRConnection[<JavaScript>]() =
    [<JavaScript>]
    static member New() = SignalRConnection()

    [<JavaScript>]
    [<Inline "connection.logging = true">]
    static member WithLogging (c : SignalRConnection) = c

    [<JavaScript>]
    [<Inline "connection.logging = false">]
    static member WithoutLogging (c : SignalRConnection) = c

    [<JavaScript>]
    [<Inline "connection.error($f)">]
    static member ConnectionError (f : string -> unit) (c : SignalRConnection) = c

    [<JavaScript>]
    [<Inline "connection.start()">]
    static member Start (c : SignalRConnection) = ()

[<Require(typeof<Dependencies.SignalRJs>)>]
[<Require(typeof<Dependencies.SignalRConnection>)>]
type SignalR[<JavaScript>](hubName : string) =
    member private x.HubName() = hubName

    [<JavaScript>]
    static member New (hub : string) = SignalR(hub)

    [<JavaScript>]
    [<Inline "connection.createHubProxy($s.hubName).on($name, $f)">]
    static member Receive<'a> (name : string) (f : 'a -> unit) (s : SignalR) = s

    [<JavaScript>]
    [<Inline "connection.createHubProxy($s.hubName).invoke($name, $m)">]
    static member Send<'a> (name : string) (m : 'a) (s : SignalR) = s

