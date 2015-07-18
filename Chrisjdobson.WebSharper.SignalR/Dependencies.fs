module Dependencies

module R = WebSharper.Core.Resources
module A = WebSharper.Core.Attributes

[<A.Require(typeof<WebSharper.JQuery.Resources.JQuery>)>]
[<Sealed>]
type SignalRJs() =
    inherit R.BaseResource("//ajax.aspnetcdn.com/ajax/signalr/", "jquery.signalr-2.1.2.min.js")


[<Sealed>]
type SignalRConnection() =
    interface R.IResource with
        member this.Render ctx writer = let scriptWriter = writer WebSharper.Core.Resources.RenderLocation.Scripts
                                        scriptWriter.RenderBeginTag "script"
                                        scriptWriter.InnerWriter.WriteLine "var connection = $.hubConnection();"
                                        scriptWriter.RenderEndTag()
