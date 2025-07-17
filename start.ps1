# Email Client Startup Script
Write-Host "Starting Email Client..." -ForegroundColor Green

# Function to check if a port is in use
function Test-Port {
    param([int]$Port)
    try {
        $connection = New-Object System.Net.Sockets.TcpClient("localhost", $Port)
        $connection.Close()
        return $true
    }
    catch {
        return $false
    }
}

# Kill any existing processes on our ports
Write-Host "Cleaning up existing processes..." -ForegroundColor Yellow
Get-Process -Name "EmailClient.Api" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "node" -ErrorAction SilentlyContinue | Where-Object {$_.ProcessName -eq "node"} | Stop-Process -Force

# Wait a moment for cleanup
Start-Sleep -Seconds 2

# Install frontend dependencies if needed
$frontendPath = Join-Path $PSScriptRoot "Frontend"
if (!(Test-Path (Join-Path $frontendPath "node_modules"))) {
    Write-Host "Installing frontend dependencies..." -ForegroundColor Yellow
    Set-Location $frontendPath
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to install frontend dependencies!" -ForegroundColor Red
        exit 1
    }
}

# Start backend
Write-Host "Starting backend API..." -ForegroundColor Cyan
$backendPath = Join-Path $PSScriptRoot "Backend\EmailClient.Api"
Set-Location $backendPath

# Start backend in background
$backendJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    dotnet run --urls="http://localhost:5000"
} -ArgumentList $backendPath

# Wait for backend to start
Write-Host "Waiting for backend to start..." -ForegroundColor Yellow
$timeout = 30
$counter = 0
do {
    Start-Sleep -Seconds 1
    $counter++
    if ($counter -ge $timeout) {
        Write-Host "Backend failed to start within $timeout seconds!" -ForegroundColor Red
        Stop-Job $backendJob -PassThru | Remove-Job
        exit 1
    }
} while (!(Test-Port 5000))

Write-Host "Backend started successfully on http://localhost:5000" -ForegroundColor Green

# Start frontend
Write-Host "Starting frontend..." -ForegroundColor Cyan
Set-Location $frontendPath

# Start frontend in background
$frontendJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    $env:BROWSER = "none"  # Prevent auto-opening browser
    npm start
} -ArgumentList $frontendPath

# Wait for frontend to start
Write-Host "Waiting for frontend to start..." -ForegroundColor Yellow
$timeout = 60
$counter = 0
do {
    Start-Sleep -Seconds 1
    $counter++
    if ($counter -ge $timeout) {
        Write-Host "Frontend failed to start within $timeout seconds!" -ForegroundColor Red
        Stop-Job $backendJob -PassThru | Remove-Job
        Stop-Job $frontendJob -PassThru | Remove-Job
        exit 1
    }
} while (!(Test-Port 3000))

Write-Host "" -ForegroundColor White
Write-Host "==================================" -ForegroundColor Green
Write-Host "Email Client Started Successfully!" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host "Frontend: http://localhost:3000" -ForegroundColor Cyan
Write-Host "Backend:  http://localhost:5000" -ForegroundColor Cyan
Write-Host "API Docs: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White
Write-Host "Press Ctrl+C to stop both services" -ForegroundColor Yellow
Write-Host "" -ForegroundColor White

# Open browser
Start-Process "http://localhost:3000"

# Wait for user to stop
try {
    while ($true) {
        Start-Sleep -Seconds 1
        # Check if jobs are still running
        if ((Get-Job $backendJob.Id).State -ne "Running") {
            Write-Host "Backend stopped unexpectedly!" -ForegroundColor Red
            break
        }
        if ((Get-Job $frontendJob.Id).State -ne "Running") {
            Write-Host "Frontend stopped unexpectedly!" -ForegroundColor Red
            break
        }
    }
}
catch {
    Write-Host "Stopping services..." -ForegroundColor Yellow
}
finally {
    # Cleanup
    Stop-Job $backendJob -PassThru | Remove-Job -Force -ErrorAction SilentlyContinue
    Stop-Job $frontendJob -PassThru | Remove-Job -Force -ErrorAction SilentlyContinue
    Write-Host "Services stopped." -ForegroundColor Green
}
