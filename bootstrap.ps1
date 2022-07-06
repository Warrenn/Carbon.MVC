$SiteName = "Manage"
$BasePath = "${env:MVC_BASE}"
Start-IISCommitDelay
$Site=Get-IISSite $SiteName
if ($Site -ne $null)  {
    Remove-IISSite -Name $SiteName -Force
}
$Site=New-IISSite -Name $SiteName -BindingInformation "129.232.194.210:80:manage.carbonknown.com" -PhysicalPath $BasePath -Passthru
$Site.Applications["/"].ApplicationPoolName = ".NET v4.5"
New-WebVirtualDirectory -Site $Site -Name "Ck3/Nampak" -PhysicalPath "$BasePath/Nampak"
Stop-IISCommitDelay