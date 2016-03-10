#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Chrisjdobson.WebSharper.SignalR", "0.6.1")
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
                Version = System.Version("0.6.1")
                ProjectUrl = Some "https://github.com/chrisdobby/chrisjdobson.websharper.signalr"
                Description = "SignalR extension for WebSharper" })
        .AddDependency("Microsoft.AspNet.SignalR.SystemWeb", version = "2.2.0")
        .Add(main)

]
|> bt.Dispatch
