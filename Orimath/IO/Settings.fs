﻿module Orimath.IO.Settings
open System.Diagnostics
open System.IO
open System.Reflection
open Orimath.Internal
open Sssl

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

    match SsslConverter.Default.TryConvertFrom(target) with
    | BoolSome(sssl) ->
        // todo: リトライ処理などを入れる
        try sssl.Save(getSettingPath fileName)
        with ex -> Debug.Print(ex.ToString())
    | BoolNone -> ()

let load fileName : 'T option =
    let path = getSettingPath fileName
    if not (File.Exists(path)) then
        None
    else
        try Some(SsslConverter.Default.ConvertTo<_>(SsslObject.Load(path)))
        with ex ->
            Debug.Print(ex.ToString())
            None

let internal loadLanguages (code: string) =
    try
        Directory.GetFiles(languageDirectory, $"*.{code.ToLowerInvariant()}.sssl")
        |> Seq.choose (fun path ->
            try Some(Path.GetFileName(path).Split('.').[0], SsslObject.Load(path))
            with ex ->
                Debug.Print(ex.ToString())
                None)
        |> Map
    with ex ->
        Debug.Print(ex.ToString())
        Map.empty