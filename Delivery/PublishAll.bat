set TargetFramework=net7.0
set ApiKey=%1

@echo off
@mkdir "Packaging"
@del "Packaging\*.*" /F /S /Q 1>nul
@echo on

rem ========== Clean ============
dotnet clean ..\Source\Odin.sln --configuration Release --framework %TargetFramework% --verbosity minimal

dotnet test ..\Source\Odin.sln --configuration Release --verbosity minimal


rem ========== Dotnet Pack  ============
dotnet pack ..\Source\Odin.Common\Odin.Common.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.DatabaseDeployment\Odin.DatabaseDeployment.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.Data.SqlScriptsRunner\Odin.Data.SqlScriptsRunner.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.BackgroundProcessing\Odin.BackgroundProcessing.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.Cryptography\Odin.Cryptography.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.RemoteFiles.Abstractions\Odin.RemoteFiles.Abstractions.csproj --configuration Release --output "Packaging" --verbosity minimal
dotnet pack ..\Source\Odin.RemoteFiles\Odin.RemoteFiles.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.RemoteFiles.SFTP\Odin.RemoteFiles.SFTP.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.Messaging\Odin.Messaging.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.Email\Odin.Email.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.Notifications\Odin.Notifications.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ..\Source\Odin.Web\Odin.Web.csproj --configuration Release --output "Packaging" --verbosity minimal 


rem ========== Nuget Publish ============
dotnet nuget push Packaging\Odin.Common.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.DatabaseDeployment.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.BackgroundProcessing.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.Data.SqlScriptsRunner.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.Cryptography.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.RemoteFiles.Abstractions.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.RemoteFiles.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.RemoteFiles.SFTP.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.Messaging.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.Email.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.Notifications.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging\Odin.Web.*.nupkg --api-key %ApiKey% --source https://api.nuget.org/v3/index.json
