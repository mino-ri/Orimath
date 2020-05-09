namespace Orimath.FoldingInstruction
open Orimath.Core

type InstructionColor =
    /// CUD黒 / #000000
    | Black = 0
    /// CUD白 / #FFFFFF
    | White = 1
    /// CUD明るいグレー / #C8C8CB
    | LightGray = 2
    /// CUDグレー / #7F878F
    | Gray = 3
    /// CUD赤 / #FF2800
    | Red = 4
    /// CUD黄色 / #FAF500
    | Yellow = 5
    /// CUD緑 / #35A16B
    | Green = 6
    /// CUD青 / #0041FF
    | Blue = 7
    /// CUD空色 / #66CCFF
    | Skyblue = 8
    /// CUDピンク / #FF99A0
    | Pink = 9
    /// CUDオレンジ / #FF9900
    | Orange = 10
    /// CUD紫 / #9A0079
    | Purple = 11
    /// CUD茶 / #663300
    | Brown = 12
    /// CUD明るいピンク / #FFD1D1
    | LightPink = 13
    /// CUDクリーム / #FFFF99
    | Cream = 14
    /// CUD明るい黄緑 / #CBF266
    | YellowGreen = 15
    /// CUD明るい空色 / #B4EBFA
    | LightSkyblue = 16
    /// CUDベージュ / #EDC58F
    | Beige = 17
    /// CUD明るい緑 / #87E7B0
    | LightGreen = 18
    /// CUD明るい紫 / #C7B2DE
    | LightPurple = 19

[<Struct; NoComparison>]
type InstructionLine =
    {
        Line: LineSegment
        Color: InstructionColor
    }

type ArrowType =
    | None = 0
    | Normal = 1
    | MountainFold = 2
    | ValleyFold = 3

[<Struct; NoComparison>]
type InstructionArrow =
    {
        Line: LineSegment
        BeginType: ArrowType
        EndType: ArrowType
        Color: InstructionColor
    }

type FoldingInstruction() as this =    
    let mutable lines = ReactiveProperty.createArray<InstructionLine> this
    let mutable arrows = ReactiveProperty.createArray<InstructionArrow> this

    member __.Lines
        with get() = lines.Value
        and set(v) = lines.Value <- v

    member __.Arrows
        with get() = arrows.Value
        and set(v) = arrows.Value <- v

    [<CLIEvent>]
    member __.LinesChanged = lines.ValueChanged

    [<CLIEvent>]
    member __.ArrowsChanged = arrows.ValueChanged
