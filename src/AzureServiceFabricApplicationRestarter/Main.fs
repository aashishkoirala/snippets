module private Main

open Core

let getPackagesForRestartAsync getDeployedPackagesAsync application nodeNames =
  let foldResults acc r = acc @ match r with Success x -> (Seq.toList x) | _ -> []
  let collapseResults rs = rs |> Seq.toList |> List.fold foldResults [] |> Array.ofList
  let resultify list =
    match Seq.isEmpty list with
    | true -> Failure "No nodes found with the application"
    | _ -> Success list
  nodeNames
  |> Seq.map (fun n -> getDeployedPackagesAsync n application)
  |> Async.Parallel
  |> Async.map (collapseResults >> resultify)

let restartPackagesAsync restartPackageAsync packages =
  let foldResults acc r = acc @ [ (match r with Success _ -> true | _ -> false) ]
  let collapseResults rs = rs |> Seq.toList |> List.fold foldResults [] |> Array.ofList
  let hasNoFalse list =
    match list |> Seq.exists ((=) false) with
    | true -> Failure "Restart failed in at least one node"
    | _ -> Success ()
  packages
  |> Seq.map restartPackageAsync
  |> Async.Parallel
  |> Async.map (collapseResults >> hasNoFalse)

let restartImpl endpoint thumbprint application =
  let logger = {
    info = ConsoleWriter.info
    success = ConsoleWriter.success
    warning = ConsoleWriter.warning
    error = ConsoleWriter.error
  }
  use client = Client.connect endpoint thumbprint
  let deps = (client, logger)

  let getNodeNamesAsync =
    deps
    ||> Client.getNodeNamesAsync
  let getPackagesForRestartAsync' =
    deps
    ||> Client.getDeployedPackagesAsync
    |> (fun a b -> (b, a)) application
    ||> getPackagesForRestartAsync
    |> AsyncResult.bind
  let restartPackagesAsync' =
    deps
    ||> Client.restartPackageAsync
    |> restartPackagesAsync
    |> AsyncResult.bind

  let result =
    getNodeNamesAsync
    |> getPackagesForRestartAsync'
    |> restartPackagesAsync'
    |> Async.RunSynchronously

  match result with
  | Success _ -> log logger.success "Restart was successful."
  | Failure e -> log logger.error "%s" e

  result

let restart input =
  try
    match input with
    | Success (e, t, a) -> restartImpl e t a
    | Failure e ->
      eprintfn "%s" e
      Failure e
  with
  | ex ->
    eprintfn "%s" ex.Message
    Failure ex.Message

[<EntryPoint>]
let main (argv:string[])=
  match argv with
  | [|endpoint; thumbprint; application|] -> Success (endpoint, thumbprint, application)
  | _ -> Failure "Invalid parameters, specify as 'endpoint thumbprint application'."
  |> restart
  |> (fun r -> match r with Success _ -> 0 | _ -> 1)
