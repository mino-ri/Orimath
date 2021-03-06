﻿namespace Orimath.Basics.View.ViewModels
open Orimath.Basics
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type NewPaperDialogViewModel(messenger: IMessenger, dispatcher: IDispatcher, executor: NewPaperExecutor) =
    inherit NotifyPropertyChanged()
    let isSquareSelected = Prop.value false
    let isRectangleSelected = Prop.value false
    let isPolygonSelected = Prop.value false
    let width = Prop.value 1.0
    let height = Prop.value 1.0
    let numberOfPolygon = Prop.value 3
    do
        match executor.NewPaperType with
        | Square ->
            isSquareSelected .<- true
        | Rectangle(w, h) ->
            isRectangleSelected .<- true
            width .<- w
            height .<- h
        | RegularPolygon(number) ->
            isPolygonSelected .<- true
            numberOfPolygon .<- number
    do isSquareSelected.Add(fun b -> if b then executor.NewPaperType <- Square)
    do (isRectangleSelected, width, height)
       |||> Prop.map3 (fun b w h -> b, Rectangle(w, h))
       |> Observable.add (fun (b, pt) -> if b then executor.NewPaperType <- pt)
    do (isPolygonSelected, numberOfPolygon)
       ||> Prop.map2(fun b n -> b, RegularPolygon(n))
       |> Observable.add (fun (b, pt) -> if b then executor.NewPaperType <- pt)

    member val SquareText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Square}Square")
    member val RectangleText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Rectangle}Rectangle")
    member val WidthText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Width}Width: ")
    member val HeightText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Height}Height: ")
    member val PolygonText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Polygon}Polygon")
    member val RegularText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Regular}Regular")
    member val GonText = messenger.LocalizeWord("{basic/Dialog.NewPaper.Gon}-gon")

    member _.IsSquareSelected = isSquareSelected
    member _.IsRectangleSelected = isRectangleSelected
    member _.IsPolygonSelected = isPolygonSelected
    member _.Width = width |> Prop.mapBoth id (fun w -> if w <= 0.0 then 1.0 else w)
    member _.Height = height |> Prop.mapBoth id (fun h -> if h <= 0.0 then 1.0 else h)
    member _.NumberOfPolygon = numberOfPolygon

    member val ExecuteCommand = Prop.ctrue |> Prop.command (fun _ ->
        dispatcher.Background {
            executor.NewPaper()
            dispatcher.UI.Invoke(messenger.CloseDialog)
        })

    member _.CloseCommand = messenger.CloseDialogCommand