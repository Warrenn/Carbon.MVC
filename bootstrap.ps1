Start-IISCommitDelay
$Site=New-IISSite -Name "Manage" -BindingInformation "*:8080:" -PhysicalPath "C:\MVC\Inetpub\CarbonKnown" -Passthru
$Site.Applications["/"].ApplicationPoolName = ".NET v4.5"
Stop-IISCommitDelay