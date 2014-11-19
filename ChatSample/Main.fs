namespace ChatSample

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets

type Action =
    | Home

module Controls =

    [<Sealed>]
    type EntryPoint() =
        inherit Web.Control()

        [<JavaScript>]
        override __.Body =
            Client.Main()

module Skin =
    open System.Web

    type Page =
        {
            Title : string
            Body : list<Content.HtmlElement>
        }

    let MainTemplate =
        Content.Template<Page>("~/Main.html")
            .With("title", fun x -> x.Title)
            .With("body", fun x -> x.Body)

    let WithTemplate title body : Content<Action> =
        Content.WithTemplate MainTemplate <| fun context ->
            {
                Title = title
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
