namespace Orimath.Basics
open Orimath.Plugins
open Orimath.Basics.ViewModels

type BasicPlugin() =
    interface IViewPlugin with
        member __.Execute(args) =
            args.Messenger.AddViewModel(new WorkspaceViewModel(args.Workspace, args.PointConverter, args.Dispatcher))
