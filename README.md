# Email Client - Mailbox Cleanup Tool

A powerful IMAP email client designed to help you clean up your mailbox efficiently by viewing emails grouped by sender and performing bulk operations.

## 🚀 Features

- 🔐 **Universal IMAP Support**: Connect to any IMAP email server (Gmail, Outlook, Yahoo, custom servers)
- 📧 **Smart Email Grouping**: View emails organized by sender with detailed statistics
- 🔍 **Advanced Search & Filter**: Search by sender name or email address with real-time filtering
- 📊 **Flexible Sorting**: Sort by email count, total size, or sender name
- 🗑️ **Bulk Operations**: Delete all emails from specific senders with confirmation dialogs
- ⚡ **Performance Optimized**: Handles large mailboxes (20k+ emails) with batch processing
- 📈 **Progress Tracking**: Real-time progress indicators for long-running operations
- 🎨 **Modern UI**: Clean, responsive interface built with React and TypeScript
- 🔄 **Real-time Updates**: Live connection status and automatic refresh capabilities

## 🏗️ Architecture

### Backend (ASP.NET Core 8.0)

- **SOLID Principles**: Clean architecture with dependency injection
- **Service Layer**: Specialized services for connection, retrieval, and deletion
- **IMAP Integration**: MailKit library for robust email server communication
- **Batch Processing**: Optimized for large datasets with progress tracking
- **API Documentation**: Comprehensive Swagger/OpenAPI documentation

### Frontend (React 18 + TypeScript)

- **Component Architecture**: Modular, reusable components following single responsibility
- **Type Safety**: Full TypeScript integration with comprehensive interfaces
- **State Management**: React hooks for efficient state handling
- **Service Layer**: Facade pattern for clean API communication
- **Responsive Design**: Mobile-friendly interface with modern styling

## 📋 Prerequisites

