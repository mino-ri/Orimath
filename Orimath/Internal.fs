﻿module internal Orimath.Internal
open System
open System.Collections.Concurrent
open System.Linq.Expressions
open System.Reflection
open System.Windows.Input

let private keyGestureConverter = KeyGestureConverter()

let convertToKeyGesture source =
    if not (String.IsNullOrWhiteSpace(source)) then
        try Some(keyGestureConverter.ConvertFromInvariantString(source) :?> KeyGesture)
        with _ -> None
    else
        None

let convert ty expr = Expression.Convert(expr, ty)

let property (propertyInfo: PropertyInfo) expr = Expression.Property(expr, propertyInfo)

let assign right left = Expression.Assign(left, right)

let compileLambda (parameters: ParameterExpression[]) body =
    Expression.Lambda<'Delegate>(body, parameters).Compile()

let private constructors = ConcurrentDictionary<Type, Func<obj>>()

let createInstance (ty: Type) =
    let constructor = constructors.GetOrAdd(ty, fun (t: Type) ->
        Expression.New(t)
        |> convert typeof<obj>
        |> compileLambda [||])
    constructor.Invoke()

let tryCast<'T> (value: obj) =
    match value with
    | :? 'T as v -> Some(v)
    | _ -> None

let createInstanceAs<'T> ty = createInstance ty |> tryCast<'T>

let (|BoolNone|BoolSome|) (hasValue, value) = if hasValue then BoolSome(value) else BoolNone

let isNotNull x = not (isNull x)

type OptionBuilder() =
    member inline _.Bind(m, f) = Option.bind f m
    member inline _.Zero() = None
    member inline _.Yield(x) = Some(x)
    member inline _.YieldFrom(x: _ option) = x
    member inline _.Return(x) = Some(x)
    member inline _.ReturnFrom(x: _ option) = x
    member inline _.Delay(f: unit -> _ option) = f
    member inline _.Run(f: unit -> _ option) = f()
    member inline _.Combine(a, b) = Option.orElseWith b a

let option = OptionBuilder()

type IterBuilder() =
    member inline _.Bind(m, f) = Option.iter f m
    member inline _.Bind(m, f) = List.iter f m
    member inline _.Bind(m, f) = Array.iter f m
    member inline _.Bind(m, f) = Seq.iter f m
    member inline _.Zero() = ()

let iter = IterBuilder()
