import { useState } from 'react';

function App() {
  const [text, setText] = useState('');
  const [result, setResult] = useState('');

  const checkText = async () => {
    const res = await fetch('http://localhost:5091/api/prediction', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ text })
    });
    const data = await res.json();
    setResult(data.result);
  };

  return (
    <div style={{ padding: 20 }}>
      <h1>Detector de Malas Palabras</h1>
      <input
        type="text"
        placeholder="Escribe un comentario..."
        value={text}
        onChange={(e) => setText(e.target.value)}
      />
      <button onClick={checkText}>Analizar</button>
      {result && <p>Resultado: {result}</p>}
    </div>
  );
}

export default App;
