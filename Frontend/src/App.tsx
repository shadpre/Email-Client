import React, { useState } from "react";
import { ConnectionForm } from "./components/ConnectionForm";
import { EmailManager } from "./components/EmailManager";
import { ImapConfig } from "./types";
import { emailService } from "./services/emailService";

function App() {
  const [isConnected, setIsConnected] = useState(false);
  const [isConnecting, setIsConnecting] = useState(false);

  const handleConnect = async (config: ImapConfig) => {
    setIsConnecting(true);
    try {
      const success = await emailService.connect(config);
      if (success) {
        setIsConnected(true);
      } else {
        alert(
          "Failed to connect. Please check your credentials and server settings."
        );
      }
    } catch (error) {
      console.error("Connection error:", error);
      alert(
        "Connection error. Please check your internet connection and try again."
      );
    } finally {
      setIsConnecting(false);
    }
  };

  const handleDisconnect = async () => {
    try {
      await emailService.disconnect();
      setIsConnected(false);
    } catch (error) {
      console.error("Disconnect error:", error);
    }
  };

  return (
    <div style={{ minHeight: "100vh", backgroundColor: "#f5f5f5" }}>
      <header
        style={{
          backgroundColor: "#343a40",
          color: "white",
          padding: "20px",
          textAlign: "center",
          boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
        }}
      >
        <h1 style={{ margin: 0, fontSize: "2rem", fontWeight: "300" }}>
          📧 Email Client - Mailbox Cleanup Tool
        </h1>
        <p style={{ margin: "8px 0 0 0", opacity: 0.9 }}>
          Clean up your mailbox by deleting emails from specific senders
        </p>
        {isConnected && (
          <div style={{ marginTop: "15px" }}>
            <button
              onClick={handleDisconnect}
              className="button button-secondary"
              style={{ fontSize: "14px" }}
            >
              Disconnect
            </button>
          </div>
        )}
      </header>

      <main>
        {!isConnected ? (
          <ConnectionForm
            onConnect={handleConnect}
            isConnecting={isConnecting}
          />
        ) : (
          <EmailManager />
        )}
      </main>

      <footer
        style={{
          textAlign: "center",
          padding: "20px",
          color: "#6c757d",
          fontSize: "14px",
          borderTop: "1px solid #dee2e6",
          marginTop: "40px",
        }}
      >
        <p style={{ margin: 0 }}>
          ⚠️ <strong>Warning:</strong> Deleted emails are permanently removed
          from your email server.
        </p>
      </footer>
    </div>
  );
}

export default App;
