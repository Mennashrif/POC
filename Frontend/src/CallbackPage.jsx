import { useEffect, useRef, useState } from 'react'

export default function CallbackPage() {
  const [status, setStatus] = useState('Exchanging code...')
  const [error, setError]   = useState(null)
  const exchanged = useRef(false)

  useEffect(() => {
    if (exchanged.current) return
    exchanged.current = true

    const params      = new URLSearchParams(window.location.search)
    const code        = params.get('code')
    const redirectUri = `${window.location.origin}/callback`

    if (!code) {
      setError('No authorization code in URL')
      return
    }

    fetch('/api/account/exchange', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ code, redirectUri })
    })
      .then(res => {
        if (!res.ok) throw new Error('Exchange failed')
        return res.json()
      })
      .then(data => {
        localStorage.setItem('access_token', data.accessToken)
        setStatus('Success! Redirecting...')
        window.location.href = '/'
      })
      .catch(() => setError('Failed to exchange code. Please try again.'))
  }, [])

  return (
    <div style={s.container}>
      <div style={s.card}>
        {error
          ? <>
              <h2 style={s.title}>Login Failed</h2>
              <p style={s.error}>{error}</p>
              <button onClick={() => window.location.href = '/'} style={s.button}>Back to Login</button>
            </>
          : <>
              <h2 style={s.title}>Authenticating</h2>
              <p style={s.hint}>{status}</p>
            </>
        }
      </div>
    </div>
  )
}

const s = {
  container: { minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' },
  card:      { background: '#fff', padding: '40px', borderRadius: '8px', boxShadow: '0 2px 12px rgba(0,0,0,0.1)', width: '360px' },
  title:     { marginBottom: '12px', fontSize: '22px', color: '#1a1a2e' },
  hint:      { fontSize: '14px', color: '#555' },
  error:     { color: '#c0392b', fontSize: '13px', marginBottom: '16px' },
  button:    { width: '100%', padding: '11px', background: '#1a1a2e', color: '#fff', border: 'none', borderRadius: '4px', fontSize: '15px', cursor: 'pointer' },
}
