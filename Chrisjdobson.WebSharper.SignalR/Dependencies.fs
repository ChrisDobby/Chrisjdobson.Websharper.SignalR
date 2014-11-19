module Dependencies

module R = IntelliFactory.WebSharper.Core.Resources
module A = IntelliFactory.WebSharper.Core.Attributes

[<A.Require(typeof<IntelliFactory.WebSharper.JQuery.Resources.JQuery>)>]
[<Sealed>]
type SignalRJs() =
    inherit R.BaseResource("//ajax.aspnetcdn.com/ajax/signalr/", "jquery.signalr-2.1.2.min.js")

[<Sealed>]
type SignalRConnection() =
    interface R.IResource with
        member this.Render ctx writer = writer.RenderBeginTag "script"
                                        writer.InnerWriter.WriteLine "var connection = $.hubConnection();"
                                        writer.RenderEndTag()
