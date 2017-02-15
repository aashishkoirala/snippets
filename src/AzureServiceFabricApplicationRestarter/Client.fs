module private Client

open System
open System.Fabric
open System.Security.Cryptography.X509Certificates
open Core

type CodePackage = {
  NodeName: string
  ApplicationName: string
  CodePackageName: string
  ServiceManifestName: string
}

let connect endpoint thumbprint =
  let creds =
    X509Credentials (
      FindType = X509FindType.FindByThumbprint,
      FindValue = thumbprint,
      StoreLocation = StoreLocation.LocalMachine,
      StoreName = "My"
    )
  creds.RemoteCertThumbprints.Add thumbprint
  new FabricClient (creds, [|endpoint|])

let getNodeNamesAsync (client:FabricClient) logger =
  AsyncResult.wrap (fun () ->
    log logger.info "Getting list of nodes..."
    client.QueryManager.GetNodeListAsync ()
    |> Async.AwaitTask
    |> Async.map (fun ns ->
      log logger.success "Found %d nodes." ns.Count
      ns |> Seq.map (fun n -> n.NodeName)))

let getDeployedPackagesAsync (client:FabricClient) logger node application =
  AsyncResult.wrap (fun () ->
    log logger.info "Checking for instances of application %s in node %s..." application node
    (node, Uri application)
    |> client.QueryManager.GetDeployedCodePackageListAsync
    |> Async.AwaitTask
    |> Async.map (fun ps ->
      log logger.success "Found %i instances in node %s." ps.Count node
      ps
      |> Seq.map (fun p ->
          let inst = {
            ApplicationName = application
            NodeName = node
            CodePackageName = p.CodePackageName
            ServiceManifestName = p.ServiceManifestName
          }
          log logger.info "Instance: %A" inst
          inst )))

let restartPackageAsync (client:FabricClient) logger (package:CodePackage) =
  AsyncResult.wrap (fun () ->
    log logger.info "Restarting application %s in node %s..." package.ApplicationName package.NodeName
    (package.NodeName, Uri package.ApplicationName, package.ServiceManifestName, package.CodePackageName, 0L, CompletionMode.Verify)
    |> client.FaultManager.RestartDeployedCodePackageAsync
    |> Async.AwaitTask )
  |> Async.map (fun r ->
    match r with
    | Success _ ->
      log logger.success "Restarted application %s in node %s." package.ApplicationName package.NodeName
      Success ()
    | Failure e -> Failure e)
