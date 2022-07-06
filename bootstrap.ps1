Start-IISCommitDelay
$Site=New-IISSite -Name "Manage" -BindingInformation "129.232.194.210:80:manage.carbonknown.com" -PhysicalPath "C:\MVC\Inetpub\CarbonKnown" -Passthru
$Site.Applications["/"].ApplicationPoolName = ".NET v4.5"
Stop-IISCommitDelay