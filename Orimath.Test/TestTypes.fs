namespace RightArrow

type TestContext<'Args, 'Result> =
    { Args: 'Args
      Result: 'Result }

type Testing<'Args, 'Result> =
    { Args: 'Args
      GetResult: unit -> 'Result }
    with
    member this.ToContext() = {
        Args = this.Args
        Result = this.GetResult()
    }

    member this.ToContextIgnored() = {
        Args = this.Args
        Result = ()
    }

type Assertion<'Args, 'Result, 'Value, 'T> = TestContext<'Args, 'Result> -> 'Value -> 'T

type Assertion<'Args, 'Result, 'Value> = TestContext<'Args, 'Result> -> 'Value -> 'Value

type Assertion<'Args, 'Result> = TestContext<'Args, 'Result> -> 'Result -> unit

type ExceptionAssertion = { ObserveException: (unit -> unit) -> unit }

type internal Create =
    static member inline Assertion4<'Args, 'Result, 'Value, 'T>(f) : Assertion<'Args, 'Result, 'Value, 'T> = f
    static member inline Assertion3<'Args, 'Result, 'T>(f) : Assertion<'Args, 'Result, 'T> = f

exception TestFailureException of args: obj * result: obj * message: string
    with
    static member FormatValue(value: obj) =
        match value with
        | null -> "<null>"
        | other -> sprintf "%A" other

    override this.Message =
        let args = TestFailureException.FormatValue(this.args)
        let result = TestFailureException.FormatValue(this.result)
        $"
Parameter : %s{args}
Result    : %s{result}
%s{this.message}"

[<AbstractClass; Sealed>]
type Testing =
    static member CreateTest(f, arg: 'T) : Testing<_, 'Result> = {
        Args = arg
        GetResult = fun () -> f arg
    }

    static member CreateTest(f, (arg1: 'T1, arg2: 'T2)) : Testing<_, 'Result> = {
        Args = arg1, arg2
        GetResult = fun () -> f arg1 arg2
    }

    static member CreateTest( f, (arg1: 'T1, arg2: 'T2, arg3: 'T3)) : Testing<_, 'Result> = {
        Args = arg1, arg2, arg3
        GetResult = fun () -> f arg1 arg2 arg3
    }

    static member CreateTest(f, (arg1: 'T1, arg2: 'T2, arg3: 'T3, arg4: 'T4)) : Testing<_, 'Result> = {
        Args = arg1, arg2, arg3, arg4
        GetResult = fun () -> f arg1 arg2 arg3 arg4
    }

    static member CreateTest( f, (arg1: 'T1, arg2: 'T2, arg3: 'T3, arg4: 'T4, arg5: 'T5))
        : Testing<_, 'Result> = {
        Args = arg1, arg2, arg3, arg4, arg5
        GetResult = fun () -> f arg1 arg2 arg3 arg4 arg5
    }

    static member CreateTest(f, (arg1: 'T1, arg2: 'T2, arg3: 'T3, arg4: 'T4, arg5: 'T5, arg6: 'T6))
        : Testing<_, 'Result> = {
        Args = arg1, arg2, arg3, arg4, arg5, arg6
        GetResult = fun () -> f arg1 arg2 arg3 arg4 arg5 arg6
    }

    static member CreateTest(f, (arg1: 'T1, arg2: 'T2, arg3: 'T3, arg4: 'T4, arg5: 'T5, arg6: 'T6, arg7: 'T7))
        : Testing<_, 'Result> = {
        Args = arg1, arg2, arg3, arg4, arg5, arg6, arg7
        GetResult = fun () -> f arg1 arg2 arg3 arg4 arg5 arg6 arg7
    }

    static member CreateTest
        (f, (arg1: 'T1, arg2: 'T2, arg3: 'T3, arg4: 'T4, arg5: 'T5, arg6: 'T6, arg7: 'T7, arg8: 'T8))
        : Testing<_, 'Result> = {
        Args = arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8
        GetResult = fun () -> f arg1 arg2 arg3 arg4 arg5 arg6 arg7 arg8
    }

[<AbstractClass; Sealed>]
type AssertionExecutor =
    static member Assert(testing: Testing<'Args, 'Result>, ex: ExceptionAssertion) =
        ex.ObserveException (testing.GetResult >> ignore)

    static member Assert(testing: Testing<'Args, 'Result>, assertion: Assertion<'Args, 'Result>) =
        let result = testing.ToContext()
        assertion result result.Result

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2 = context.Result
        a1 context v1
        a2 context v2

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2 * 'R3>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>,
          a3: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2, v3 = context.Result
        a1 context v1
        a2 context v2
        a3 context v3

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2 * 'R3 * 'R4>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>,
          a3: Assertion<_, _, _, _>,
          a4: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2, v3, v4 = context.Result
        a1 context v1
        a2 context v2
        a3 context v3
        a4 context v4

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2 * 'R3 * 'R4 * 'R5>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>,
          a3: Assertion<_, _, _, _>,
          a4: Assertion<_, _, _, _>,
          a5: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2, v3, v4, v5 = context.Result
        a1 context v1
        a2 context v2
        a3 context v3
        a4 context v4
        a5 context v5

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2 * 'R3 * 'R4 * 'R5 * 'R6>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>,
          a3: Assertion<_, _, _, _>,
          a4: Assertion<_, _, _, _>,
          a5: Assertion<_, _, _, _>,
          a6: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2, v3, v4, v5, v6 = context.Result
        a1 context v1
        a2 context v2
        a3 context v3
        a4 context v4
        a5 context v5
        a6 context v6

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2 * 'R3 * 'R4 * 'R5 * 'R6 * 'R7>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>,
          a3: Assertion<_, _, _, _>,
          a4: Assertion<_, _, _, _>,
          a5: Assertion<_, _, _, _>,
          a6: Assertion<_, _, _, _>,
          a7: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2, v3, v4, v5, v6, v7 = context.Result
        a1 context v1
        a2 context v2
        a3 context v3
        a4 context v4
        a5 context v5
        a6 context v6
        a7 context v7

    static member Assert
        (testing: Testing<'Args, 'R1 * 'R2 * 'R3 * 'R4 * 'R5 * 'R6 * 'R7 * 'R8>,
         (a1: Assertion<_, _, _, _>,
          a2: Assertion<_, _, _, _>,
          a3: Assertion<_, _, _, _>,
          a4: Assertion<_, _, _, _>,
          a5: Assertion<_, _, _, _>,
          a6: Assertion<_, _, _, _>,
          a7: Assertion<_, _, _, _>,
          a8: Assertion<_, _, _, _>)
        ) =
        let context = testing.ToContext()
        let v1, v2, v3, v4, v5, v6, v7, v8 = context.Result
        a1 context v1
        a2 context v2
        a3 context v3
        a4 context v4
        a5 context v5
        a6 context v6
        a7 context v7
        a8 context v8
