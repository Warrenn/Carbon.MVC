$config =  @{
	input = @{
		type="csv";
		url = "f";
	};
#	input = @{
#		type="web";
#		username = "test";
#		password = "password";
#		loginPage = "/Account/login";
#		url = "http://localhost:53248";
#        contentPage = "/Home";
#	};
	output = @{
		type="ck3";
		url="http://localhost:50807/";
		username = "SManager";
		password = "Pa`$`$w0rd";
        handler="CarHire";
	}
#	output = @{
#		type="csv";
#		path="C:\Work Bench\csv\out1.csv";
#	}
}

function ExtractWebData($config){
    $baseUri = New-Object System.Uri $config.input.url

    $loginPage = New-Object System.Uri ($baseUri,$config.input.loginPage)
    $session = Login $loginPage.AbsoluteUri $config.input.username $config.input.password

    $contentPage = New-Object System.Uri ($baseUri,$config.input.contentPage)
    $response = Invoke-RestMethod -WebSession $session -Uri $contentPage.AbsoluteUri

    $doc = New-Object HtmlAgilityPack.HtmlDocument
    $doc.LoadHtml($response);

    $doc.DocumentNode.SelectNodes("//table/tbody/tr")|
    %{@{
        RowNo = $_.SelectNodes("td")[0].InnerText;
        CarGroupBill = $_.SelectNodes("td")[1].InnerText;
        StartDate = [System.DateTime]::Parse($_.SelectNodes("td")[2].InnerText);
        EndDate = [System.DateTime]::Parse($_.SelectNodes("td")[3].InnerText);
        CostCode = $_.SelectNodes("td")[4].InnerText;
        Money = $_.SelectNodes("td")[5].InnerText;
        Units = $_.SelectNodes("td")[6].InnerText;
    }}
}


function ExtractData($config){
    if($config.input.type -eq "web"){
        return ExtractWebData $config
    }
    if($config.input.type -eq "csv"){
        gci "C:\Work Bench\csv\*.csv"|
        %{Import-Csv -LiteralPath $_}|
        %{@{
            RowNo = [Int32]$_.RowNo;
            CarGroupBill = $_.CarGroupBill;
            StartDate = [System.DateTime]::Parse($_.StartDate);
            EndDate = [System.DateTime]::Parse($_.EndDate);
            CostCode = $_.CostCode;
            Money = $_.Money;
            Units = $_.Units;
        }}
    }
    if($config.input.type -eq "database"){
    }
}

function GetEntryUrl($config){
    $handlers = @{
        "Accommodation" = "api/datasource/upsert/accommodation"; 
        "AirTravel" = "api/datasource/upsert/airtravel";
        "AirTravelRoute" = "api/datasource/upsert/airtravelroute";
        "CarHire" = "api/datasource/upsert/carhire";
        "Commuting" = "api/datasource/upsert/commuting";
        "Courier" ="api/datasource/upsert/courier";

        "CourierRoute" = "api/datasource/upsert/courierroute";
        "Fleet" = "api/datasource/upsert/fleet";
        "Paper" = "api/datasource/upsert/paper";
        "Waste" = "api/datasource/upsert/waste";
        "Water" ="api/datasource/upsert/water";
    }

    $baseUri = New-Object System.Uri $config.output.url
    $methodUri = New-Object System.Uri ($baseUri,$handlers[$config.output.handler])
    return $methodUri.AbsoluteUri
}

function AddFeedEntry($baseurl,$session,$entry){
    $baseUri = New-Object System.Uri $baseurl
    $methodUri = New-Object System.Uri ($baseUri,"api/datasource/insert/datafeed")

	return Invoke-RestMethod -Method Post -WebSession $session -Uri $methodUri.AbsoluteUri -Body $entry
}

function Calculate($baseurl,$session,$entry){
    $baseUri = New-Object System.Uri $baseurl
    $methodUri = New-Object System.Uri ($baseUri,("api/datasource/calculate/" + $entry.SourceId))

	return Invoke-RestMethod -Method Post -WebSession $session -Uri $methodUri.AbsoluteUri
}

function Login($loginPage,$userName,$password){
    $loginUri = New-Object System.Uri $loginPage
	$response = Invoke-WebRequest -Uri $loginPage -SessionVariable ws -UseBasicParsing
    $formData = @{}
    $response.InputFields | 
    %{ $formData[$_.name] = $_.value;}
    $actionUrl = $response.Content | Get-Matches('[A|a][C|c][T|t][I|i][O|o][N|n][^"'']*["|''](?<Url>[^"'']*)')
    $actionUri = New-Object System.Uri ($loginUri,$actionUrl.Url)
    $formData.UserName = $userName
    $formData.Password = $password
	$response = Invoke-WebRequest -Uri $actionUri.AbsoluteUri -Body $formData -WebSession $ws -Method Post
    return $ws
}

function Get-Matches {
  param(
    [Parameter(Mandatory=$true)]
    $Pattern,
    
    [Parameter(ValueFromPipeline=$true)]
    $InputObject
  )
  
 begin {
  
    try {
   $regex = New-Object Regex($pattern) 
  } 
  catch {
   Throw "Get-Matches: Pattern not correct. '$Pattern' is no valid regular expression."
  }
  $groups = @($regex.GetGroupNames() | 
  Where-Object { ($_ -as [Int32]) -eq $null } |
  ForEach-Object { $_.toString() })
 }

 process { 
  foreach ($line in $InputObject) {
   foreach ($match in ($regex.Matches($line))) {
    if ($groups.Count -eq 0) {
     ([Object[]]$match.Groups)[-1].Value
    } else {
     $rv = 1 | Select-Object -Property $groups
     $groups | ForEach-Object {
      $rv.$_ = $match.Groups[$_].Value
     }
     $rv
    }
   }
  }
 }
}

if($config.output.type -eq "ck3"){
    $feedData = @{
        SourceUrl = $config.input.url;
        ScriptPath = $MyInvocation.MyCommand.Definition;
        HandlerName = $config.output.handler;
        UserName = $config.output.username;
    }

    $directory = [System.IO.Path]::GetDirectoryName($feedData.ScriptPath)
    $assembly = [System.IO.Path]::Combine($directory,"HtmlAgilityPack.dll")
    Add-Type -Path $assembly

    $methodUrl = GetEntryUrl $config

    $ws = Login ($config.output.url+"/Account/Login") $config.output.userName $config.output.password
    $sourceData = AddFeedEntry $config.output.url $ws $feedData
    $allCorrect = $true
    $currentDate = Get-Date
    ExtractData $config|
        %{$_.SourceId = $sourceData.SourceId;$_}|
         %{Invoke-RestMethod -Method Post -WebSession $ws -Uri $methodUrl -Body $_}|
         %{$allCorrect = $allCorrect -and $_.Succeeded}

    if($allCorrect){
        Calculate $config.output.url $ws $sourceData
    }
}

if($config.output.type -eq "csv"){
    ExtractData $config|
        %{new-object psobject -Property $_}|
        Export-Csv -Path $config.output.path
}
