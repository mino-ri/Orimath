namespace Orimath.Basics
open System.IO
open Orimath.Plugins
open ApplicativeProperty

type IImageExporter =
    abstract member FileTypeName : string
    abstract member Extension : string
    abstract member Export : FileStream * IPaperModel -> Async<unit>
    abstract member ShortcutKey : string
    abstract member EffectName : string
    abstract member Icon: Stream


type ExportEffect(fileManager: IFileManager, workspace: IWorkspace, exporter: IImageExporter) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.File}File" |]
        member _.Name = exporter.EffectName
        member _.ShortcutKey = exporter.ShortcutKey
        member _.Icon = exporter.Icon
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() =
            async {
                match! fileManager.SaveStream(exporter.FileTypeName, $"*.%s{exporter.Extension}") with
                | Some(stream) ->
                    use str = stream
                    do! exporter.Export(str, workspace.Paper)
                | _ -> return ()
            }
            |> Async.RunSynchronously
