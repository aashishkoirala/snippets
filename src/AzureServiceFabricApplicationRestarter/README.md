### Azure Service Fabric Application Restarter
##### by [Aashish Koirala](https://aashishkoirala.github.io)
---

I built this as an exercise in real-world F# while trying to use concepts such as *map*, *bind* and *Railway-Oriented Programming*. It lets you connect to an Azure Service Fabric Cluster and restart a given application on all relevant nodes.

To build, run the **build.cmd** command (you need **nuget** and the F# compiler **fsc** on the path). The build will produce an EXE **RestartAzureServiceFabricApplication.exe** that you can then run on the command line as follows:

**RestartAzureServiceFabricApplication** *endpoint* *thumprint* *application*

Here, *endpoint* is the TCP administration endpoint forthe cluster, the *thumbprint* corresponds to that of the certificate that the cluster is secured with (you need to have the certificate loaded on your Personal store for this to work), and *application* is the "fabric:/" style URL for the application to restart.

For example:

	RestartAzureServiceFabricApplication mycluster.mysite.com:19000 d0968a748299c5f875d1936885a0708dd5df7986 fabric:/MyApp
