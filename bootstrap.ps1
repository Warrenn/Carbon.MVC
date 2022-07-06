$SiteName = "Manage"
$BasePath = "${env:MVC_BASE}"
if([String]::IsNullOrWhiteSpace($BasePath)){$BasePath="C:\MVC\Inetpub\CarbonKnown"}
Start-IISCommitDelay
$Site=Get-IISSite $SiteName
if ($Site -ne $null)  {
    Remove-IISSite -Name $SiteName -Confirm:$false
}
$Site=New-IISSite -Name $SiteName -BindingInformation "129.232.194.210:80:manage.carbonknown.com" -PhysicalPath $BasePath -Passthru
$Site.Applications["/"].ApplicationPoolName = ".NET v4.5"
$Site.Applications["/Ck3"].ApplicationPoolName = ".NET v4.5"
$Site.Applications["/Ck3/Nampak"].ApplicationPoolName = ".NET v4.5"
New-Item -ItemType Directory -Force -Path "$BasePath/Nampak"
New-WebApplication -Name "Ck3" -Site $SiteName -PhysicalPath "$BasePath" -ApplicationPool ".NET v4.5"
New-WebApplication -Name "Ck3/Nampak" -Site $SiteName -PhysicalPath "$BasePath\Nampak" -ApplicationPool ".NET v4.5"
Stop-IISCommitDelay