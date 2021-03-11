[<AutoOpen>]
module RightArrow.TestOperator

let inline private createTest (_: ^C) (f: ^Func) (args: ^Args) =
    ((^C or ^Func) : (static member CreateTest : ^Func * ^Args -> Testing< ^Args, ^Result >) f, args)

let inline test (testFunc: ^``('Args -> 'Result)``) (args: ^Args) : Testing< ^Args, ^Result > =
    createTest (Unchecked.defaultof<Testing>) testFunc args

let inline private assertCore (_: ^C) (testing: Testing< ^Args, ^Result>) (source: ^Source) =
    ((^C or ^Source) : (static member Assert : Testing< ^Args, ^Result> * ^Source -> unit) testing, source)

let inline ( ==> ) (testing: Testing< ^Args, ^Result >) (assertion: ^``Assertion<'Args, 'Result>``) =
    assertCore (Unchecked.defaultof<AssertionExecutor>) testing assertion
