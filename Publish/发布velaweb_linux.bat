set version=2.8.1
set versionNew=2.8.2
del %~dp0VelaWeb.Linux.%version%.zip
cd "..\VelaWeb.Server"
dotnet publish -c release -o bin\Release\linuxpublish --self-contained true --runtime linux-x64
"C:\Program Files\WinRAR\winrar.exe" a -ep1 -r %~dp0VelaWeb.Linux.%version%.zip bin\Release\linuxpublish\
copy /Y %~dp0VelaWeb.Linux.%version%.zip "C:\Users\jack\OneDrive - MUSE\JMS\VelaWeb.Linux.%version%.zip"
@echo wait to upload complted
pause
ren "C:\Users\jack\OneDrive - MUSE\JMS\VelaWeb.Linux.%version%.zip" VelaWeb.Linux.%versionNew%.zip
@echo modify name successed
pause