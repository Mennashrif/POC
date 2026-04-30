import { useEffect, useState } from 'react'
import * as signalR from '@microsoft/signalr'
import CallbackPage from './CallbackPage'

export default function App() {
  const isCallback = window.location.pathname === '/callback'
  if (isCallback) return <CallbackPage />

  return (
    <>
      <ReservationNotification />
      <LoginPage />
    </>
  )
}

// ─── SignalR Notification Popup ───────────────────────────────────────────────

function ReservationNotification() {
  const [notification, setNotification] = useState(null)
  const [connected, setConnected]       = useState(false)

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/notifications')
      .withAutomaticReconnect()
      .build()

    connection.on('ReservationCreated', (data) => {
      setNotification(data)
      setTimeout(() => setNotification(null), 10000)
    })

    connection.onreconnecting(() => setConnected(false))
    connection.onreconnected(()  => setConnected(true))
    connection.onclose(()        => setConnected(false))

    connection.start()
      .then(() => setConnected(true))
      .catch((err) => console.error('SignalR error:', err))

    return () => { connection.stop() }
  }, [])

  return (
    <div style={n.wrapper}>
      <div style={{ ...n.dot, background: connected ? '#27ae60' : '#e74c3c' }}
           title={connected ? 'Notifications connected' : 'Notifications disconnected'} />

      {!notification ? null : <div style={n.popup}>
        <div style={n.header}>
          <span style={n.icon}>🔔</span>
          <strong style={n.title}>New Reservation</strong>
        </div>
        <p style={n.row}><span style={n.lbl}>Guest</span>{notification.guestName}</p>
        <p style={n.row}><span style={n.lbl}>Check-in</span>{notification.checkIn}</p>
        <p style={n.row}><span style={n.lbl}>Check-out</span>{notification.checkOut}</p>
        <div style={n.bar}>
          <div style={n.progress} />
        </div>
      </div>}
    </div>
  )
}

const n = {
  wrapper:  { position: 'fixed', bottom: '24px', right: '24px', zIndex: 9999, display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: '8px' },
  dot:      { width: '10px', height: '10px', borderRadius: '50%', alignSelf: 'flex-end' },
  popup:    { background: '#fff', border: '1px solid #e0e0e0', borderLeft: '4px solid #27ae60',
              borderRadius: '6px', padding: '16px 20px', width: '280px',
              boxShadow: '0 4px 16px rgba(0,0,0,0.12)' },
  header:   { display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '10px' },
  icon:     { fontSize: '18px' },
  title:    { fontSize: '15px', color: '#1a1a2e' },
  row:      { margin: '4px 0', fontSize: '13px', color: '#444', display: 'flex', gap: '8px' },
  lbl:      { color: '#888', minWidth: '70px' },
  bar:      { marginTop: '12px', height: '3px', background: '#eee', borderRadius: '2px', overflow: 'hidden' },
  progress: { height: '100%', width: '100%', background: '#27ae60',
              animation: 'drain 10s linear forwards' }
}

// ─── Login Page ───────────────────────────────────────────────────────────────

function LoginPage() {
  const [loading, setLoading] = useState(false)
  const [error, setError]     = useState(null)
  const token = localStorage.getItem('access_token')

  async function handleLogin() {
    setLoading(true)
    setError(null)
    try {
      const res  = await fetch('/api/account/login-url')
      const data = await res.json()
      window.location.href = data.url
    } catch {
      setError('Could not reach the server')
      setLoading(false)
    }
  }

  function handleLogout() {
    localStorage.removeItem('access_token')
    window.location.reload()
  }

  if (token) {
    return (
      <div style={s.container}>
        <div style={s.card}>
          <h2 style={s.title}>Logged in</h2>
          <p style={s.success}>Login successful</p>
          <label style={s.label}>Access Token</label>
          <textarea readOnly value={token} style={s.tokenBox} />
          <button onClick={handleLogout} style={s.button}>Logout</button>
        </div>
      </div>
    )
  }

  return (
    <div style={s.container}>
      <div style={s.card}>
        <h2 style={s.title}>eSAP Login</h2>
        <p style={s.hint}>You will be redirected to the login page</p>
        {error && <p style={s.error}>{error}</p>}
        <button onClick={handleLogin} style={s.button} disabled={loading}>
          {loading ? 'Redirecting...' : 'Login with Keycloak'}
        </button>
      </div>
    </div>
  )
}

const s = {
  container: { minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' },
  card:      { background: '#fff', padding: '40px', borderRadius: '8px', boxShadow: '0 2px 12px rgba(0,0,0,0.1)', width: '360px' },
  title:     { marginBottom: '12px', fontSize: '22px', color: '#1a1a2e' },
  hint:      { fontSize: '13px', color: '#777', marginBottom: '24px' },
  label:     { display: 'block', marginBottom: '6px', fontSize: '13px', color: '#555' },
  button:    { width: '100%', padding: '11px', background: '#1a1a2e', color: '#fff', border: 'none', borderRadius: '4px', fontSize: '15px', cursor: 'pointer', marginTop: '8px' },
  error:     { color: '#c0392b', fontSize: '13px', marginBottom: '12px' },
  success:   { color: '#27ae60', fontSize: '13px', marginBottom: '12px' },
  tokenBox:  { width: '100%', height: '120px', fontSize: '11px', padding: '8px', borderRadius: '4px', border: '1px solid #ccc', resize: 'none', marginBottom: '16px', wordBreak: 'break-all' },
}
