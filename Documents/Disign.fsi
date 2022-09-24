namespace Orimath.Design
open System

// ソフトウェアの要求分析・ドメイン設計

/// 未定義
type ``?`` = unit -> unit

/// 自然数(0以上の整数)
type [<Struct>] Natural = Natural of uint

type nat = Natural

/// エラー共通
type Error = Error of message: string

/// IO操作(Async<Result<T, Error>> の簡易的再実装)
type [<Struct>] IO<'T> = IO of exec: ((Result<'T, Error> -> unit) -> unit)

/// 直接の入力ではなく、依存関係としての入力を表す。
type DependencyAttribute = inherit Attribute


type [<Interface>] IStateReader<'T> =
    val Value : IO<'T>

type [<Interface>] IStateWriter<'T> =
    val SetValue : 'T -> IO<unit>

type [<Interface>] IState<'T> =
    inherit IStateReader<'T>
    inherit IStateWriter<'T>

/// レイヤー番号
type [<Measure>] LayerId

/// 折線の種類
type [<Struct>] CreaseType = Crease | Draft | Mountain | Valley

/// フチの種類
type [<Struct>] EdgeType = Inside | Outside

type [<Struct>] Point = { X: float; Y: float }
type [<Struct>] Line = { XFactor: float; YFactor: float; Intercept: float }
type [<Struct>] LineSegment = { Line: Line; P1: Point; P2: Point }
type [<Struct>] Crease = { Type: CreaseType; Segment: LineSegment }
type [<Struct>] Edge = { Type: EdgeType; Segment: LineSegment }
type Layer = { Edges: Edge list; Creases: Crease list; Points: Point list }
type [<Struct>] Paper = { Layers: Layer list }

type UpdatePaperEventUnit =
    | AddPoint of uint<LayerId> * Point list
    | AddSegment of uint<LayerId> * LineSegment list
    | ReplaceLayer of uint<LayerId> * Layer
    | ReplacePaper of Paper

type UpdatePaperEvent = {
    Name: string
    Units: UpdatePaperEventUnit list
}

type MeasureResult = {
    Segment: LineSegment
}

/// 操作履歴の操作ユニット
type OperationHistoryUnit =
    | PointAdded of uint<LayerId> * Point list
    | SegmentAdded of uint<LayerId> * LineSegment list * Point list
    | LayerReplaced of uint<LayerId> * oldLayer: Layer * newLayer: Layer
    | PaperReplaced of oldPaper: Paper * newPaper: Paper

/// これまでの操作履歴(アンドゥ・リドゥにも使用)
type [<Struct>] OperationHistoryFrame = {
    Name: string
    Units: OperationHistoryUnit
}

type OperationHistory = {
    UndoStack: OperationHistoryFrame list
    RedoStack: OperationHistoryFrame list
}

type SerializationTarget = {
    Paper: Paper
    OperationHistory: OperationHistory
}

type GeneratedImage = ``?``

type [<Struct>] ImageFormat = Png | Svg

type GenerateImageSetting = {
    ImageFormat : ImageFormat
    Margin : uint
}

type GenerateInstructionSetting = {
    ImageFormat : ImageFormat
    Margin : uint
    IndexSize : uint
    IndexMargin : uint
    ColumnCount : nat
}

/// 設定
type Setting = {
    PaperScale: nat
    GenerateImageSetting: GenerateImageSetting
    GenerateInstructionSetting: GenerateInstructionSetting
}

type CreateNewPaperParameter =
    | Square
    | Rectangle of width: nat * height: nat
    | RegularPolygon of n: nat

type PathAsSave = IO<string>

type PathAsOpen = IO<string>

type [<Struct>] Orientation = Vertical | Horizontal

type [<Struct>] RotationOrientation = Clockwise | CounterClockwise

type [<RequireQualifiedAccess>] PointingElement =
    | Point of Point
    | Crease of Line * Point
    | Edge of Line * Point
    | Layer of Point

type SelectedElement = {
    Point: Point voption
    Segment: LineSegment voption
}

type PreviewElement =
    | PreviewLine of LineSegment
    | PreviewArrow of LineSegment
    | PreviewPoint of Point

type InstructionProcess = {
    Paper : Paper
    PreviewElements : PreviewElement list
}

type Instruction = InstructionProcess list

