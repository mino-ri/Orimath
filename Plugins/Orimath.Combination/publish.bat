dotnet publish . -c Release -o ../../Release/Plugins -p:PublishSingleFile=true -p:RuntimeIdentifier=win-x64 -p:SelfContained=false
pause