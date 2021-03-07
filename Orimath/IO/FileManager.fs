namespace Orimath.IO
open System.IO
open Microsoft.Win32
open Orimath
open Orimath.Plugins
open Orimath.Internal
open SsslFSharp

type FileManager(dispatcher: IDispatcher, workspace: IWorkspace) =
    member private _.ShowDialogCore(dialog: #FileDialog, fileTypeName, filter, callback) =
        dialog.Filter <- $"%s{fileTypeName} (%s{filter})|%s{filter}"
        dialog.FileName <- filter
        dialog.DefaultExt <- filter.Split('.') |> Array.tryLast |> Option.defaultValue ""
        let ok = dialog.ShowDialog()
        if ok.HasValue && ok.Value
        then dispatcher.Background { callback (Some(dialog.FileName)) }
        else callback None

    member private this.OpenPath(fileTypeName, filter, callback) =
        dispatcher.UI {
            let dialog = OpenFileDialog()
            dialog.Title <- Language.GetWord("{DialogTitle.OpenFile}Open file") + "..."
            this.ShowDialogCore(dialog, fileTypeName, filter, callback)
        }

    member private this.SavePath(fileTypeName, filter, callback) =
        dispatcher.UI {
            let dialog = SaveFileDialog()
            dialog.Title <- Language.GetWord("{DialogTitle.SaveFile}Save file") + "..."
            dialog.AddExtension <- true
            this.ShowDialogCore(dialog, fileTypeName, filter, callback)
        }

    member this.SaveObject(fileTypeName, filter, object: obj, objType) =
        Async.FromContinuations(fun (callback, onError, _) ->
            try
                this.SavePath(fileTypeName, filter, fun path ->
                    iter {
                        let! path = path
                        let! sssl = Settings.converter.TryConvertFrom(object, objType)
                        Sssl.saveToFile SsslFormat.Default path sssl
                    }
                    callback())
            with ex -> onError ex)

    member this.LoadObject<'T>(fileTypeName, filter, objType) =
        Async.FromContinuations(fun (callback, onError, _) ->
            try
                this.OpenPath(fileTypeName, filter, fun path ->
                    option {
                        let! path = path
                        if File.Exists(path) then
                            let! converted = Settings.converter.TryConvertTo(Sssl.loadFromFile path, objType)
                            return converted :?> 'T
                    }
                    |> callback)
            with ex -> onError ex)

    interface IFileManager with
        member this.SavePaper() =
            this.SaveObject(
                "Orimath orpject file", "*.orimath",
                workspace.Paper.GetRawUndoItems(),
                workspace.Paper.RawUndoItemType)

        member this.LoadPaper() =
            async {
                let! converted =
                    this.LoadObject<obj>(
                        "Orimath orpject file", "*.orimath",
                        workspace.Paper.RawUndoItemType)
                Option.iter workspace.Paper.SetRawUndoItems converted
            }

        member this.SaveObject(fileTypeName, filter, object: 'T) =
            this.SaveObject(fileTypeName, filter, object, typeof<'T>)

        member this.LoadObject<'T>(fileTypeName, filter) =
            this.LoadObject<'T>(fileTypeName, filter, typeof<'T>)

        member this.SaveStream(fileTypeName, filter) =
            Async.FromContinuations(fun (callback, onError, _) ->
                try
                    this.SavePath(fileTypeName, filter, fun path ->
                        option {
                            let! path = path
                            let file = File.Create(path)
                            return file :> Stream
                        }
                        |> callback)
                with ex -> onError ex)

        member this.LoadStream(fileTypeName, filter) =
            Async.FromContinuations(fun (callback, onError, _) ->
                try
                    this.OpenPath(fileTypeName, filter, fun path ->
                        option {
                            let! path = path
                            let file = File.Create(path)
                            return file :> Stream
                        }
                        |> callback)
                with ex -> onError ex)
