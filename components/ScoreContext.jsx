import React, { createContext, useContext, useState } from 'react';

const ScoreContext = createContext(); // Cria o contexto

export const ScoreProvider = ({ children }) => {
  const [score, setScore] = useState(0); // Estado compartilhado

  return (
    <ScoreContext.Provider value={{ score, setScore }}>
      {children}
    </ScoreContext.Provider>
  );
};

// Hook para consumir o contexto
export const useScore = () => {
  const context = useContext(ScoreContext);
  if (!context) {
    throw new Error("useScore deve ser usado dentro de um ScoreProvider");
  }
  return context;
};
