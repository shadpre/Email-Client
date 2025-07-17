#!/bin/bash

# Email Client Startup Script for Linux/macOS
echo "Starting Email Client..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check if a port is in use
port_in_use() {
    local port=$1
    if command_exists lsof; then
        lsof -i :$port >/dev/null 2>&1
    elif command_exists netstat; then
        netstat -ln | grep ":$port " >/dev/null 2>&1
    else
        # Fallback: try to connect to the port
        timeout 1 bash -c "</dev/tcp/localhost/$port" >/dev/null 2>&1
    fi
}

# Function to kill processes on a port
kill_port() {
    local port=$1
    echo -e "${YELLOW}Cleaning up processes on port $port...${NC}"
    
    if command_exists lsof; then
        local pids=$(lsof -t -i:$port 2>/dev/null)
        if [ ! -z "$pids" ]; then
            echo $pids | xargs kill -9 2>/dev/null
            echo -e "${GREEN}Killed processes on port $port${NC}"
        fi
    elif command_exists fuser; then
        fuser -k $port/tcp 2>/dev/null
    fi
}

# Check prerequisites
echo -e "${YELLOW}Checking prerequisites...${NC}"

if ! command_exists node; then
    echo -e "${RED}Error: Node.js is not installed${NC}"
    echo "Please install Node.js from https://nodejs.org/"
    exit 1
fi

if ! command_exists npm; then
    echo -e "${RED}Error: npm is not installed${NC}"
    echo "Please install npm (usually comes with Node.js)"
    exit 1
fi

if ! command_exists dotnet; then
    echo -e "${RED}Error: .NET is not installed${NC}"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version | cut -d'.' -f1)
if [ "$DOTNET_VERSION" -lt "8" ]; then
    echo -e "${RED}Error: .NET 8.0 or higher is required${NC}"
    echo "Current version: $(dotnet --version)"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

echo -e "${GREEN}All prerequisites are installed${NC}"

# Clean up existing processes
echo -e "${YELLOW}Cleaning up existing processes...${NC}"
kill_port 3000  # React dev server
kill_port 5000  # .NET API server

# Kill any remaining dotnet processes (be careful with this in production)
pkill -f "EmailClient.Api" 2>/dev/null || true

# Wait a moment for processes to clean up
sleep 2

# Check if ports are still in use
if port_in_use 3000; then
    echo -e "${RED}Error: Port 3000 is still in use${NC}"
    echo "Please manually stop the process using port 3000 and try again"
    exit 1
fi

if port_in_use 5000; then
    echo -e "${RED}Error: Port 5000 is still in use${NC}"
    echo "Please manually stop the process using port 5000 and try again"
    exit 1
fi

# Install frontend dependencies if needed
if [ ! -d "Frontend/node_modules" ]; then
    echo -e "${YELLOW}Installing frontend dependencies...${NC}"
    cd Frontend
    npm install
    if [ $? -ne 0 ]; then
        echo -e "${RED}Failed to install frontend dependencies${NC}"
        exit 1
    fi
    cd ..
    echo -e "${GREEN}Frontend dependencies installed${NC}"
fi

# Build backend
echo -e "${YELLOW}Building backend...${NC}"
dotnet build Backend/EmailClient.Api/EmailClient.Api.csproj
if [ $? -ne 0 ]; then
    echo -e "${RED}Failed to build backend${NC}"
    exit 1
fi
echo -e "${GREEN}Backend built successfully${NC}"

# Start backend in background
echo -e "${YELLOW}Starting backend server...${NC}"
cd Backend/EmailClient.Api
dotnet run --urls="http://localhost:5000" > ../../backend.log 2>&1 &
BACKEND_PID=$!
cd ../..

# Wait for backend to start
echo -e "${YELLOW}Waiting for backend to start...${NC}"
for i in {1..30}; do
    if port_in_use 5000; then
        echo -e "${GREEN}Backend started successfully on port 5000${NC}"
        break
    fi
    sleep 1
    if [ $i -eq 30 ]; then
        echo -e "${RED}Backend failed to start within 30 seconds${NC}"
        echo "Check backend.log for errors"
        kill $BACKEND_PID 2>/dev/null
        exit 1
    fi
done

# Start frontend
echo -e "${YELLOW}Starting frontend server...${NC}"
cd Frontend
npm start > ../frontend.log 2>&1 &
FRONTEND_PID=$!
cd ..

# Wait for frontend to start
echo -e "${YELLOW}Waiting for frontend to start...${NC}"
for i in {1..60}; do
    if port_in_use 3000; then
        echo -e "${GREEN}Frontend started successfully on port 3000${NC}"
        break
    fi
    sleep 1
    if [ $i -eq 60 ]; then
        echo -e "${RED}Frontend failed to start within 60 seconds${NC}"
        echo "Check frontend.log for errors"
        kill $BACKEND_PID $FRONTEND_PID 2>/dev/null
        exit 1
    fi
done

echo ""
echo -e "${GREEN}🚀 Email Client is now running!${NC}"
echo ""
echo -e "${GREEN}📱 Frontend: ${NC}http://localhost:3000"
echo -e "${GREEN}🔧 Backend API: ${NC}http://localhost:5000"
echo -e "${GREEN}📚 API Documentation: ${NC}http://localhost:5000/swagger"
echo ""
echo -e "${YELLOW}📋 Process IDs:${NC}"
echo -e "   Backend PID: $BACKEND_PID"
echo -e "   Frontend PID: $FRONTEND_PID"
echo ""
echo -e "${YELLOW}📄 Logs:${NC}"
echo -e "   Backend: backend.log"
echo -e "   Frontend: frontend.log"
echo ""
echo -e "${YELLOW}⚠️  To stop the application:${NC}"
echo -e "   Press Ctrl+C or run: kill $BACKEND_PID $FRONTEND_PID"
echo ""

# Create a stop script
cat > stop.sh << 'EOF'
#!/bin/bash
echo "Stopping Email Client..."
kill $(cat .pids 2>/dev/null) 2>/dev/null
rm -f .pids
pkill -f "EmailClient.Api" 2>/dev/null
pkill -f "npm start" 2>/dev/null
echo "Email Client stopped"
EOF
chmod +x stop.sh

# Save PIDs for stop script
echo "$BACKEND_PID $FRONTEND_PID" > .pids

# Wait for user to stop
echo -e "${GREEN}Press Ctrl+C to stop the Email Client...${NC}"
trap 'echo -e "\n${YELLOW}Stopping Email Client...${NC}"; kill $BACKEND_PID $FRONTEND_PID 2>/dev/null; rm -f .pids; exit 0' INT TERM

# Keep script running
wait
