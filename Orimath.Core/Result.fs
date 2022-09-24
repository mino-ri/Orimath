module Orimath.Result

let forceOk (result: Result<'Ok, 'Error>) =
    match result with
    | Ok(ok) -> ok
    | Error _ -> failwith "値はOkでなければなりません。"

let toList (result: Result<'Ok, 'Error>) =
    match result with
    | Ok(result) -> [ result ]
    | Error _ -> []

let isOk (result: Result<'Ok, 'Error>) =
    match result with
    | Ok _ -> true
    | Error _ -> false


let isError (result: Result<'Ok, 'Error>) =
    match result with
    | Ok _ -> false
    | Error _ -> true
