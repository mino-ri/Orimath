[<AutoOpen>]
module RightArrow.AssertOperator
open System

let inline formatValue value = TestFailureException.FormatValue(value)

// Generic Utilities
let inline private item1 (tuple: ^Tuple) : ^T = (^Tuple : (member Item1 : ^T) tuple)
let inline private item2 (tuple: ^Tuple) : ^T = (^Tuple : (member Item2 : ^T) tuple)
let inline private item3 (tuple: ^Tuple) : ^T = (^Tuple : (member Item3 : ^T) tuple)
let inline private item4 (tuple: ^Tuple) : ^T = (^Tuple : (member Item4 : ^T) tuple)
let inline private item5 (tuple: ^Tuple) : ^T = (^Tuple : (member Item5 : ^T) tuple)
let inline private item6 (tuple: ^Tuple) : ^T = (^Tuple : (member Item6 : ^T) tuple)
let inline private item7 (tuple: ^Tuple) : ^T = (^Tuple : (member Item7 : ^T) tuple)
let inline private item8 (tuple: ^Tuple) : ^T = (^Tuple : (member Rest : ^T) tuple)

let failTest<'Args, 'Result, 'Value, 'T> message =
    Create.Assertion4<'Args, 'Result, 'Value, 'T>(fun ctx _ ->
        raise (TestFailureException(box ctx.Args, box ctx.Result, message)))

let failTestWithValue message (actual: obj) =
    failTest<'Args, 'Result, 'Value, 'T> $"%s{message}
Actual   : %s{formatValue actual}"

let failTestExpected assertName (expected: obj) (actual: obj) =
    failTest<'Args, 'Result, 'Value, 'T> $"Assert.%s{assertName} is faled.
Expected : %s{formatValue expected}
Actual   : %s{formatValue actual}"

// basic values
let it<'Args, 'Result, 'T> = Create.Assertion3<'Args, 'Result, 'T>(fun _ v -> v)

let res<'Args, 'Result, 'Value> = Create.Assertion4<'Args, 'Result, 'Value, 'Result>(fun ctx _ -> ctx.Result)
let inline res1 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item1 ctx.Result
let inline res2 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item2 ctx.Result
let inline res3 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item3 ctx.Result
let inline res4 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item4 ctx.Result
let inline res5 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item5 ctx.Result
let inline res6 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item6 ctx.Result
let inline res7 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item7 ctx.Result
let inline res8 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item8 ctx.Result

let arg<'Args, 'Result, 'Value> = Create.Assertion4<'Args, 'Result, 'Value, 'Args>(fun ctx _ -> ctx.Args)
let inline arg1 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item1 ctx.Result
let inline arg2 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item2 ctx.Result
let inline arg3 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item3 ctx.Result
let inline arg4 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item4 ctx.Result
let inline arg5 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item5 ctx.Result
let inline arg6 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item6 ctx.Result
let inline arg7 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item7 ctx.Result
let inline arg8 (ctx: TestContext< ^Args, ^Result >) (_: ^Value) : ^T = item8 ctx.Result

let inline private runTest (testResult: Assertion<'Args, 'Result, 'Value, 'T>) ctx value =
    testResult ctx value

let bind (mapping: 'T -> _) testResult = Create.Assertion4<'Args, 'Result, 'Value, 'U>(fun ctx value ->
    runTest (mapping (runTest testResult ctx value)) ctx value)

let map (mapping: 'T -> 'U) testResult = Create.Assertion4<'Args, 'Result, 'Value, 'U>(fun ctx value ->
    mapping (runTest testResult ctx value))

type AssertionBuilder() =
    member inline _.Bind(f, mapping) = bind mapping f
    member inline _.Return(value: 'T): Assertion<'Args, 'Result, 'Value, 'T> = fun _ _ -> value
    member inline _.ReturnFrom(value: Assertion<'Args, 'Result, 'Value, 'T>) = value
    member inline _.Zero(): Assertion<'Args, 'Result, 'Value, unit> = fun _ _ -> ()
    member inline _.Combine(before: Assertion<'Args, 'Result, 'Value, unit>, after: Assertion<_, _, _, 'T>)
        : Assertion<_, _, _, 'T> =
        fun ctx value -> before ctx value; after ctx value
        
let assert' = AssertionBuilder()

let any<'Args, 'Result, 'Value> = Create.Assertion4<'Args, 'Result, 'Value, unit>(fun _ _ -> ())

let replaceWhenNot pred replacing assersion =
    assert' {
        let! value = assersion
        if not (pred value) then return! replacing value
    }

