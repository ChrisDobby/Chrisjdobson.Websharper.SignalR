#load "tools/includes.fsx"
open IntelliFactory.Build

type Ref(p, v) =
    interface INuGetReference with
        member x.PackageId with get() = p
        member x.PackageVersion with get() = Some v
        member x.Paths with get() = None

let bt =
    BuildTool().PackageId("Chrisjdobson.WebSharper.SignalR", "0.1.0-alpha")
        .References(fun r -> [r.Assembly "System.Web"])
    |> fun bt -> bt.WithFramework(bt.Framework.Net40)

let main =
    bt.WebSharper.Library("Chrisjdobson.WebSharper.SignalR")
        .SourcesFromProject()

bt.Solution [
    main

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c.WithApache20License() with
                Title = Some "Chrisjdobson.WebSharper.SignalR"
                Version = System.Version("0.1.0")
                NuGetReferences = [Ref("Microsoft.AspNet.SignalR.SystemWeb", "2.1.2")]
                ProjectUrl = Some "https://github.com/chrisdobby/chrisjdobson.websharper.signalr"
                Description = "SignalR extension for WebSharper" })
        .Add(main)

]
|> bt.Dispatch
