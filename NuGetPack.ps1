 param(   
    [switch]$forceDownload,
    [string]$packageVersion = $null
 )

if($packageVersion -eq ""){
    $releases = & git tag -l v*
    if($releases -eq $null -or ($releases -eq '')){
        $packageVersion = "0.1.0"
    } 
    else {
        $packageVersion = ($releases | Sort-Object -Descending | Select -First 1).TrimStart('v')
    }
}

if(!(Test-Path -Path ".\packages")){
    New-Item -Name ".\packages" -ItemType Directory
}

if(!(Test-Path -Path ".\packages\NuGet.exe" -PathType Leaf) -or $forceDownload){
    Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile ".\packages\NuGet.exe"
}

& ".\packages\NuGet.exe" pack ".\AlbanianXrm.XrmExtensions.Source\AlbanianXrm.XrmExtensions.8.nuspec" -OutputDirectory ".\packages" -Version $packageVersion
& ".\packages\NuGet.exe" pack ".\AlbanianXrm.XrmExtensions.Source\AlbanianXrm.XrmExtensions.9.nuspec" -OutputDirectory ".\packages" -Version $packageVersion
& ".\packages\NuGet.exe" pack ".\AlbanianXrm.XrmPluginExtensions.Source\AlbanianXrm.XrmPluginExtensions.8.nuspec" -OutputDirectory ".\packages" -Version $packageVersion
& ".\packages\NuGet.exe" pack ".\AlbanianXrm.XrmPluginExtensions.Source\AlbanianXrm.XrmPluginExtensions.9.nuspec" -OutputDirectory ".\packages" -Version $packageVersion
& ".\packages\NuGet.exe" pack ".\AlbanianXrm.XrmWorkflowExtensions.Source\AlbanianXrm.XrmWorkflowExtensions.8.nuspec" -OutputDirectory ".\packages" -Version $packageVersion
& ".\packages\NuGet.exe" pack ".\AlbanianXrm.XrmWorkflowExtensions.Source\AlbanianXrm.XrmWorkflowExtensions.9.nuspec" -OutputDirectory ".\packages" -Version $packageVersion
