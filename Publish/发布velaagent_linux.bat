set version=2.3.51
cd "..\VelaAgent"
dotnet publish -c release -o bin\Release\agent_linuxpublish --self-contained true --runtime linux-x64
"C:\Program Files\WinRAR\winrar.exe" a -ep1 -r %~dp0VelaAgent.Linux.%version%.zip bin\Release\agent_linuxpublish\
