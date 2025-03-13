import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { collection, getDocs, query, orderBy, limit } from 'firebase/firestore';
import { db } from './firebaseConfig';

const Ranking = () => {
  const navigate = useNavigate();
  const [ranking, setRanking] = useState([]);

  useEffect(() => {
    const fetchRanking = async () => {
      try {
        const rankingRef = collection(db, 'ranking');
        const snapshot = await getDocs(query(rankingRef, orderBy('score', 'desc'), limit(10)));
        const data = snapshot.docs.map(doc => doc.data());
        setRanking(data);
      } catch (error) {
        console.error('Erro ao carregar o ranking:', error);
      }
    };

    fetchRanking();
  }, []);

  return (
    <div id="ranking" className="screen">
      <h2 className="screen-title">Ranking</h2>
      <ul id="rankingList">
        {ranking.map((player, index) => (
          <li key={index}>
            {index + 1}. {player.name} - {player.score} pontos
          </li>
        ))}
      </ul>
      <div className="actions-panel">
        <button onClick={() => navigate('/')}>Voltar para o Menu</button>
      </div>
    </div>
  );
};

export default Ranking;
