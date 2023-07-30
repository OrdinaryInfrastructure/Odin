TargetFramework=net7.0
ApiKey=oy2czxzguy43uewudv7qm64cga3nsq4ubhxmdzeibqa4l4

sudo rm -f Packaging/* 

read -p "Press any key to build and run tests..."

# ========== Clean ============
dotnet clean ../Source/Odin.sln --configuration Release --framework $TargetFramework --verbosity normal

dotnet test ../Source/Odin.sln --configuration Release --verbosity normal

read -p "If Tests have passed, press any key to dotnet pack .nukpg files"

# ========== Dotnet Pack  ============
dotnet pack ../Source/Odin.Common/Odin.Common.csproj --configuration Release --output "Packaging" --verbosity normal 
dotnet pack ../Source/Odin.Data.SqlScriptsRunner/Odin.Data.SqlScriptsRunner.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.BackgroundProcessing.Abstractions/Odin.BackgroundProcessing.Abstractions.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.BackgroundProcessing/Odin.BackgroundProcessing.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.BackgroundProcessing.Hangfire/Odin.BackgroundProcessing.Hangfire.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.Cryptography/Odin.Cryptography.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.RemoteFiles.Abstractions/Odin.RemoteFiles.Abstractions.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.RemoteFiles/Odin.RemoteFiles.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.RemoteFiles.SFTP/Odin.RemoteFiles.SFTP.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.Messaging/Odin.Messaging.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.Email/Odin.Email.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.Email.Mailgun/Odin.Email.Mailgun.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.Notifications/Odin.Notifications.csproj --configuration Release --output "Packaging" --verbosity minimal 
dotnet pack ../Source/Odin.Web/Odin.Web.csproj --configuration Release --output "Packaging" --verbosity minimal 

read -p "Pack finished. Press any key to publish to Nuget..."

# ========== Nuget Publish ============
dotnet nuget push Packaging/Odin.Common.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.BackgroundProcessing.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.Data.SqlScriptsRunner.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.Cryptography.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.RemoteFiles.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.Messaging.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.Email.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.Notifications.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
dotnet nuget push Packaging/Odin.Web.*.nupkg --api-key $ApiKey --source https://api.nuget.org/v3/index.json
