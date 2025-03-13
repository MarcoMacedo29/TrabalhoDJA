import React, { createContext, useContext, useState, useEffect } from 'react';

const RankingContext = createContext();

export const RankingProvider = ({ children }) => {
  const [ranking, setRanking] = useState([]);


  useEffect(() => {
    const storedRanking = JSON.parse(localStorage.getItem('ranking')) || [];
    setRanking(storedRanking);
  }, []);

  
  useEffect(() => {
    localStorage.setItem('ranking', JSON.stringify(ranking));
  }, [ranking]);

  const addToRanking = (name, score) => {
    setRanking((prevRanking) => {
      const existingPlayer = prevRanking.find((player) => player.name === name);

      if (existingPlayer) {
        // Atualizar apenas se a nova pontuação for maior
        if (score > existingPlayer.score) {
          existingPlayer.score = score;
        }
      } else {
        // Adicionar novo jogador
        prevRanking.push({ name, score });
      }

      // Ordenar por pontuação (decrescente) e limitar a 10 jogadores
      return [...prevRanking]
        .sort((a, b) => b.score - a.score)
        .slice(0, 10);
    });
  };

  return (
    <RankingContext.Provider value={{ ranking, addToRanking }}>
      {children}
    </RankingContext.Provider>
  );
};

export const useRanking = () => {
  const context = useContext(RankingContext);
  if (!context) {
    throw new Error("useRanking deve ser usado dentro de um RankingProvider.");
  }
  return context;
};
