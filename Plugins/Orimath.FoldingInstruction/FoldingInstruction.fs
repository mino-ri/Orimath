namespace Orimath.FoldingInstruction
open System
open System.Collections.Generic
open Orimath.Core
open ApplicativeProperty

module internal Internal =
    let createArrayProp<'T when 'T : equality>() =
        ValueProp<'T[]>(Array.Empty(), { new IEqualityComparer<'T[]> with
            member _.Equals(a, b) = a.Length = b.Length && Array.forall2 (=) a b
            member _.GetHashCode(_) = 0 // not used
        }, System.Threading.SynchronizationContext.Current)

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
        StartType: ArrowType
        EndType: ArrowType
        Color: InstructionColor
    }
    
    static member Create(startPoint: Point, endPoint: Point, startType: ArrowType, endType: ArrowType, color: InstructionColor) =
        {
            Line = LineSegment.FromPoints(startPoint, endPoint).Value
            StartType = startType
            EndType = endType
            Color = color
        }

    static member Create(startPoint: Point, endPoint: Point, endType: ArrowType, color: InstructionColor) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.None, endType, color)

    static member Create(startPoint: Point, endPoint: Point, endType: ArrowType) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.None, endType, InstructionColor.Black)

    static member Normal(startPoint, endPoint, color) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.Normal, color)

    static member Normal(startPoint, endPoint) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.Normal)

    static member MountainFold(startPoint, endPoint, color) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.MountainFold, color)

    static member MountainFold(startPoint, endPoint) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.MountainFold)

    static member ValleyFold(startPoint, endPoint, color) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.ValleyFold, color)

    static member ValleyFold(startPoint, endPoint) =
        InstructionArrow.Create(startPoint, endPoint, ArrowType.ValleyFold)

[<Struct; NoComparison>]
type InstructionPoint =
    {
        Point: Point
        Color: InstructionColor
    }

type FoldingInstruction() =
    member val Lines = Internal.createArrayProp<InstructionLine>()
    member val Arrows = Internal.createArrayProp<InstructionArrow>()
    member val Points = Internal.createArrayProp<InstructionPoint>()
