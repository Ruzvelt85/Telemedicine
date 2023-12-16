function Green
{
    process { Write-Host $_ -ForegroundColor Green }
}

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
					Write-Output("remove package")
					Write-Output("`n")
					Invoke-Expression -Command "dotnet remove $file package $($obj.Name)" -ErrorAction Stop
					Write-Output("`n")

					Write-Output("add project")
					Write-Output("`n")
					Invoke-Expression -Command "dotnet add $file reference $($PSScriptRoot + $obj.Value)" -ErrorAction Stop
					Write-Output("`n")
				}
			}
		}
	}
}

cd $PSScriptRoot