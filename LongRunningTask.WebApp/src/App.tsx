import { useState, useEffect } from 'react'
import { HubConnectionBuilder } from '@microsoft/signalr';
import './App.css'

function App() {
  const [message, setMessage] = useState("");
  const [response, setResponse] = useState<string | null>(null);
  const [emittedText, setEmittedText] = useState("");
  const [beingProcessed, setBeingProcessed] = useState(false);
  const [processCompleted, setProcessCompleted] = useState(false);
  const [processId, setProcessId] = useState<string | null>(null);
  const [progress, setProgress] = useState(0);
  const [totalLength, setTotalLength] = useState(0);


  useEffect(() => {

    const checkProcessRunningAtStart = async () => {
      console.log("Checking for running process at start...");
      await checkProcessRunning();
    };

    checkProcessRunningAtStart();

    const hubUrl = "/api/message/responsehub";
    const conn = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    conn.on("ReceiveCharacter", (char: string, userid: string, processId: string, position: number, isLast: boolean) => {
      console.log("Received character:", char, "for user:", userid, "process:", processId, "position:", position, "isLast:", isLast);
      setEmittedText(prev => prev + char);
      setProgress(position + 1);
      if (isLast) {
        setBeingProcessed(false);
        setProcessCompleted(true);
        setProgress(0);
      }
    });


    conn.start()
      .catch(err => console.error("SignalR Connection Error:", err));

    return () => {
      conn.stop();
    };
  }, []);

  const handleCancel = async () => {

    if (!processId) {
      setResponse("No process to cancel.");
      return;
    }

    try {
      setBeingProcessed(false);
      setProgress(0);
      const res = await fetch("/api/message/cancel", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ processId: processId }),
      });
      if (!res.ok) {
        console.log("Failed to cancel process", res.status, res.statusText);
        throw new Error("Failed to cancel process");
      }
    } catch (err: any) {
      setResponse(err.message);
    }
  };

  const checkProcessRunning = async () => {

    const res = await fetch("/process/running", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      }
    });

    if (!res.ok) {
      console.log("Failed to get running message", res.status, res.statusText);
      throw new Error("Failed to get running message");
    }

    res.json().then(data => {
      console.log("Received response:", data);
      
      if (!data.processId) {
        setResponse("No running process found.");
        setBeingProcessed(false);
        setProcessId(null);
        return;
      }

      setResponse(`Running process found. Process ID: ${data.processId}`);
      setBeingProcessed(true);
      setProcessId(data.processId);

    }).catch(err => {
      console.error("Failed to parse response JSON:", err);
      setProcessId(null);
      setBeingProcessed(false);
      setResponse("Message sent but failed to parse response.");
    });
  }

  const handleSend = async () => {
    try {
      setEmittedText("");
      setProcessId(null);
      setProcessCompleted(false);
      setBeingProcessed(true);
      setProgress(0);
      setTotalLength(message.length);
      const res = await fetch("/api/message/", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ message }),
      });
      if (!res.ok) {
        console.log("Failed to send message", res.status, res.statusText);
        throw new Error("Failed to send message");
      }
      res.json().then(data => {
        console.log("Received response:", data);
        setProcessId(data.processId);
        setBeingProcessed(true);
        setResponse(`Message sent successfully. Process ID: ${data.processId}`);
      }).catch(err => {
        console.error("Failed to parse response JSON:", err);
        setProcessId(null);
        setBeingProcessed(false);
        setResponse("Message sent but failed to parse response.");
      });

    } catch (err: any) {
      setResponse(err.message);
    }
  };

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-12 col-lg-10 col-xl-8">
          <div className="text-center mb-4">
            <h1 className="display-4 mb-3">Long Running Task</h1>
            <p className="lead text-muted">Process your messages with real-time progress tracking</p>
          </div>

          <div className="card shadow">
            <div className="card-body p-4">
              <div className="mb-3">
                <label htmlFor="messageInput" className="form-label fw-semibold">
                  Message to Process
                </label>
                <input
                  id="messageInput"
                  type="text"
                  className="form-control form-control-lg"
                  value={message}
                  onChange={e => setMessage(e.target.value)}
                  placeholder="Enter your message here..."
                  disabled={beingProcessed}
                />
              </div>

              <div className="d-flex gap-2 mb-3 flex-wrap">
                <button 
                  onClick={handleSend} 
                  className="btn btn-primary btn-lg flex-grow-1 flex-sm-grow-0" 
                  disabled={beingProcessed || !message.trim()}
                >
                  <i className="bi bi-send me-2"></i>
                  Send
                </button>
                <button 
                  onClick={handleCancel} 
                  className="btn btn-danger btn-lg flex-grow-1 flex-sm-grow-0" 
                  disabled={!beingProcessed}
                >
                  <i className="bi bi-x-circle me-2"></i>
                  Cancel
                </button>
              </div>

              {beingProcessed && totalLength > 0 && (
                <div className="mb-3">
                  <div className="d-flex justify-content-between align-items-center mb-2">
                    <small className="text-muted fw-semibold">Processing Progress</small>
                    <span className="badge bg-primary">
                      {progress} / {totalLength} ({Math.round((progress / totalLength) * 100)}%)
                    </span>
                  </div>
                  <div className="progress" style={{ height: '25px' }}>
                    <div 
                      className="progress-bar progress-bar-striped progress-bar-animated"
                      role="progressbar"
                      style={{ width: `${(progress / totalLength) * 100}%` }}
                      aria-valuenow={progress}
                      aria-valuemin={0}
                      aria-valuemax={totalLength}
                    >
                      {Math.round((progress / totalLength) * 100)}%
                    </div>
                  </div>
                </div>
              )}

              {response && (
                <div className="alert alert-info d-flex align-items-start" role="alert">
                  <i className="bi bi-info-circle-fill me-2 mt-1"></i>
                  <div className="flex-grow-1">
                    <strong>Response:</strong> {response}
                  </div>
                </div>
              )}

              {emittedText && (
                <div className="mt-3">
                  <div className="card bg-light">
                    <div className="card-header bg-secondary text-white">
                      <i className="bi bi-file-text me-2"></i>
                      <strong>Processed Text</strong>
                    </div>
                    <div className="card-body">
                      <div className="processed-text" style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
                        {emittedText}
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>

          {processCompleted && (
            <div className="text-center mt-4">
              <div className="alert alert-success d-inline-block" role="alert">
                <i className="bi bi-check-circle-fill me-2"></i>
                Text has been processed successfully!
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default App
