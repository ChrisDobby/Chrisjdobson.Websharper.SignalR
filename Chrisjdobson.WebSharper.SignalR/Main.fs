namespace Chrisjdobson.WebSharper.SignalR

open IntelliFactory.WebSharper

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html

type TransportType =
    | WebSockets
    | ForeverFrame
    | ServerSentEvents
    | LongPolling

type ConnectionState =
    | Connecting = 0
    | Connected = 1
    | Reconnecting = 2
    | Disconnected = 4

type StateChange =
    {
        newState : ConnectionState
        oldState : ConnectionState
    }

type StartupConfig[<JavaScript>]() =
    [<JavaScript>]
    let rec transportList transports =
        let transportText =
            function
                | TransportType.WebSockets -> "webSockets"
                | TransportType.ForeverFrame -> "foreverFrame"
                | TransportType.ServerSentEvents -> "serverSentEvents"
                | TransportType.LongPolling -> "longPolling"

        match transports with            
            | t::l -> (transportText t)::transportList l
            | _ -> []

    [<Name "transport">]
    [<Stub>]
    member val private T = Unchecked.defaultof<string array> with get, set

    [<JavaScript>]
    member x.Transport with set(v: TransportType list) = x.T <- (transportList v) |> List.toArray

[<Require(typeof<Dependencies.SignalRJs>)>]
[<Require(typeof<Dependencies.SignalRConnection>)>]
type Connection[<JavaScript>]() =
    [<JavaScript>]
    static member New() = Connection()

    [<JavaScript>]
    [<Inline "connection.url = $url + '/signalr'">]
    static member New(url : string) = Connection()

    [<JavaScript>]
    [<Inline "connection.logging = true">]
    static member WithLogging (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.logging = false">]
    static member WithoutLogging (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.error($f)">]
    static member ConnectionError (f : string -> unit) (s : Connection) = s

    [<JavaScript>]
    [<Inline "connection.starting($f)">]
    static member Starting (f : unit -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.received($f)">]
    static member Received (f : unit -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.connectionSlow($f)">]
    static member ConnectionSlow (f : unit -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.reconnecting($f)">]
    static member Reconnecting (f : unit -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.reconnected($f)">]
    static member Reconnected (f : unit -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.disconnected($f)">]
    static member Disconnected (f : unit -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.stateChanged($f)">]
    static member StateChanged (f : StateChange -> unit) (c : Connection) = c

    [<JavaScript>]
    [<Inline "connection.start($cfg).done($success).fail($fail)">]
    static member Start (cfg : StartupConfig) (success : unit -> unit) (fail : string -> unit) (s : Connection) = ()

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
    [<Inline "connection.createHubProxy($s.hubName).invoke($name, $m).done($success).fail($fail)">]
    static member Send<'a> (name : string) (m : 'a) (success : unit -> unit) (fail : string -> unit) (s : SignalR) = s