/// モデルロジック: すべて純粋関数
module BasicCommand =
    /// 回転する
    val rotate : RotationOrientation -> Paper -> UpdatePaperEvent
    /// 裏返す
    val turnOver : Orientation -> Paper -> UpdatePaperEvent
    /// 新しい紙
    val createNewPaper : CreateNewPaperParameter -> Paper
    /// シリアライズ
    val serialize : SerializationTarget -> byte[]
    /// デシリアライズ
    val deserialize : byte[] -> SerializationTarget

    /// 抽象画像構成を生成
    val generateImage : GenerateImageSetting -> Paper -> GeneratedImage
    /// 折り図を生成
    val generateInstruction : GenerateInstructionSetting -> OperationHistoryFrame list -> Instruction


module FoldTool =
    type [<Struct>] FoldParameter = {
        BeginElement: PointingElement
        EndElement: PointingElement
        SelectedElement: SelectedElement
    }

    type [<Struct>] FoldPreview = {
        PreviewElements: PreviewElement list
    }

    /// 折る操作の予告を取得
    val previewFold : FoldParameter -> Paper -> FoldPreview
    /// 紙を折る
    val fold : FoldParameter -> Paper -> UpdatePaperEvent


module DivideTool =
    type [<Struct>] DivideParameter = {
        BeginElement: PointingElement
        EndElement: PointingElement
        SelectedElement: SelectedElement
    }
    
    type [<Struct>] DividePreview = {
        PreviewElements: PreviewElement list
    }

    /// プレビュー
    val previewDivide : DivideParameter -> Paper -> DividePreview
    /// n分割折り
    val divide : DivideParameter -> Paper -> UpdatePaperEvent


module WriteLineTool =
    type [<Struct>] WriteLineParameter = {
        BeginElement: PointingElement
        EndElement: PointingElement
    }

    type [<Struct>] WriteLinePreview = {
        PreviewElements: PreviewElement list
    }

    /// 予告を取得
    val previewWriteLine : WriteLineParameter -> Paper -> WriteLinePreview
    /// 点から点に線を引く
    val writeLine : WriteLineParameter -> Paper -> UpdatePaperEvent


module MeasureTool =
    type [<Struct>] MeasureParameter = {
        BeginElement: PointingElement
        EndElement: PointingElement
    }

    /// 計測
    val measure : MeasureParameter -> Paper -> MeasureResult


module OperationHistory =
    /// 操作履歴を追加
    val updatePaper : OperationHistory -> Paper -> UpdatePaperEvent -> OperationHistory * Paper
    /// 元に戻す
    val undo : OperationHistory -> Paper -> OperationHistory * Paper
    /// やり直し
    val redo : OperationHistory -> Paper -> OperationHistory * Paper
    

/// UI から直接呼ばれる操作群
module View =
    /// 現在の紙の状態を取得
    val paper : IStateReader<Paper>
    /// 設定を取得
    val setting : IStateReader<Setting>

    val updatePaper : [<Dependency>] IState<Paper> -> [<Dependency>] IState<OperationHistory> -> IO<unit>

    /// 元に戻す
    val undo : [<Dependency>] IState<OperationHistory> -> [<Dependency>] IState<Paper> -> IO<unit>
    /// やり直し
    val redo : [<Dependency>] IState<OperationHistory> -> [<Dependency>] IState<Paper> -> IO<unit>

    /// クリア
    val clear : [<Dependency>] IStateReader<CreateNewPaperParameter> -> [<Dependency>] IStateWriter<Paper> -> IO<Paper>

    /// ファイルを開く
    val openFile : [<Dependency>] PathAsOpen -> IO<SerializationTarget>
    /// 上書き保存
    val save : [<Dependency>] PathAsSave -> SerializationTarget -> IO<unit>
    /// 名前をつけて保存
    val saveAs : [<Dependency>] PathAsSave -> SerializationTarget -> IO<unit>
    /// png形式・svg形式で保存
    val saveAsImage : [<Dependency>] PathAsSave -> GeneratedImage -> IO<unit>
    /// 折り図を保存
    val saveInstruction : [<Dependency>] PathAsSave -> Instruction -> IO<unit>
    /// 設定を変更
    val updateSetting : [<Dependency>] IStateWriter<Setting> -> Setting -> IO<unit>
