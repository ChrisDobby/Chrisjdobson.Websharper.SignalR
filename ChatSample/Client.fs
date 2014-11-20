namespace ChatSample

open IntelliFactory.WebSharper
open Chrisjdobson.WebSharper
open IntelliFactory.WebSharper.UI.Next

[<JavaScript>]
module Client =
    type ViewModel =
        {
            User : Var<string>
            Message : Var<string>
            MessageList : ListModel<string, Msg>
            ConnectionList : ListModel<string, string * string>
        }

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
                                ]

        let renderConnectionMessage (s : string * string) : Doc =
            Doc.Element "li" [] [Doc.TextNode (snd s)]

        let connectionList = 
            ListModel.View model.ConnectionList
                |> Doc.ConvertBy (fun s -> fst s) (renderConnectionMessage)

        let messageList = ListModel.View model.MessageList
                            |> Doc.ConvertBy (fun m -> m.SentAt + m.Name) (renderMessage)


        let s = SignalR.New "chatHub"
                        |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> model.MessageList.Add m)
                        |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> JavaScript.Alert ("Message received from " + m.Name))

        SignalRConnection.New() 
            |> SignalRConnection.WithLogging
            |> SignalRConnection.ConnectionError (fun e -> JavaScript.Alert e)
            |> SignalRConnection.Starting (fun _ -> model.ConnectionList.Add (EcmaScript.Date.Now().ToString(), "Connection starting"))
            |> SignalRConnection.Received (fun _ -> model.ConnectionList.Add (EcmaScript.Date.Now().ToString(), "Connection received"))
            |> SignalRConnection.ConnectionSlow (fun _ -> model.ConnectionList.Add (EcmaScript.Date.Now().ToString(), "Slow connection"))
            |> SignalRConnection.Reconnecting (fun _ -> model.ConnectionList.Add (EcmaScript.Date.Now().ToString(), "Connection reconnecting"))
            |> SignalRConnection.Reconnected (fun _ -> model.ConnectionList.Add (EcmaScript.Date.Now().ToString(), "Connection reconnected"))
            |> SignalRConnection.Disconnected (fun _ -> model.ConnectionList.Add (EcmaScript.Date.Now().ToString(), "Connection disconnected"))
            |> SignalRConnection.StateChanged (fun s -> model.ConnectionList.Add(EcmaScript.Date.Now().ToString(), ("from " + StateText s.oldState + " to " + StateText s.newState)))
            |> SignalRConnection.Start (fun _ -> ()) (fun e -> JavaScript.Alert ("connection error: " + e))

        Var.Set model.User (Prompt "Enter your name:" "")
        Doc.Element "div" [] [
            Doc.Element "div" [Attr.Class "container"] [
                Doc.Input [] model.Message
                Doc.Button "Send" [] (fun _ -> s |> SignalR.Send 
                                                        "chat" 
                                                        {SentAt = EcmaScript.Date.Now().ToString(); Name = model.User.Value; Message = model.Message.Value} 
                                                        (fun _ -> ()) // called when successfully sent
                                                        (fun e -> JavaScript.Alert e) // called when error sending
                                                        |> ignore)            
                Doc.Element "ul" [] [messageList]
            ]
            Doc.Element "div" [] [
                Doc.Element "ul" [] [connectionList]
            ]
        ]
        |> Doc.AsPagelet
