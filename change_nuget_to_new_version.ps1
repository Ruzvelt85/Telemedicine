param([string]$version)

function Green
{
    process { Write-Host $_ -ForegroundColor Green }
}

Write-Output("Authorizing... `n")
Invoke-Command -ScriptBlock {aws codeartifact login --tool nuget --domain telemedicine --domain-owner 066612722316 --repository Telemedicine}
Write-Output("`n")

$hash = @{}

$json = Get-Content "nuget_project.json" | Out-String | ConvertFrom-Json

foreach ($property in $json.PSObject.Properties) {
    $hash[$property.Name] = $property.Value
}

$projects = Get-ChildItem -Path $PSScriptRoot -Include "*.csproj" -Recurse -ErrorAction SilentlyContinue

foreach ($project in $projects)
{
	Set-Location -Path $project.DirectoryName
	$file = $project.Name

	Write-Output("Processing $file ..." | Green)

	Write-Output("`n")	
	Invoke-Expression -Command "dotnet restore" -ErrorAction Stop
	Write-Output("`n")
	$packages_list = Invoke-Expression -Command "dotnet list $file package" -ErrorAction Stop

	foreach ($package in $packages_list)
	{
		foreach ($object in $package)
		{
			$package_string = $object.ToString()

			foreach ($obj in $hash.GetEnumerator())
			{
				if($package_string -like "*> $($obj.Name) *")
				{
					$project.DirectoryName
					$file

					Write-Output("add package")
					if($version)
					{
						Invoke-Expression -Command "dotnet add $file package $($obj.Name) -v $($version)" -ErrorAction Stop
						Write-Output("`n")
					}
					else
					{
						Invoke-Expression -Command "dotnet add $file package $($obj.Name)" -ErrorAction Stop
						Write-Output("`n")
					}
				}
			}
		}
	}
}

cd $PSScriptRoot