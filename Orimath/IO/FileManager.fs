namespace Orimath.IO
open System.IO
open Microsoft.Win32
open Orimath
open Orimath.Plugins
open Orimath.Internal
open SsslFSharp
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type FileManager(dispatcher: IDispatcher, workspace: IWorkspace) =
    let mutable paperFilePath = Prop.value None

    member private _.ShowDialogCore(dialog: #FileDialog, fileTypeName, filter, callback) =
        dialog.Filter <- $"%s{Language.GetWord(fileTypeName)} (%s{filter})|%s{filter}"
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

    member private _.SavePaper(path: string option) =
        let object = workspace.Paper.GetRawUndoItems()
        let objType = workspace.Paper.RawUndoItemType
        iter {
            let! path = path
            let! sssl = Settings.converter.TryConvertFrom(object, objType)
            Sssl.saveToFile SsslFormat.Default path sssl
            paperFilePath .<- Some(path)
        }

    member private _.LoadPaper(path: string option) =
        let objType = workspace.Paper.RawUndoItemType
        iter {
            let! path = path
            if File.Exists(path) then
                let! converted = Settings.converter.TryConvertTo(Sssl.loadFromFile path, objType)
                workspace.Paper.SetRawUndoItems converted
                paperFilePath .<- Some(path)
        }

    interface IFileManager with
        member val PaperFilePath = Prop.asGet paperFilePath

        member this.SavePaper() =
            if paperFilePath.Value.IsSome then
                Async.FromContinuations(fun (callback, onError, _) ->
                    try
                        this.SavePaper(paperFilePath.Value)
                        callback()
                    with ex -> onError ex)
            else
                (this :> IFileManager).SavePaperAs()

        member this.SavePaperAs() =
            Async.FromContinuations(fun (callback, onError, _) ->
                try
                    this.SavePath("Orimath orpject file", "*.orimath", fun path ->
                        this.SavePaper(path)
                        callback())
                with ex -> onError ex)

        member this.LoadPaper() =
            Async.FromContinuations(fun (callback, onError, _) ->
                try
                    this.OpenPath("Orimath orpject file", "*.orimath", fun path ->
                        this.LoadPaper(path)
                        callback())
                with ex -> onError ex)

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
                            return file
                        }
                        |> callback)
                with ex -> onError ex)

        member this.SaveIndexedStream(fileTypeName, filter) =
            Async.FromContinuations(fun (callback, onError, _) ->
                try
                    this.SavePath(fileTypeName, filter, fun path ->
                        match path with
                        | Some(path) ->
                            callback (fun index ->
                                Path.Combine(
                                    Path.GetDirectoryName(path),
                                    Path.GetFileNameWithoutExtension(path) +
                                    $"_%03d{index}" +
                                    Path.GetExtension(path))
                                |> File.Create
                                |> Some)
                        | None -> callback (fun _ -> None))
                with ex -> onError ex)

        member this.LoadStream(fileTypeName, filter) =
            Async.FromContinuations(fun (callback, onError, _) ->
                try
                    this.OpenPath(fileTypeName, filter, fun path ->
                        option {
                            let! path = path
                            let file = File.Create(path)
                            return file
                        }
                        |> callback)
                with ex -> onError ex)
