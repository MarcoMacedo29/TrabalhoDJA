import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Settings = () => {
  const navigate = useNavigate(); // Hook para navegação
  const [volume, setVolume] = useState(50); // Estado para controlar o volume

  // Atualiza o estado do volume ao mover o controle
  const handleVolumeChange = (e) => {
    setVolume(e.target.value);
  };

  return (
    <div id="settings" className="screen active">
      <h2 className="screen-title">Configurações</h2>

      <div className="settings-option">
        <label htmlFor="volumeControl">Volume:</label>
        <input
          type="range"
          id="volumeControl"
          min="0"
          max="100"
          value={volume}
          onChange={handleVolumeChange}
        />
        <span id="volumeValue">{volume}%</span>
      </div>

      <div className="actions-panel">
        <button onClick={() => navigate(-1)}>Voltar</button>
      </div>
    </div>
  );
};

export default Settings;