Before running the application, ensure you have:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Node.js](https://nodejs.org/) (version 16 or later) with npm

## 🚀 Quick Start

### Option 1: Platform-Specific One-Command Startup (Recommended)

**Windows PowerShell:**

```powershell
.\start.ps1
```

**Windows Command Prompt:**

```cmd
start.bat
```

**Linux/macOS:**

```bash
chmod +x start.sh
./start.sh
```

### Stopping the Application

**Windows PowerShell:**

```powershell
# Press Ctrl+C in the terminal running start.ps1
# Or use PowerShell to stop processes:
Get-Process | Where-Object {$_.ProcessName -match "node|dotnet|EmailClient"} | Stop-Process -Force
```

**Linux/macOS:**

```bash
# Press Ctrl+C in the terminal running start.sh
# Or use the stop script:
./stop.sh
```

### What the Start Scripts Do

The startup scripts automatically:

- ✅ Check all prerequisites (.NET 8.0, Node.js, npm)
- 🧹 Clean up any existing processes on ports 3000 and 5000
- 📦 Install frontend dependencies if needed
- 🔨 Build the backend application
- 🚀 Start both backend (port 5000) and frontend (port 3000) services
- 🌐 Provide direct links to access the application
- 📋 Display process IDs for manual control if needed
- 📄 Create log files (backend.log, frontend.log) for debugging

### Option 2: Manual Startup

If you prefer manual control or need to debug individual components:

**1. Start the Backend API:**

```bash
cd Backend/EmailClient.Api
dotnet run --urls=http://localhost:5000
```

**2. Start the Frontend (in a new terminal):**

```bash
cd Frontend
npm install
npm start
```

**3. Access the Application:**

- 🌐 **Frontend**: http://localhost:3000
- 🔧 **Backend API**: http://localhost:5000
- 📚 **API Documentation**: http://localhost:5000/swagger

## 📱 Usage Guide

### Connecting to Your Email Server

1. **Select your email provider** from the dropdown:

   - Gmail, Outlook, Yahoo (pre-configured settings)
   - Custom (for other IMAP servers)

2. **Enter your credentials:**

   - **Gmail**: Use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password
   - **Outlook/Hotmail**: Regular password should work
   - **Yahoo**: May require App Password depending on security settings
   - **Custom servers**: Check with your email provider for IMAP settings

3. **Connect**: Click "Connect to Email Server" and wait for connection confirmation

### Managing Your Mailbox

1. **View Email Groups**: After connecting, emails are automatically grouped by sender
2. **Search & Filter**: Use the search box to find specific senders
3. **Sort Results**: Choose sorting by email count, total size, or sender name
4. **Expand Details**: Click on any sender group to view individual emails
5. **Bulk Delete**: Click "Delete All" next to any sender to remove all their emails (with confirmation)

### Performance with Large Mailboxes

- ✅ **Optimized for 20k+ emails** with batch processing
- ⏱️ **Progress tracking** shows real-time status for long operations
- 📊 **Memory efficient** processing prevents browser/server crashes
- 🔄 **Resumable operations** can be stopped and restarted safely

## 🔧 Technical Details

### Architecture Overview

- **Backend**: ASP.NET Core 8.0 Web API with MailKit for IMAP operations
- **Frontend**: React 18 with TypeScript, modern hooks-based architecture
- **Communication**: RESTful API with JSON data exchange
- **Authentication**: Direct IMAP server authentication (no data stored)
- **Performance**: Batch processing with configurable chunk sizes

### API Endpoints

- `POST /api/email/connect` - Establish IMAP connection
- `GET /api/email/emails-by-sender` - Retrieve grouped emails
- `GET /api/email/processing-status` - Get operation progress
- `DELETE /api/email/delete` - Delete specific emails by UID
- `DELETE /api/email/delete-by-sender/{senderEmail}` - Bulk delete by sender
- `POST /api/email/disconnect` - Close IMAP connection

### Security Considerations

- 🔐 **No data persistence**: Credentials are not stored anywhere
- 🔒 **Local processing**: All operations happen on your machine
- 🛡️ **SSL/TLS**: Secure connections to email servers
- 🔑 **App passwords**: Supports modern authentication methods

### Technology Stack

- **Backend**: ASP.NET Core 8.0, MailKit, Dependency Injection
- **Frontend**: React 18, TypeScript, Axios, Modern Hooks
- **Email Protocol**: IMAP with SSL/TLS encryption
- **API Documentation**: Swagger/OpenAPI available at http://localhost:5000/swagger

## 📧 Email Provider Setup

### Gmail Configuration

- **Server**: imap.gmail.com | **Port**: 993 | **SSL**: Yes
- **Authentication**: [App Password required](https://support.google.com/accounts/answer/185833)
- **2FA**: Must be enabled to generate App Passwords

### Outlook/Hotmail Configuration

- **Server**: outlook.office365.com | **Port**: 993 | **SSL**: Yes
- **Authentication**: Regular password (usually works)
- **Modern Auth**: Supports OAuth2 if configured

### Yahoo Mail Configuration

- **Server**: imap.mail.yahoo.com | **Port**: 993 | **SSL**: Yes
- **Authentication**: App Password may be required depending on security settings

### Custom IMAP Servers

- Configure server, port, and SSL settings manually
- Consult your email provider's documentation
- Most servers use port 993 with SSL enabled

## 🚨 Troubleshooting

### Connection Issues

- **"Failed to connect"**:
  - ✅ Verify email address and password/App Password
  - ✅ Ensure IMAP is enabled in your email account settings
  - ✅ Check firewall settings (ports 993, 143)
  - ✅ Try different network connection (some ISPs block email ports)

### Performance Issues

- **Slow loading with large mailboxes**:
  - ⏱️ Be patient - processing 20k+ emails takes time
  - 📊 Monitor progress bar for status updates
  - 💾 Close other memory-intensive applications

### Application Won't Start

- **Backend startup errors**:

  - ✅ Ensure .NET 8.0 SDK is installed: `dotnet --version`
  - ✅ Check if port 5000 is available
  - ✅ Review backend.log for specific error messages

- **Frontend startup errors**:
  - ✅ Ensure Node.js is installed: `node --version`
  - ✅ Clear npm cache: `npm cache clean --force`
  - ✅ Delete node_modules and reinstall: `rm -rf node_modules && npm install` (Linux/macOS) or `rmdir /s node_modules && npm install` (Windows)
  - ✅ Review frontend.log for specific error messages

### Build Issues

- **Dependency injection errors**:
  - ✅ Ensure all services are properly registered in Program.cs
  - ✅ Check that interface implementations match constructor parameters
  - ✅ Verify .NET 8.0 compatibility

## ⚠️ Important Notes

- **⚠️ Permanent Deletion**: When you delete emails, they are **permanently removed** from your email server
- **🔧 IMAP Required**: This tool only works with IMAP-enabled email accounts
- **🏠 Local Processing**: Application runs entirely on your local machine - no data sent to external servers
- **🔒 Privacy First**: Your credentials and emails are never stored or transmitted to third parties
- **💾 No Database**: The application is stateless - connection details are not persisted

## 🤝 Contributing

This project follows SOLID principles and clean architecture patterns. Contributions are welcome!

### Development Setup

1. Clone the repository
2. Follow the manual startup instructions for development
3. Backend changes: Modify files in `Backend/EmailClient.Api/`
4. Frontend changes: Modify files in `Frontend/src/`
5. Run tests: `dotnet test` (backend) and `npm test` (frontend)

### Code Style

- **Backend**: Follow C# conventions with XML documentation
- **Frontend**: Use TypeScript with strict mode, ESLint rules
- **Architecture**: Maintain single responsibility per file/class
- **Documentation**: Update JSDoc/XML docs for public APIs

## 📄 License

This project is open source and available under the MIT License.

## 🙋‍♂️ Support

If you encounter any issues:

1. Check the troubleshooting section above
2. Review the log files (backend.log, frontend.log)
3. Ensure all prerequisites are properly installed
4. Verify your email provider's IMAP settings

## 🎯 Project Status

✅ **Stable Release** - Ready for production use  
✅ **Cross-Platform** - Works on Windows, Linux, and macOS  
✅ **Performance Optimized** - Handles large mailboxes efficiently  
✅ **Modern Architecture** - Built with latest technologies and best practices

## 📄 License

This project is open source and available under the MIT License.
