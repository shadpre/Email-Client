# PowerShell script to test the date filtering functionality
# This script sends requests to the API to verify date filtering works correctly

$baseUrl = "http://localhost:5000/api/email"

Write-Host "Testing Date Filter Functionality" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Test 1: Filter for emails older than 30 days
Write-Host "`nTest 1: Emails older than 30 days" -ForegroundColor Yellow

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

# Test 2: Filter for emails in a date range
Write-Host "`nTest 2: Emails in date range (last 7 days)" -ForegroundColor Yellow

$startDate = (Get-Date).AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
$endDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

$dateRangeFilter = @{
    DateFilter = @{
        FilterType = "DateRange"
        StartDate = $startDate
        EndDate = $endDate
    }
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/emails-by-sender/filter" -Method POST -Body $dateRangeFilter -ContentType "application/json"
    Write-Host "✓ Success: Found $($response.Length) sender groups" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: No filter (all emails)
Write-Host "`nTest 3: All emails (no filter)" -ForegroundColor Yellow

$noFilter = @{
    DateFilter = $null
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/emails-by-sender/filter" -Method POST -Body $noFilter -ContentType "application/json"
    Write-Host "✓ Success: Found $($response.Length) sender groups" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nDate filter testing completed!" -ForegroundColor Green
