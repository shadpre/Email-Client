# Email Client Setup Verification
Write-Host "Email Client Setup Verification" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""

$hasErrors = $false

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = & dotnet --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ .NET SDK version: $dotnetVersion" -ForegroundColor Green
    } else {
        throw "dotnet command failed"
    }
} catch {
    Write-Host "✗ .NET SDK not found!" -ForegroundColor Red
    Write-Host "  Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    $hasErrors = $true
}

# Check Node.js
Write-Host "Checking Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = & node --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Node.js version: $nodeVersion" -ForegroundColor Green
    } else {
        throw "node command failed"
    }
} catch {
    Write-Host "✗ Node.js not found!" -ForegroundColor Red
    Write-Host "  Please install Node.js from https://nodejs.org/" -ForegroundColor Yellow
    $hasErrors = $true
}

# Check npm
Write-Host "Checking npm..." -ForegroundColor Yellow
try {
    $npmVersion = & npm --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ npm version: $npmVersion" -ForegroundColor Green
    } else {
        throw "npm command failed"
    }
} catch {
    Write-Host "✗ npm not found!" -ForegroundColor Red
    $hasErrors = $true
}

# Check project structure
Write-Host "Checking project structure..." -ForegroundColor Yellow
$requiredFiles = @(
    "Backend\EmailClient.Api\EmailClient.Api.csproj",
    "Backend\EmailClient.Api\Program.cs",
    "Frontend\package.json",
    "Frontend\src\App.tsx",
    "Frontend\src\index.tsx"
)

foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "✓ Found: $file" -ForegroundColor Green
    } else {
        Write-Host "✗ Missing: $file" -ForegroundColor Red
        $hasErrors = $true
    }
}

# Check backend build
Write-Host "Testing backend build..." -ForegroundColor Yellow
$currentLocation = Get-Location
try {
    Set-Location "Backend\EmailClient.Api"
    $buildResult = & dotnet build --nologo --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Backend builds successfully" -ForegroundColor Green
    } else {
        Write-Host "✗ Backend build failed" -ForegroundColor Red
        $hasErrors = $true
    }
} catch {
    Write-Host "✗ Backend build failed" -ForegroundColor Red
    $hasErrors = $true
} finally {
    Set-Location $currentLocation
}

# Check frontend dependencies
Write-Host "Checking frontend dependencies..." -ForegroundColor Yellow
if (Test-Path "Frontend\node_modules") {
    Write-Host "✓ Frontend dependencies installed" -ForegroundColor Green
} else {
    Write-Host "⚠ Frontend dependencies not installed (will be installed on first run)" -ForegroundColor Yellow
}

Write-Host ""
if ($hasErrors) {
    Write-Host "❌ Setup verification failed!" -ForegroundColor Red
    Write-Host "Please fix the errors above before running the application." -ForegroundColor Yellow
} else {
    Write-Host "✅ Setup verification passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now start the email client with:" -ForegroundColor Cyan
    Write-Host "  .\start.ps1" -ForegroundColor White
    Write-Host "or" -ForegroundColor Cyan
    Write-Host "  start.bat" -ForegroundColor White
}
Write-Host ""
