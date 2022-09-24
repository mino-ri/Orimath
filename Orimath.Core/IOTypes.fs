namespace Orimath
open System.Threading.Tasks

type AsyncIO<'T> = Task<Result<'T, Error>>


type IStateReader<'T> =
    abstract member Value : 'T


type IStateWriter<'T> =
    abstract member SetValue : 'T -> unit


type IState<'T> =
    inherit IStateReader<'T>
    inherit IStateWriter<'T>
