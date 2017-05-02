# Overview

This is a demo application created to show how SQL Server can operate on OpenShift Container Platform. 
 
## Project Content

```
SQL on Linux-Docker-OpenShift
│   README.md                           <--- Contains project overview and instructions to run SQL Server 
│
└───Lab-Scripts
│   │   01-cluster-up.bat               <--- Script to run OpenShift cluster on Windows Lab 
│   │   02-port-forward.bat             <--- Script to forward MSSQL Container port
│   │   03-PersonDirectory-db-script.sql<--- Script to create Database and Table required by application
│   │   04-cluster-down.bat             <--- Script to stop OpenShift cluster
│   
└───src
│   │   dotnet-core-mssql-app           <--- Sample .NET Core application
│   
└───template
│   │   dotnet-mssql-persistent.json    <--- OpenShift application template 
│    
```

