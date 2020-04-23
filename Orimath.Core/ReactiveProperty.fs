namespace Orimath.Core
open System

type internal ReactiveProperty<'T> (init: 'T, eqComparer: 'T -> 'T -> bool) =
    let mutable value = init
    let valueChanged = Event<EventHandler, EventArgs>()
    
    member this.Value
        with get() = value
        and set v =
            if not (eqComparer value v) then
                value <- v
                valueChanged.Trigger(this, EventArgs())

    member __.ValueChanged = valueChanged.Publish

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal ReactiveProperty =
    let createEq<'T when 'T : equality> init = ReactiveProperty<'T>(init, (=))

    let createArray<'T when 'T : equality> =
        ReactiveProperty<'T[]>(Array.empty, fun a b ->
            a.Length = b.Length && Array.forall2 (=) a b)
