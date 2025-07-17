# PowerShell script to test the date filtering functionality
# This script sends requests to the API to verify date filtering works correctly

$baseUrl = "http://localhost:5000/api/email"

Write-Host "Testing Date Filter Functionality" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Test 1: Filter for emails older than 30 days
Write-Host ""
Write-Host "Test 1: Emails older than 30 days" -ForegroundColor Yellow

$olderThanFilter = @{
    DateFilter = @{
        FilterType = "OlderThanDays"
        Days = 30
    }
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/emails-by-sender/filter" -Method POST -Body $olderThanFilter -ContentType "application/json"
    Write-Host "✓ Success: Found $($response.Length) sender groups" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Filter for emails older than 6 months
Write-Host ""
Write-Host "Test 2: Emails older than 6 months" -ForegroundColor Yellow

$monthsFilter = @{
    DateFilter = @{
        FilterType = "OlderThanMonths"
        Months = 6
    }
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/emails-by-sender/filter" -Method POST -Body $monthsFilter -ContentType "application/json"
    Write-Host "✓ Success: Found $($response.Length) sender groups" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Filter for emails older than 1 year
Write-Host ""
Write-Host "Test 3: Emails older than 1 year" -ForegroundColor Yellow

$yearsFilter = @{
    DateFilter = @{
        FilterType = "OlderThanYears"
        Years = 1
    }
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/emails-by-sender/filter" -Method POST -Body $yearsFilter -ContentType "application/json"
    Write-Host "✓ Success: Found $($response.Length) sender groups" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: No filter (all emails)
Write-Host ""
Write-Host "Test 4: All emails (no filter)" -ForegroundColor Yellow

$noFilter = @{
    DateFilter = $null
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/emails-by-sender/filter" -Method POST -Body $noFilter -ContentType "application/json"
    Write-Host "✓ Success: Found $($response.Length) sender groups" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Date filter testing completed!" -ForegroundColor Green
