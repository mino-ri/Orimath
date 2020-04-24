namespace Orimath.Core
open System

type internal ReactiveProperty<'T> (sender: obj, init: 'T, eqComparer: 'T -> 'T -> bool) =
    let mutable value = init
    let valueChanged = Event<EventHandler, EventArgs>()
    
    member this.Value
        with get() = value
        and set v =
            if not (eqComparer value v) then
                value <- v
                valueChanged.Trigger(sender, EventArgs())

    member __.ValueChanged = valueChanged.Publish

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal ReactiveProperty =
    let createEq<'T when 'T : equality> sender init = ReactiveProperty<'T>(sender, init, (=))

    let createArray<'T when 'T : equality> sender =
        ReactiveProperty<'T[]>(sender, Array.empty, fun a b ->
            a.Length = b.Length && Array.forall2 (=) a b)
