﻿namespace Orimath.Basics
open System.ComponentModel
open Orimath.Plugins

[<DisplayName("コマンド: 基本機能"); Description("アンドゥ・リドゥなど基本的な操作を含みます。")>]
type BasicPlugin() =
    interface IPlugin with
        member __.Execute(args) =
            args.Workspace.AddEffect(UndoEffect(args.Workspace))
            args.Workspace.AddEffect(RedoEffect(args.Workspace))
