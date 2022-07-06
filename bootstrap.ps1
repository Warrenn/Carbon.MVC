Start-IISCommitDelay
$BasePath = "${env:MVC_BASE}"
$Site=New-IISSite -Name "Manage" -BindingInformation "129.232.194.210:80:manage.carbonknown.com" -PhysicalPath $BasePath  -Passthru
$Site.Applications["/"].ApplicationPoolName = ".NET v4.5"
write-host $Site
New-WebVirtualDirector -Site $Site -Name "Ck3/Nampak" -PhysicalPath "$BasePath/Nampak"
Stop-IISCommitDelay