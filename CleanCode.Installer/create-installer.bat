@echo off

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true

move .\bin\Release\netcoreapp3.1\win-x64\publish\clean-code.installer.exe .
