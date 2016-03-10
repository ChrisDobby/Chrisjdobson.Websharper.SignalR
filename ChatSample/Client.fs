namespace ChatSample

open WebSharper
open Chrisjdobson.WebSharper.SignalR
open WebSharper.UI.Next
open WebSharper.UI.Next.Client

[<JavaScript>]
module Client =
    type ViewModel =
        {
            User : Var<string>
            Message : Var<string>
            MessageList : ListModel<string, Msg>
            ConnectionList : ListModel<string, string * string>
        }

    type ConnectionParams[<JavaScript>]() =
        [<Name "user">]
        [<Stub>]
        [<JavaScript>]
        member val User = Unchecked.defaultof<string> with get, set

    [<Inline "prompt($question, $defaultResponse)">]
    let Prompt (question: string) (defaultResponse: string) = null : string

    let CreateModel() = {
                            User = Var.Create ""
                            Message = Var.Create ""
                            MessageList = ListModel.Create (fun m -> m.SentAt + m.Name)[]
                            ConnectionList = ListModel.Create (fun s -> fst s)[]
                        }

    let StateText = 
        function
            | ConnectionState.Connected -> "connected"
            | ConnectionState.Connecting -> "connecting"
            | ConnectionState.Disconnected -> "disconnected"
            | ConnectionState.Reconnecting -> "reconnecting"
            | _ -> "unknown"

    let Main () =
        let model = CreateModel()
        let renderMessage (m : Msg) : Doc = 
            Doc.Element "li" [] [
                                    Doc.Element "strong" [] [Doc.TextNode m.Name]
                                    Doc.TextNode (": " + m.Message)
                                ] :> _

        let renderConnectionMessage (s : string * string) : Doc =
            Doc.Element "li" [] [Doc.TextNode (snd s)] :> _

        let connectionList = 
            ListModel.View model.ConnectionList
                |> Doc.BindSeqCachedBy (fun s -> fst s) (renderConnectionMessage)

        let messageList = ListModel.View model.MessageList
                            |> Doc.BindSeqCachedBy (fun m -> m.SentAt + m.Name) (renderMessage)


        let s = SignalR.New "chatHub"
                        |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> model.MessageList.Add m)
                        |> SignalR.Receive<string> "userConnected" (fun u -> model.ConnectionList.Add (JavaScript.Date.Now().ToString() + u, "User " + u + " connected"))

        Var.Set model.User (Prompt "Enter your name:" "")

        let startup = StartupConfig()
        Connection.New()
            |> Connection.WithQueryString (ConnectionParams(User = model.User.Value))
            |> Connection.WithLogging
            |> Connection.ConnectionError (fun e -> JavaScript.JS.Alert e)
            |> Connection.Starting (fun _ -> model.ConnectionList.Add (JavaScript.Date.Now().ToString(), "Connection starting"))
            |> Connection.Received (fun _ -> model.ConnectionList.Add (JavaScript.Date.Now().ToString(), "Connection received"))
            |> Connection.ConnectionSlow (fun _ -> model.ConnectionList.Add (JavaScript.Date.Now().ToString(), "Slow connection"))
            |> Connection.Reconnecting (fun _ -> model.ConnectionList.Add (JavaScript.Date.Now().ToString(), "Connection reconnecting"))
            |> Connection.Reconnected (fun _ -> model.ConnectionList.Add (JavaScript.Date.Now().ToString(), "Connection reconnected"))
            |> Connection.Disconnected (fun _ -> model.ConnectionList.Add (JavaScript.Date.Now().ToString(), "Connection disconnected"))
            |> Connection.StateChanged (fun s -> model.ConnectionList.Add(JavaScript.Date.Now().ToString(), ("from " + StateText s.OldState + " to " + StateText s.NewState)))
            |> Connection.Start startup (fun _ -> ()) (fun e -> JavaScript.JS.Alert ("connection error: " + e.Message))

        Doc.Element "div" [] [
            Doc.Element "div" [Attr.Class "container"] [
                Doc.Input [] model.Message
                Doc.Button "Send" [] (fun _ -> s |> SignalR.Send 
                                                        "chat" 
                                                        {SentAt = JavaScript.Date.Now().ToString(); Name = model.User.Value; Message = model.Message.Value} 
                                                        (fun _ -> ()) // called when successfully sent
                                                        (fun e -> JavaScript.JS.Alert e.Message) // called when error sending
                                                        |> ignore)            
                Doc.Element "ul" [] [messageList]
            ]
            Doc.Element "div" [] [
                Doc.Element "ul" [] [connectionList]
            ]
        ]
