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
      if (isLast) {
        setBeingProcessed(false);
        setProcessCompleted(true);
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
    <>
      <h1>Long Running Task</h1>
      <div className="card">
        <input
          type="text"
          value={message}
          onChange={e => setMessage(e.target.value)}
          placeholder="Type your message"
          disabled={beingProcessed === true}
        />
        <button onClick={handleSend} style={{ marginLeft: 8 }} disabled={beingProcessed === true}>Send</button>
        <button onClick={handleCancel} style={{ marginLeft: 8 }} disabled={beingProcessed === false}>Cancel</button>
        <div style={{ marginTop: 12 }}>
          {response && <div>Response: {response}</div>}
          {emittedText && (
            <div style={{ marginTop: 12 }}>
              <strong>Processed Text:</strong>
              <div style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-all' }}>{emittedText}</div>
            </div>
          )}
        </div>
      </div>
      {
        processCompleted &&
        <p className="read-the-docs">
          Text Has been processed!
        </p>
      }
    </>
  )
}

export default App
