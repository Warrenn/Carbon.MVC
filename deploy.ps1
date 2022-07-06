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
New-Item -ItemType Directory -Force -Path "$ParentPath/$Client"
Write-Host "$RepoUrl"
(New-Object System.Net.WebClient).DownloadFile($RepoUrl, "$ParentPath/$Client/$Artifact")

Expand-Archive -Path "$ParentPath/$Client/$Artifact" -DestinationPath "$ParentPath\Inetpub\CarbonKnown\$Client"