namespace Orimath
open System.Diagnostics

type ErrorCode =
    | Exception = 0
    | NegativeValue = 1
    | ZeroValue = 2
    | EmptyList = 3
    | InvalidLayer = 4
    | InvalidLine = 5
    | MustBeDifferent = 6


type Error =
    {
        Code: ErrorCode
        Message: string
        StackTrace: string
    }


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Error =
    let private create code message =
        {
            Code = code
            Message = message
            StackTrace = string (StackTrace(1))
        }

    let exn (ex: exn) =
        {
            Code = ErrorCode.Exception
            Message = ex.Message
            StackTrace = ex.StackTrace
        }

    let negativeValue = create ErrorCode.NegativeValue "0以下の値を正数型に設定できません。"

    let zeroValue = create ErrorCode.ZeroValue "0を非零型に設定できません。"

    let private invalidLayer message = create ErrorCode.InvalidLayer message

    let invalidLayerVertexesLessThan3 = invalidLayer "多角形の辺は3以上でなければなりません。"

    let invalidLayerPolygonMustBeClosed = invalidLayer "多角形の辺は閉じている必要があります。"

    let invalidLayerPointOutOfLayer = invalidLayer "レイヤー内に含まれていない点があります。"

    let invalidLayerSegmentOutOfLayer = invalidLayer "レイヤー内に含まれていない線分があります。"
    
    let emptyList = create ErrorCode.EmptyList "空のリストを1以上リストに設定できません。"

    let invalidLine = create ErrorCode.InvalidLine "直線の傾きを定義できません。"

    let mustBeDifferent = create ErrorCode.MustBeDifferent "値は異なっている必要があります。"