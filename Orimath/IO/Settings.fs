module Orimath.IO.Settings
open System.Diagnostics
open System.IO
open System.Reflection
open Sssl

[<Literal>]
let Plugin = "plugins";

[<Literal>]
let Global = "global";

let settingDirectory = 
    Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Settings")

let getSettingPath(fileName) = Path.Combine(settingDirectory, fileName + ".sssl")

let save fileName (target: obj) =
    if not (Directory.Exists(settingDirectory)) then
        ignore (Directory.CreateDirectory(settingDirectory))

    let ok, sssl = SsslConverter.Default.TryConvertFrom(target)
    if ok then
        // todo: リトライ処理などを入れる
        try
            sssl.Save(getSettingPath(fileName))
        with
        | ex -> Debug.Print(ex.ToString())

let load fileName : 'T option =
    let path = getSettingPath(fileName)
    if not (File.Exists(path)) then
        None
    else
        try
            Some(SsslConverter.Default.ConvertTo<_>(SsslObject.Load(path)))
        with
        | ex ->
            Debug.Print(ex.ToString())
            None
