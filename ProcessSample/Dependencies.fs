module Dependancies

open WebSharper

[<Require(typeof<JQuery.Resources.JQuery>)>]
[<Sealed>]
type TwitterBootstrap() =
    inherit Resources.BaseResource("//netdna.bootstrapcdn.com/bootstrap/3.1.1/",
        "js/bootstrap.min.js")

[<assembly: Require(typeof<TwitterBootstrap>)>]
do ()