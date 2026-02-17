import { useState, useEffect } from 'react'
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'


function App() {
  const [count, setCount] = useState(0);
  const [message, setMessage] = useState("");
  const [response, setResponse] = useState<string | null>(null);
  const [emittedText, setEmittedText] = useState("");
  const [connection, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const hubUrl = "/message/responsehub";
    const conn = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    conn.on("ReceiveCharacter", (char: string) => {
      console.log("Received character:", char);
      setEmittedText(prev => prev + char);

    });

    conn.start()
      .then(() => setConnection(conn))
      .catch(err => console.error("SignalR Connection Error:", err));

    return () => {
      conn.stop();
    };
  }, []);

  const handleSend = async () => {
    try {
      const res = await fetch("/message", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ message }),
      });
      if (!res.ok) 
        {
          console.log("Failed to send message", res.status, res.statusText);
          throw new Error("Failed to send message");
        }
      const data = await res.json();
      setResponse(JSON.stringify(data));
    } catch (err: any) {
      setResponse(err.message);
    }
  };

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <input
          type="text"
          value={message}
          onChange={e => setMessage(e.target.value)}
          placeholder="Type your message"
        />
        <button onClick={handleSend} style={{ marginLeft: 8 }}>Send</button>
        <div style={{ marginTop: 12 }}>
          {response && <div>Response: {response}</div>}
          {emittedText && (
            <div style={{ marginTop: 12 }}>
              <strong>Emitted Text:</strong>
              <div style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-all' }}>{emittedText}</div>
            </div>
          )}
        </div>
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App
