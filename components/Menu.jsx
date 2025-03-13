import React from 'react';
import { useNavigate } from 'react-router-dom';

const Menu = () => {
  const navigate = useNavigate(); // Hook para navegação

  return (
    <div id="menu" className="screen">
      <h1>Eco Survivor</h1>
      <button id="playButton" onClick={() => navigate('/game')}>Jogar</button>
      <button id="rulesButton" onClick={() => navigate('/rules')}>Regras</button>
      <button id="rankingButton" onClick={() => navigate('/ranking')}>Ranking</button>
      <button id="exitButton" onClick={() => window.close()}>Sair</button>
      <button id="settingsButton" onClick={() => navigate('/settings')}>⚙️</button>
      <button id="aboutButton" onClick={() => navigate('/about')}>❔</button>
    </div>
  );
};

export default Menu;
