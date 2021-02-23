namespace Orimath.Core
open System
open System.Collections.Generic
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type UndoItem<'Tag, 'Operation> =
    { Tag: 'Tag
      Operations: 'Operation[] }


type UndoStack<'Tag, 'Operation>() =
    let mutable changeBlockDeclared = false
    let mutable changeBlockDisabled = false
    let changeBlockOprs = ResizeArray()
    let undoOprStack = Stack()
    let redoOprStack = Stack()
    let canUndo = Prop.value false
    let canRedo = Prop.value false
    let onUndo = Subject()
    let onRedo = Subject()

    member val CanUndo = Prop.asGet canUndo
    member val CanRedo = Prop.asGet canRedo
    member val OnUndo = Observable.asObservable onUndo
    member val OnRedo = Observable.asObservable onRedo
    member _.ChangeBlockDeclared = changeBlockDeclared
    member _.ChangeBlockDisabled = changeBlockDisabled

    member internal _.PushUndoOpr(opr: 'Operation) =
        if not changeBlockDisabled then changeBlockOprs.Add(opr)

    member internal _.UpdateCanUndo() =
        canUndo .<- (undoOprStack.Count > 0)
        canRedo .<- (redoOprStack.Count > 0)

    member this.BeginChange(tag: 'Tag) =
        if changeBlockDisabled then invalidOp "変更が無効化されています。"
        if changeBlockDeclared then invalidOp "既に変更ブロックが定義されています。"
        redoOprStack.Clear()
        changeBlockDeclared <- true
        changeBlockOprs.Clear()
        { new IDisposable with
            member _.Dispose() =
                if changeBlockOprs.Count > 0 then
                    undoOprStack.Push({
                        Tag = tag
                        Operations = changeBlockOprs.ToArray()
                    })
                changeBlockDeclared <- false
                this.UpdateCanUndo()
        }

    member _.UndoTags = seq { for s in undoOprStack -> s.Tag }

    member _.RedoTags = seq { for s in redoOprStack -> s.Tag }

    /// 一次的に変更ブロックを無効にします。
    member this.DisableChangeBlock() =
        if changeBlockDeclared then invalidOp "変更ブロックが定義されているため、Undoを開始できません。"
        changeBlockDeclared <- true
        changeBlockDisabled <- true
        { new IDisposable with
            member _.Dispose() =
                changeBlockDeclared <- false
                changeBlockDisabled <- false
                this.UpdateCanUndo()
        }

    member this.ClearUndoStack() =
        undoOprStack.Clear()
        redoOprStack.Clear()
        this.UpdateCanUndo()

    member this.Undo() =
        if this.CanUndo.Value && not this.ChangeBlockDeclared then
            use __ = this.DisableChangeBlock()
            let oprBlock = undoOprStack.Pop()
            redoOprStack.Push(oprBlock)
            onUndo.OnNext(oprBlock)

    member this.Redo() =
        if this.CanRedo.Value && not this.ChangeBlockDeclared then
            use __ = this.DisableChangeBlock()
            let oprBlock = redoOprStack.Pop()
            undoOprStack.Push(oprBlock)
            onRedo.OnNext(oprBlock)
