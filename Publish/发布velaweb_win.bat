set version=2.8.6
set versionNew=2.8.7
del %~dp0VelaWeb.win.%version%.zip
cd "..\VelaWeb.Server"
dotnet publish -c release -o bin\Release\winpublish --self-contained true --runtime win-x64
"C:\Program Files\WinRAR\winrar.exe" a -ep1 -r %~dp0VelaWeb.win.%version%.zip bin\Release\winpublish\
copy /Y %~dp0VelaWeb.win.%version%.zip "C:\Users\89687\OneDrive - MUSE\JMS\VelaWeb.win.%version%.zip"
@echo wait to upload complted
pause
ren "C:\Users\89687\OneDrive - MUSE\JMS\VelaWeb.win.%version%.zip" VelaWeb.win.%versionNew%.zip
@echo modify name successed
pause