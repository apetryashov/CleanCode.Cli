@echo off

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true

del .\bin\Release\netcoreapp3.1\win-x64\publish\clean-code.pdb

7z a clean-code.zip .\bin\Release\netcoreapp3.1\win-x64\publish\*