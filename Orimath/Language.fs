namespace Orimath
open Orimath.Internal
open Sssl

[<Sealed>]
type Language private (settings: Map<string, SsslObject>) =
    member private _.ResolvePath(path: string) =
        let source, local =
            match path.Split('/') with
            | [| |] -> "global", ""
            | [| local |] -> "global", local
            | [| source; local |] -> source, local
            | splitted -> splitted.[0], splitted.[1]
        Map.tryFind source settings
        |> Option.bind (fun sssl ->
            let local = local.Split('.') |> List.ofArray
            let rec resolvePath sssl local =
                match sssl, local with
                | SsslString(value), [] -> Some(value)
                | SsslRecord(record), (head :: tail) ->
                    match record.TryGetValue(head) with
                    | BoolNone -> None
                    | BoolSome(sssl) -> resolvePath sssl tail
                | _ -> None
            resolvePath sssl local)

    member private this.GetWordCore(text: string) =
        if isNull text then null
        elif text.StartsWith('{') && text.Contains('}') then
            let index = text.IndexOf('}')
            let path = text.[1 .. index - 1]
            this.ResolvePath(path) |> Option.defaultWith (fun () -> text.[index + 1 ..])
        else
            text

    static member val private Code = "" with get, set

    static member val private Instance = Unchecked.defaultof<Language> with get, set

    static member internal SetInstance(code, settings) =
        Language.Code <- if Map.isEmpty settings then "en" else code
        Language.Instance <- Language(settings)

    static member LanguageCode = Language.Code

    static member GetWord(text) = Language.Instance.GetWordCore(text)
