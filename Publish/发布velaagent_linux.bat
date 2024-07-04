set version=2.3.16
set versionNew=2.3.17
del %~dp0VelaAgent.Linux.%version%.zip
cd "..\VelaAgent"
dotnet publish -c release -o bin\Release\agent_linuxpublish --self-contained true --runtime linux-x64
"C:\Program Files\WinRAR\winrar.exe" a -ep1 -r %~dp0VelaAgent.Linux.%version%.zip bin\Release\agent_linuxpublish\
copy /Y %~dp0VelaAgent.Linux.%version%.zip "C:\Users\jack\OneDrive - MUSE\JMS\VelaAgent.Linux.%version%.zip"
@echo wait to upload complted
pause
ren "C:\Users\jack\OneDrive - MUSE\JMS\VelaAgent.Linux.%version%.zip" VelaAgent.Linux.%versionNew%.zip
@echo modify name successed
pause