import React from 'react';
import { useNavigate } from 'react-router-dom'; // Importar o hook para navegação

const About = () => {
  const navigate = useNavigate(); // Hook para redirecionar para o menu

  return (
    <div id="about" className="screen">
      <h2 className="screen-title">Sobre o Jogo</h2>
      <p className="screen-about">
        No âmbito da disciplina de Programação e Desenvolvimento Web, foi-nos
        proposto desenvolver um jogo interativo com foco em uma causa social
        importante, inspirado pelos princípios do movimento <b>Games for Change</b>, 
        onde os jogos são usados como uma ferramenta para impactar positivamente a
        sociedade, educar e inspirar mudanças.
        <br />
        <br />
        <strong>Realizado Por:</strong> João Reis / Miguel Freitas
      </p>
      <div className="actions-panel">
        <button onClick={() => navigate(-1)}>Voltar</button>
      </div>
    </div>
  );
};

export default About;
