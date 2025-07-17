@echo off
echo Starting Email Client...

REM Check if npm is installed
where npm >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo Error: npm is not installed or not in PATH
    echo Please install Node.js from https://nodejs.org/
    pause
    exit /b 1
)

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo Error: .NET is not installed or not in PATH
    echo Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Install frontend dependencies if needed
if not exist "Frontend\node_modules" (
    echo Installing frontend dependencies...
    cd Frontend
    call npm install
    if %ERRORLEVEL% neq 0 (
        echo Failed to install dependencies!
        pause
        exit /b 1
    )
    cd ..
)

echo.
echo Starting Email Client services...
echo Backend will be available at: http://localhost:5000
echo Frontend will be available at: http://localhost:3000
echo Swagger API docs at: http://localhost:5000/swagger
echo.
echo Press Ctrl+C in the new windows to stop the services
echo.

REM Start backend in new window
start "Email Client Backend" cmd /k "cd Backend\EmailClient.Api && dotnet run --urls=http://localhost:5000"

REM Wait a bit for backend to start
timeout /t 5 /nobreak >nul

REM Start frontend in new window
start "Email Client Frontend" cmd /k "cd Frontend && set BROWSER=none && npm start"

REM Wait a bit for frontend to start
timeout /t 10 /nobreak >nul

REM Open browser
start http://localhost:3000

echo Email Client is starting...
echo Check the separate command windows for any errors.
echo.
echo Your Email Client is ready! Features:
echo - Connect to any IMAP email server (Gmail, Outlook, Yahoo, etc.)
echo - View emails grouped by sender
echo - Sort and filter emails
echo - Delete all emails from specific senders in one click
echo.
pause
