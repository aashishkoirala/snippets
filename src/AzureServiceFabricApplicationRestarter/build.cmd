@echo off
rd /s /q bin
md bin
nuget install Microsoft.ServiceFabric -Version 5.0.135 -OutputDirectory packages
fsc -r packages\Microsoft.ServiceFabric.5.0.135\lib\net45\System.Fabric.dll -o bin\RestartAzureServiceFabricApplication.exe Core.fs Client.fs Main.fs
