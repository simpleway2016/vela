set version=2.8.18
set versionNew=2.8.19
del %~dp0VelaWeb.Linux.%version%.zip
cd "..\VelaWeb.Server"
dotnet publish -c release -o bin\Release\linuxpublish --self-contained true --runtime linux-x64
"C:\Program Files\WinRAR\winrar.exe" a -ep1 -r %~dp0VelaWeb.Linux.%version%.zip bin\Release\linuxpublish\
copy /Y %~dp0VelaWeb.Linux.%version%.zip "C:\Users\89687\OneDrive - MUSE\JMS\VelaWeb.Linux.%version%.zip"
@echo wait to upload complted
pause
ren "C:\Users\89687\OneDrive - MUSE\JMS\VelaWeb.Linux.%version%.zip" VelaWeb.Linux.%versionNew%.zip
ren %~dp0VelaWeb.Linux.%version%.zip VelaWeb.Linux.%versionNew%.zip
@echo modify name successed
pause