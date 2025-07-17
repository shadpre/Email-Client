import React, { useState } from "react";
import { ImapConfig } from "../types";

interface ConnectionFormProps {
  onConnect: (config: ImapConfig) => void;
  isConnecting: boolean;
}

const presetConfigs = {
  gmail: { server: "imap.gmail.com", port: 993, useSsl: true },
  outlook: { server: "outlook.office365.com", port: 993, useSsl: true },
  yahoo: { server: "imap.mail.yahoo.com", port: 993, useSsl: true },
  custom: { server: "", port: 993, useSsl: true },
};

export const ConnectionForm: React.FC<ConnectionFormProps> = ({
  onConnect,
  isConnecting,
}) => {
  const [selectedPreset, setSelectedPreset] =
    useState<keyof typeof presetConfigs>("gmail");
  const [config, setConfig] = useState<ImapConfig>({
    server: "imap.gmail.com",
    port: 993,
    username: "",
    password: "",
    useSsl: true,
  });

  const handlePresetChange = (preset: keyof typeof presetConfigs) => {
    setSelectedPreset(preset);
    setConfig({
      ...config,
      ...presetConfigs[preset],
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onConnect(config);
  };

  return (
    <div className="container">
      <div className="card" style={{ maxWidth: "500px", margin: "20px auto" }}>
        <div className="card-header">
          <h2 style={{ margin: 0 }}>Connect to IMAP Server</h2>
          <p style={{ margin: "8px 0 0 0", color: "#6c757d" }}>
            Clean up your mailbox by connecting to your email server
          </p>
        </div>
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="preset">Email Provider:</label>
              <select
                id="preset"
                className="form-control"
                value={selectedPreset}
                onChange={(e) =>
                  handlePresetChange(
                    e.target.value as keyof typeof presetConfigs
                  )
                }
              >
                <option value="gmail">Gmail</option>
                <option value="outlook">Outlook/Hotmail</option>
                <option value="yahoo">Yahoo Mail</option>
                <option value="custom">Custom</option>
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="server">IMAP Server:</label>
              <input
                id="server"
                type="text"
                className="form-control"
                value={config.server}
                onChange={(e) =>
                  setConfig({ ...config, server: e.target.value })
                }
                required
              />
            </div>

            <div className="d-flex gap-3">
              <div className="form-group" style={{ flex: 1 }}>
                <label htmlFor="port">Port:</label>
                <input
                  id="port"
                  type="number"
                  className="form-control"
                  value={config.port}
                  onChange={(e) =>
                    setConfig({ ...config, port: parseInt(e.target.value) })
                  }
                  required
                />
              </div>
              <div className="form-group" style={{ flex: 1 }}>
                <label>
                  <input
                    type="checkbox"
                    checked={config.useSsl}
                    onChange={(e) =>
                      setConfig({ ...config, useSsl: e.target.checked })
                    }
                    style={{ marginRight: "8px" }}
                  />
                  Use SSL
                </label>
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="username">Email Address:</label>
              <input
                id="username"
                type="email"
                className="form-control"
                value={config.username}
                onChange={(e) =>
                  setConfig({ ...config, username: e.target.value })
                }
                placeholder="your.email@example.com"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="password">Password:</label>
              <input
                id="password"
                type="password"
                className="form-control"
                value={config.password}
                onChange={(e) =>
                  setConfig({ ...config, password: e.target.value })
                }
                placeholder="Enter your email password or app password"
                required
              />
              <small className="text-muted">
                For Gmail, you may need to use an App Password instead of your
                regular password.
              </small>
            </div>

            <button
              type="submit"
              disabled={isConnecting}
              className={`button ${
                isConnecting ? "button-secondary" : "button-primary"
              }`}
              style={{ width: "100%" }}
            >
              {isConnecting ? "Connecting..." : "Connect to Email Server"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
