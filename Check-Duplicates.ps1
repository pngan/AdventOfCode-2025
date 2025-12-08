param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$filePath
)

# Check if the file exists
if (Test-Path $filePath) {
    # Get the content of the file, group by line, and filter for counts greater than 1
    $duplicates = Get-Content $filePath | Group-Object | Where-Object { $_.Count -gt 1 }
    
    if ($duplicates) {
        Write-Host "Found $($duplicates.Count) line(s) with duplicates:" -ForegroundColor Yellow
        $duplicates | ForEach-Object {
            Write-Host "  '$($_.Name)' (Count: $($_.Count))" -ForegroundColor Cyan
        }
    } else {
        Write-Host "No duplicate lines found in '$filePath'" -ForegroundColor Green
    }
} else {
    Write-Host "Error: File not found at '$filePath'" -ForegroundColor Red
    exit 1
}