 param(   
    [string]$ApiKey
 )


if(!(Test-Path -Path ".\packages\NuGet.exe" -PathType Leaf)){
    Write-Error "NuGet.exe is missing" -ErrorAction Stop 
}

$packages = Get-Item .\packages\*.nupkg

foreach($package in $packages){
  & .\packages\NuGet.exe push "$package" -ApiKey "$ApiKey" -Source https://api.nuget.org/v3/index.json 
}