let replaceWhenNot2 pred replacing assertion1 assertion2 =
    assert' {
        let! value1 = assertion1
        let! value2 = assertion2
        if not (pred value1 value2) then return! replacing value1 value2
    }

let inline lift name predicate (actual: Assertion<'Args, 'Result, 'Value, 'T>) =
    replaceWhenNot predicate (failTestWithValue name) actual

let inline liftExpected name predicate (expected: 'T) (actual: Assertion<'Args, 'Result, 'Value, 'T>) =
    replaceWhenNot (predicate expected) (failTestExpected name expected) actual

let inline liftExpected2 name (predicate: 'T -> 'T -> bool) expected (actual: Assertion<'Args, 'Result, 'Value, 'T>) =
    replaceWhenNot2 predicate (failTestExpected name) expected actual

let ( ^|> ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) predicate =
    lift "Condition" predicate actual

let ( ^= ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) (expected: 'T) =
    liftExpected "Equal" (=) expected actual

let ( ^<> ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) (expected: 'T) =
    liftExpected "NotEqual" (<>) expected actual

let ( ^=@ ) (actual: Assertion<'Args, 'Result, 'Value, seq<'T>>) (expected: seq<'T>) =
    liftExpected "Equal" (Seq.forall2 (=)) expected actual

let ( ^<>@ ) (actual: Assertion<'Args, 'Result, 'Value, seq<'T>>) (expected: seq<'T>) =
    liftExpected "Equal" (Seq.forall2 (=) >> (<<) not) expected actual

let ( ^< ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) (expected: 'T) =
    liftExpected "LessThan" (>) expected actual

let ( ^> ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) (expected: 'T) =
    liftExpected "GreaterThan" (<) expected actual

let ( ^<= ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) (expected: 'T) =
    liftExpected "LessThanOrEqual" (>=) expected actual

let ( ^>= ) (actual: Assertion<'Args, 'Result, 'Value, 'T>) (expected: 'T) =
    liftExpected "GreaterThanOrEqual" (<=) expected actual

let ( <&> )
    (assersion1: Assertion<'Args, 'Result, 'Value, unit>)
    (assersion2: Assertion<'Args, 'Result, 'Value, unit>) =
    Create.Assertion4(fun ctx value -> assersion1 ctx value; assersion2 ctx value)

let ( <|> )
    (assersion1: Assertion<'Args, 'Result, 'Value, unit>)
    (assersion2: Assertion<'Args, 'Result, 'Value, unit>) =
    Create.Assertion4(fun ctx value -> try assersion1 ctx value with _ -> assersion2 ctx value)

let startsWith expected (actual: Assertion<'Args, 'Result, 'Value, string>) =
    (expected, actual) ||> liftExpected "StartsWith" (fun ex ac -> ac.StartsWith(ex))

let endsWith expected (actual: Assertion<'Args, 'Result, 'Value, string>) =
    (expected, actual) ||> liftExpected "StartsWith" (fun ex ac -> ac.EndsWith(ex))

let inline equal expected: Assertion<'Args, 'Result, 'Value, unit> = it ^= expected

let inline notEqual expected: Assertion<'Args, 'Result, 'Value, unit> = it ^<> expected

let inline equalSeq expected: Assertion<'Args, 'Result, seq<'Value>, unit> = it ^=@ expected

let inline notEqualSeq expected: Assertion<'Args, 'Result, seq<'Value>, unit> = it ^<>@ expected

[<RequiresExplicitTypeArguments>]
let throws<'T when 'T :> exn> =
    { ObserveException = fun testing -> try testing() with ex when ex.GetType() = typeof<'T> -> () }
 
let throwsAny<'T when 'T :> exn> =
    { ObserveException = fun testing -> try testing() with :? 'T -> () }
