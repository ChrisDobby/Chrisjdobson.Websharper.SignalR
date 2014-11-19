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
        }

    [<Inline "prompt($question, $defaultResponse)">]
    let Prompt (question: string) (defaultResponse: string) = null : string

    let CreateModel() = {User = Var.Create ""; Message = Var.Create "";MessageList = ListModel.Create (fun m -> m.SentAt + m.Name)[]}

    let Main () =
        let model = CreateModel()
        let renderMessage (m : Msg) : Doc = 
            Doc.Element "li" [] [
                                    Doc.Element "strong" [] [Doc.TextNode m.Name]
                                    Doc.TextNode (": " + m.Message)
                                ]
        let messageList = ListModel.View model.MessageList
                            |> Doc.ConvertBy (fun m -> m.SentAt + m.Name) (renderMessage)

        let s = SignalR.New "chatHub"
                        |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> model.MessageList.Add m)
                        |> SignalR.Receive<Msg> "broadcastMessage" (fun m -> JavaScript.Alert ("Message received from " + m.Name))

        SignalRConnection.New() 
            |> SignalRConnection.WithLogging
            |> SignalRConnection.ConnectionError (fun e -> JavaScript.Alert e)
            |> SignalRConnection.Start
        
        Var.Set model.User (Prompt "Enter your name:" "")
        Doc.Element "div" [] [
            Doc.Input [] model.Message
            Doc.Button "Send" [] (fun _ -> s |> SignalR.Send "chat" {SentAt = EcmaScript.Date.Now().ToString(); Name = model.User.Value; Message = model.Message.Value} |> ignore)            
            Doc.Element "ul" [] [messageList]
        ]
        |> Doc.AsPagelet
