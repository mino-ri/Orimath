module Orimath.IO.Settings
open System
open System.Diagnostics
open System.IO
open System.Reflection
open SsslFSharp
open Orimath
open Orimath.Core
open Orimath.Internal

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

let converter =
    let options = ObjectConversionOptions.AllowMissingMember ||| ObjectConversionOptions.AllowUnknownMember
    let convertFromSeg (converter: ISsslConverter) name (seg: LineSegment) =
        option {
            let! x1 = converter.TryConvertFrom(seg.Point1.X)
            let! y1 = converter.TryConvertFrom(seg.Point1.Y)
            let! x2 = converter.TryConvertFrom(seg.Point2.X)
            let! y2 = converter.TryConvertFrom(seg.Point2.Y)
            return Sssl.Tuple(name, x1, y1, x2, y2)
        }
    let convertToSeg (converter: ISsslConverter) sssl =
        match sssl with
        | Sssl.Record(name, _, [| x1Src; y1Src; x2Src; y2Src |]) ->
            option {
                let! x1 = converter.TryConvertTo(x1Src)
                let! y1 = converter.TryConvertTo(y1Src)
                let! x2 = converter.TryConvertTo(x2Src)
                let! y2 = converter.TryConvertTo(y2Src)
                let! seg = LineSegment.FromPoints({ X = x1; Y = y1 }, { X = x2; Y = y2 })
                return name, seg
            }
        | _ -> None
    SsslConverter.emptyBuilder
    |> SsslConverter.addBaseValues options
    |> SsslConverter.addPrimitiveValues
    |> SsslConverter.add typeof<Point>
        { new SsslConverterPart<Point>() with
            override _.TryConvertFrom(converter, value, expected) =
                option {
                    let! x = converter.TryConvertFrom(value.X)
                    let! y = converter.TryConvertFrom(value.Y)
                    return Sssl.Tuple(SsslType.getName expected typeof<Point>, x, y)
                }
            override _.TryConvertTo(converter, sssl, _) =
                match sssl with
                | Sssl.Record(_, _, [| xSrc; ySrc |]) ->
                    option {
                        let! x = converter.TryConvertTo(xSrc)
                        let! y = converter.TryConvertTo(ySrc)
                        return { X = x; Y = y }
                    }
                | _ -> None
        }
    |> SsslConverter.add typeof<Crease>
        { new SsslConverterPart<Crease>() with
            override _.TryConvertFrom(converter, value, _) =
                convertFromSeg converter (string value.Type) value.Segment
            override _.TryConvertTo(converter, sssl, _) =
                option {
                    let! name, seg = convertToSeg converter sssl
                    let! creaseType = Enum.TryParse<CreaseType>(name) |> BoolOption.toOption
                    return { Segment = seg; Type = creaseType }
                }
        }
    |> SsslConverter.add typeof<Edge>
        { new SsslConverterPart<Edge>() with
            override _.TryConvertFrom(converter, value, _) =
                let name = if value.Inner then "Inner" else "Edge"
                convertFromSeg converter name value.Segment
            override _.TryConvertTo(converter, sssl, _) =
                option {
                    let! name, seg = convertToSeg converter sssl
                    let inner = name = "Inner"
                    return { Segment = seg; Inner = inner }
                }
        }
    |> SsslConverter.add typeof<LineSegment>
        { new SsslConverterPart<LineSegment>() with
            override _.TryConvertFrom(converter, value, expected) =
                convertFromSeg converter (SsslType.getName expected typeof<LineSegment>) value
            override _.TryConvertTo(converter, sssl, _) =
                convertToSeg converter sssl |> Option.map snd
        }
    |> SsslConverter.add typeof<Matrix>
        { new SsslConverterPart<Matrix>() with
            override _.TryConvertFrom(converter, value, expected) =
                option {
                    let! m11 = converter.TryConvertFrom(value.M11)
                    let! m12 = converter.TryConvertFrom(value.M12)
                    let! m21 = converter.TryConvertFrom(value.M21)
                    let! m22 = converter.TryConvertFrom(value.M22)
                    let! ofX = converter.TryConvertFrom(value.OffsetX)
                    let! ofY = converter.TryConvertFrom(value.OffsetY)
                    return Sssl.Tuple(SsslType.getName expected typeof<Matrix>, m11, m12, m21, m22, ofX, ofY)
                }
            override _.TryConvertTo(converter, sssl, _) =
                match sssl with
                | Sssl.Record(_, _, [| m11; m12; m21; m22; ofX; ofY |]) ->
                    option {
                        let! m11 = converter.TryConvertTo(m11)
                        let! m12 = converter.TryConvertTo(m12)
                        let! m21 = converter.TryConvertTo(m21)
                        let! m22 = converter.TryConvertTo(m22)
                        let! ofX = converter.TryConvertTo(ofX)
                        let! ofY = converter.TryConvertTo(ofY)
                        return { M11 = m11; M12 = m12; M21 = m21; M22 = m22; OffsetX = ofX; OffsetY = ofY }
                    }
                | _ -> None
        }
    |> SsslConverter.build

let save fileName target =
    if not (Directory.Exists(settingDirectory)) then
        ignore (Directory.CreateDirectory(settingDirectory))

    match converter.TryConvertFrom(target) with
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
        try Some(converter.ConvertTo(Sssl.loadFromFile path))
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
