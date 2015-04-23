namespace ProcessSample

open WebSharper.Html.Server
open WebSharper
open WebSharper.Sitelets

type Action =
    | Home

module Controls =    
    open WebSharper.UI.Next

    [<Sealed>]
    type EntryPoint() =
        inherit Web.Control()

        [<JavaScript>]
        override __.Body =
            Client.Main() :> _


module Skin =
    open System.Web

    type Page =
        {
            Body : list<Content.HtmlElement>
        }

    let MainTemplate =
        Content.Template<Page>("~/Main.html")
            .With("body", fun x -> x.Body)

    let WithTemplate title body : Content<Action> =
        Content.WithTemplate MainTemplate <| fun context ->
            {
                Body = body context
            }

module Site =

    let ( => ) text url =
        A [HRef url] -< [Text text]

    let HomePage =
        Skin.WithTemplate "HomePage" <| fun ctx ->
            [
                Div [new Controls.EntryPoint()]
            ]

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
        ]

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [Home]

type Global() =
    inherit System.Web.HttpApplication()

    member g.Application_Start(sender: obj, args: System.EventArgs) =
        ()

[<assembly: Website(typeof<Website>)>]
do ()
