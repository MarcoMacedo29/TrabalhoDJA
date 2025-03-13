import React from 'react';
import { useNavigate } from 'react-router-dom'; // Importar o hook para navegação

const Rules = () => {
  const navigate = useNavigate(); // Hook para redirecionar para o menu

  return (
    <div id="rules" className="screen">
      <h2 className="screen-title">Regras do Jogo</h2>
      <div className="regras-text">
        <p>
          <strong>1. Objetivo:</strong>
          <br />
          O objetivo é gerenciar recursos e garantir que sua população sobreviva o
          maior número de turnos em diferentes condições climáticas. Quanto mais
          turnos sobreviver, maior pontuação terá.
        </p>

        <p>
          <strong>2. Recursos:</strong>
          <br />
          <strong>- População:</strong> Reflete o número de habitantes em sua cidade.
          A população pode aumentar ou diminuir com base nos fatores saúde e felicidade.
          <br />
          <strong>- Saúde/Água/Comida:</strong> São essenciais para a sobrevivência.
          Se qualquer um deles chegar a zero, você perde o jogo.
          <br />
          <strong>- Felicidade:</strong> Afeta o crescimento da população. 
          Se estiver baixa, pode levar à redução da população.
          <br />
          <strong>- Energia:</strong> Usada para várias infraestruturas. 
          Se acabar, pode afetar a saúde e o bem-estar da população.
          <br />
          <strong>- Dinheiro:</strong> Serve para comprar melhorias e garantir a manutenção
          dos recursos. Ganha-se conforme a quantidade de população a cada turno.
          <br />
          <strong>- Pontuação:</strong> Calculada com base nos turnos jogados.
        </p>

        <p>
          <strong>3. Ações Disponíveis:</strong>
          <br />
          - Construir Painéis Solares: Aumenta a produção de energia em troca de dinheiro.
          <br />
          - Criar Infraestrutura Verde: Melhora a saúde da população em troca de dinheiro.
          <br />
          - Preparar para Desastres: Compra comida para a população em troca de dinheiro.
          <br />
          - Aplicar Políticas Climáticas: Aumenta a felicidade da população em troca de dinheiro.
        </p>

        <p>
          <strong>4. Mudanças Climáticas:</strong>
          <br />
          - Sol: Diminui água e comida.
          <br />
          - Chuva: Aumenta a água, mas diminui a felicidade.
          <br />
          - Tempestade: Diminui saúde e energia.
          <br />
          - Frio: Diminui comida, mas melhora a saúde.
        </p>

        <p>
          <strong>5. Ranking:</strong>
          <br />
          Após a derrota, você poderá salvar sua pontuação e compará-la com outros
          jogadores. As melhores pontuações serão registradas no ranking e salvas
          localmente no seu navegador.
        </p>
      </div>
      
      <div className="actions-panel">
        <button onClick={() => navigate(-1)}>Voltar</button>
      </div>
   
    </div>
  );
};

export default Rules;
