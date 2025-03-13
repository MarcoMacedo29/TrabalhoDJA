import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useScore } from './ScoreContext';
import { collection, addDoc, getDocs, query, orderBy, doc, updateDoc } from 'firebase/firestore';
import { db } from './firebaseConfig';

const GameOver = () => {
  const navigate = useNavigate();
  const { score } = useScore();
  const [playerName, setPlayerName] = useState('');

  const handleSaveScore = async () => {
    if (!playerName.trim()) {
      alert('Por favor, insira um nome antes de salvar.');
      return;
    }

    try {
      const rankingRef = collection(db, 'ranking');

      // Verificar se o jogador já existe no ranking
      const snapshot = await getDocs(query(rankingRef, orderBy('score', 'desc')));
      const existingPlayer = snapshot.docs
        .map(doc => ({ id: doc.id, ...doc.data() }))
        .find(entry => entry.name.toLowerCase() === playerName.toLowerCase());

      if (existingPlayer) {
        if (score > existingPlayer.score) {
          // Atualizar a pontuação do jogador
          await updateDoc(doc(db, 'ranking', existingPlayer.id), { score });
        }
      } else {
        // Adicionar novo jogador
        await addDoc(rankingRef, { name: playerName, score });
      }

      navigate('/ranking'); // Ir para a tela de ranking
    } catch (error) {
      console.error('Erro ao salvar no Firestore:', error);
    }
  };

  return (
    <div id="gameOver" className="screen">
      <h1 className="screen-over">GAME OVER</h1>
      <p className="screen-text">
        Pontuação: <span id="finalScore">{score}</span>
      </p>
      <div id="saveScoreForm">
        <input
          type="text"
          id="playerName"
          placeholder="Seu Nome"
          maxLength={10}
          value={playerName}
          onChange={(e) => setPlayerName(e.target.value)}
        />
        <button id="saveScoreButton" className="screen-salvar" onClick={handleSaveScore}>
          Salvar Pontuação
        </button>
      </div>
      <div className="actions-panel">
        <button onClick={() => navigate('/')}>Voltar para o Menu</button>
      </div>
    </div>
  );
};

export default GameOver;
