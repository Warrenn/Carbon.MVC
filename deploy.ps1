 Param(
    $Artifact = "CarbonKnown.MVC.zip",
    $Release = "v1.0.0",
    [Parameter(Mandatory=$true)]
    $Client
)

$RepoUrl = "https://github.com/Warrenn/Carbon.MVC/releases/download/$Release/$Artifact"

[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
Set-ExecutionPolicy Bypass -Scope Process -Force; 
$ParentPath= Resolve-Path ".." 
iex ((New-Object System.Net.WebClient).DownloadFile($RepoUrl, "$ParentPath/$Artifact")) 


