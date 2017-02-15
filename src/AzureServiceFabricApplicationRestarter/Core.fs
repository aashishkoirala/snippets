module private Core

type Result<'a> = Success of 'a | Failure of string
type AsyncResult<'a> = Async<Result<'a>>

module Result =
  let map (f:'a->'b) (x:Result<'a>) =
    match x with
    | Success x' -> Success (f x')
    | Failure e -> Failure e
  let wrap (f:unit->'a) =
    try Success (f ()) with ex -> Failure ex.Message

module Async =
  let map (f:'a->'b) (x:Async<'a>) = async {
    let! x' = x
    return f x'
  }

module AsyncResult =
  let map (f:'a->'b) (x:AsyncResult<'a>) = async {
    let! x' = x
    return Result.map f x'
  }
  let bind (f:'a->AsyncResult<'b>) (x:AsyncResult<'a>) = async {
    let! x' = x
    let fx =
      match x' with
      | Success x'' -> f x''
      | Failure e -> async { return Failure e }
    let! fx' = fx
    return fx'
  }
  let wrap (f:unit->Async<'a>) = async {
    try
      let! x = f ()
      return Success x
    with
    | ex -> return Failure ex.Message
  }

type Logger = {
  info: string -> unit
  success: string -> unit
  error: string -> unit
  warning: string -> unit
}

let log writer format = Printf.kprintf writer format

module ConsoleWriter =
  let withColor color (text:string) =
    lock System.Console.Out (fun () ->
      let c = System.Console.ForegroundColor
      System.Console.ForegroundColor <- color
      System.Console.WriteLine text
      System.Console.ForegroundColor <- c )
  let info = withColor System.ConsoleColor.Gray
  let success = withColor System.ConsoleColor.Green
  let warning = withColor System.ConsoleColor.Yellow
  let error = withColor System.ConsoleColor.Red
