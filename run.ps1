start-job { cd "$($args[0])\PsGui"; dotnet run } -arg (pwd).path
start-job { cd "$($args[0])\psgui-web\src"; npm run serve } -arg (pwd).path