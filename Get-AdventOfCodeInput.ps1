# A function to download Advent of Code input data and store with standard filename

    param (
        [Parameter(Position=0, Mandatory = $true, HelpMessage = 'Get cookie https://github.com/GreenLightning/advent-of-code-downloader?tab=readme-ov-file#how-do-i-get-my-session-cookie')]
        [string]$Cookie,
        
		[Parameter(Mandatory = $true, HelpMessage = 'Password to protect input file')]
        [string]$Password,
		
        [ValidateRange(2015, 2025)]
        [Parameter(Mandatory=$true)]
        [int]$Year,

        [ValidateRange(1, 25)]
        [Parameter(Mandatory=$true)]
        [int]$Day
    )

    # Construct the URL for the input data
    $url = "https://adventofcode.com/$Year/day/$Day/input"

    # Attempt to download the input data
    try {
        $webClient = New-Object System.Net.WebClient    
        $webClient.Headers.add('Cookie', "session=$Cookie")   
        $response = $webClient.DownloadString($url)    
        
        $filePath = "input\${Year}_{0:D2}_input.txt" -f $Day
        $response | Out-File -FilePath $filePath -Encoding utf8
        Write-Output "Input data for Day $Day of Advent of Code $Year has been saved to '$filePath'."
    } catch {
        Write-Output "Error: Failed to download input data. Please check your session cookie and ensure you have access to the Advent of Code website."
    }
	
	$7zipPath = "$env:ProgramData\chocolatey\bin\7z.exe"

	if (-not (Test-Path -Path $7zipPath -PathType Leaf)) {
		throw "7 zip executable '$7zipPath' not found"
	}

	Set-Alias Start-SevenZip $7zipPath

	$Source = "*.txt"
	$Target = ".\Input.7z"
    Push-Location input
	Start-SevenZip a $Target $Source -p"${Password}"
	Pop-Location

    # Also copy the days code file
    Push-Location code
    $dayFile = "day{0:D2}.cs" -f $Day
	Copy-Item -Path template.cs -Destination $dayFile -Force
	Pop-Location
	
# Example usage:
# Get-AdventOfCodeInput -Password hellothere -Cookie <cookie> -Year 2025 -Day 1 

