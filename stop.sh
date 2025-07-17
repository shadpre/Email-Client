#!/bin/bash

# Email Client Stop Script for Linux/macOS
echo "Stopping Email Client..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to kill processes on a port
kill_port() {
    local port=$1
    echo -e "${YELLOW}Stopping processes on port $port...${NC}"
    
    if command_exists lsof; then
        local pids=$(lsof -t -i:$port 2>/dev/null)
        if [ ! -z "$pids" ]; then
            echo $pids | xargs kill -TERM 2>/dev/null
            sleep 2
            # Force kill if still running
            pids=$(lsof -t -i:$port 2>/dev/null)
            if [ ! -z "$pids" ]; then
                echo $pids | xargs kill -9 2>/dev/null
            fi
            echo -e "${GREEN}Stopped processes on port $port${NC}"
        else
            echo -e "${YELLOW}No processes found on port $port${NC}"
        fi
    elif command_exists fuser; then
        fuser -k $port/tcp 2>/dev/null
    fi
}

# Kill processes from PID file if it exists
if [ -f ".pids" ]; then
    echo -e "${YELLOW}Stopping processes from PID file...${NC}"
    while read pid; do
        if kill -0 $pid 2>/dev/null; then
            kill -TERM $pid 2>/dev/null
            echo "Sent TERM signal to PID $pid"
        fi
    done < .pids
    
    # Wait a moment then force kill if needed
    sleep 3
    while read pid; do
        if kill -0 $pid 2>/dev/null; then
            kill -9 $pid 2>/dev/null
            echo "Force killed PID $pid"
        fi
    done < .pids
    
    rm -f .pids
    echo -e "${GREEN}Cleaned up PID file${NC}"
fi

# Kill by port numbers
kill_port 3000  # React dev server
kill_port 5000  # .NET API server

# Kill by process name (more aggressive cleanup)
echo -e "${YELLOW}Cleaning up by process name...${NC}"
pkill -f "EmailClient.Api" 2>/dev/null && echo -e "${GREEN}Stopped EmailClient.Api processes${NC}"
pkill -f "npm start" 2>/dev/null && echo -e "${GREEN}Stopped npm start processes${NC}"
pkill -f "node.*react-scripts" 2>/dev/null && echo -e "${GREEN}Stopped React development server${NC}"

# Clean up log files (optional)
if [ -f "backend.log" ] || [ -f "frontend.log" ]; then
    echo -e "${YELLOW}Log files are available:${NC}"
    [ -f "backend.log" ] && echo "  - backend.log"
    [ -f "frontend.log" ] && echo "  - frontend.log"
    echo -e "${YELLOW}Remove them manually if desired${NC}"
fi

echo -e "${GREEN}✅ Email Client stopped successfully${NC}"
