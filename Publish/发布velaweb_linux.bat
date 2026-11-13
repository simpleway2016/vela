set version=2.8.22
cd "..\VelaWeb.Server"
dotnet publish -c release -o bin\Release\linuxpublish --self-contained true --runtime linux-x64
"C:\Program Files\WinRAR\winrar.exe" a -ep1 -r %~dp0VelaWeb.Linux.%version%.zip bin\Release\linuxpublish\

pause