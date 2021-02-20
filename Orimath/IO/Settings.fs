module Orimath.IO.Settings
open System.Diagnostics
open System.IO
open System.Reflection
open SsslFSharp

[<Literal>]
let Plugin = "plugins";

[<Literal>]
let Global = "global";

let settingDirectory =
    Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Settings")

let languageDirectory =
    Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Languages")

let getSettingPath fileName =
    Path.Combine(settingDirectory, fileName + ".sssl")

let save fileName target =
    if not (Directory.Exists(settingDirectory)) then
        ignore (Directory.CreateDirectory(settingDirectory))

    match Sssl.tryConvertFrom target with
    | Some(sssl) ->
        // todo: リトライ処理などを入れる
        try Sssl.saveToFile SsslFormat.Default (getSettingPath fileName) sssl
        with ex -> Debug.Print(ex.ToString())
    | None -> ()

let load fileName : 'T option =
    let path = getSettingPath fileName
    if not (File.Exists(path)) then
        None
    else
        try Some(Sssl.convertTo (Sssl.loadFromFile path))
        with ex ->
            Debug.Print(ex.ToString())
            None

let internal loadLanguages (code: string) =
    try
        Directory.GetFiles(languageDirectory, $"*.{code.ToLowerInvariant()}.sssl")
        |> Seq.choose (fun path ->
            try Some(Path.GetFileName(path).Split('.').[0], Sssl.loadFromFile path)
            with ex ->
                Debug.Print(ex.ToString())
                None)
        |> Map
    with ex ->
        Debug.Print(ex.ToString())
        Map.empty
