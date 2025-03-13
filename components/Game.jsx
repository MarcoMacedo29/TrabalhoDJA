import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useScore } from './ScoreContext';
import painelsolar from './painelsolar.png';
import infrae from './infra.png';
import politi from './politica.png';
import  prepar from './prep.png';

const Game = () => {
  const navigate = useNavigate();
  const { setScore } = useScore();

  const [resources, setResources] = useState({
    population: 50000,
    health: 80,
    happiness: 90,
    water: 70,
    food: 70,
    energy: 400,
    money: 1000,
    score: 0,
    climate: 'Sol',
    impact: '',
  });

  const [actionAnimation, setActionAnimation] = useState('');
  const [isImageVisible, setIsImageVisible] = useState(false);
  const [imageToShow, setImageToShow] = useState(''); // Armazenar a imagem a ser mostrada

  // Função para verificar se o jogo terminou
  const checkGameOver = () => {
    if (
      resources.health <= 0 ||
      resources.food <= 0 ||
      resources.happiness <= 0 ||
      resources.water <= 0
    ) {
      setScore(resources.score);
      navigate('/gameover');
    }
  };

  const handleButtonClick = (action) => {
    let newResources = { ...resources };
    let newImpact = '';
    let imageToShow = ''; // Definir uma imagem vazia por padrão, ou uma imagem padrão para "mudar turno"
  
    switch (action) {
      case 'solar':
        if (newResources.money >= 50) {
          newResources.energy = newResources.energy + 50;
          newResources.money = newResources.money - 50;
          imageToShow = painelsolar; // Definir a imagem para painel solar
        } else {
          alert('Dinheiro insuficiente para comprar energia solar!');
          return;
        }
        break;
      case 'infra':
        if (newResources.money >= 100) {
          newResources.health = newResources.health + 10;
          newResources.money = newResources.money - 100;
          imageToShow = infrae; // Definir imagem para infraestrutura verde
        } else {
          alert('Dinheiro insuficiente para investir em infraestrutura!');
          return;
        }
        break;
      case 'prep':
        if (newResources.money >= 200) {
          newResources.food = newResources.food + 5;
          newResources.money = newResources.money - 200;
          imageToShow = prepar; // Definir imagem para preparação
        } else {
          alert('Dinheiro insuficiente para investir em preparação!');
          return;
        }
        break;
      case 'policy':
        if (newResources.money >= 300) {
          newResources.happiness = newResources.happiness + 10;
          newResources.money = newResources.money - 300;
          imageToShow = politi; // Definir imagem para políticas climáticas
        } else {
          alert('Dinheiro insuficiente para implementar novas políticas!');
          return;
        }
        break;
      case 'nextTurn':
        // Não define a imagem aqui, para evitar que imagens de outros botões sejam mostradas
        const climates = ['Sol', 'Frio', 'Chuva', 'Tempestade'];
        const randomIndex = Math.floor(Math.random() * climates.length);
        const newClimate = climates[randomIndex];
  
        newResources.climate = newClimate;
        newResources.score = newResources.score + 10;
  
        switch (newClimate) {
          case 'Sol':
            newImpact = '-20 Água / -10 Comida';
            newResources.water = Math.max(newResources.water - 5, 0);
            newResources.food = Math.max(newResources.food - 5, 0);
            break;
          case 'Frio':
            newImpact = '-10 Saúde / -15 Comida';
            newResources.health = Math.max(newResources.health - 10, 0);
            newResources.food = Math.max(newResources.food - 5, 0);
            break;
          case 'Chuva':
            newImpact = '+10 Água / -5 Felicidade';
            newResources.water = Math.min(newResources.water + 10, 100);
            newResources.happiness = Math.max(newResources.happiness - 5, 0);
            break;
          case 'Tempestade':
            newImpact = '-5 Saúde / -30 Energia';
            newResources.health = Math.max(newResources.health - 5, 0);
            newResources.energy = Math.max(newResources.energy - 30, 0);
            break;
          default:
            break;
        }
  
        // Adiciona os 200 apenas na mudança de turno
        newResources.money = newResources.money + 200;
        break;
  
      case 'backToMenu':
        navigate('/');
        return;
  
      default:
        break;
    }
  
    // Garante que o dinheiro não fique negativo
    newResources.money = Math.max(newResources.money, 0);
  
    setResources({
      ...newResources,
      impact: newImpact,
    });
  
    // Defina a animação com a ação do botão
    setActionAnimation(action);
    
    // Se o botão for de "mudar turno", não exibe imagem
    if (action !== 'nextTurn') {
      setImageToShow(imageToShow);
      setIsImageVisible(true);
      
      // Esconde a imagem após 2 segundos
      setTimeout(() => {
        setIsImageVisible(false);
      }, 2000);
    }
  
    checkGameOver();
  };
  

  // Função para renderizar a animação da imagem
  const renderActionImage = () => {
    if (!isImageVisible) return null;

    return (
      <div className="construir-imagem construindo">
        <img src={imageToShow} alt="Construção em andamento" />
      </div>
    );
  };

  // Gera os elementos visuais do clima com base no estado atual
  const renderWeatherEffect = () => {
    switch (resources.climate) {
      case 'Chuva':
        return (
          <div className="rain">
            {Array.from({ length: 100 }).map((_, index) => (
              <div
                key={index}
                className="drop"
                style={{
                  left: `${Math.random() * 100}vw`,
                  animationDelay: `${Math.random() * 2}s`,
                }}
              ></div>
            ))}
          </div>
        );
      case 'Tempestade':
        return (
          <>
            <div className="rain">
              {Array.from({ length: 100 }).map((_, index) => (
                <div
                  key={index}
                  className="drop"
                  style={{
                    left: `${Math.random() * 100}vw`,
                    animationDelay: `${Math.random() * 2}s`,
                  }}
                ></div>
              ))}
            </div>
            <div className="lightning"></div>
          </>
        );
      case 'Frio':
        return (
          <div className="snow">
            {Array.from({ length: 50 }).map((_, index) => (
              <div
                key={index}
                className="snowflake"
                style={{
                  left: `${Math.random() * 100}vw`,
                  animationDuration: `${2 + Math.random() * 3}s`,
                  animationDelay: `${Math.random() * 5}s`,
                }}
              ></div>
            ))}
          </div>
        );
      default:
        return null; // Sol não precisa de efeito
    }
  };

  return (
    <div>
      <div id="game" className="screen">
        {renderWeatherEffect()}
        {renderActionImage()}

        {/* Recursos no lado esquerdo */}
        <div id="resources" className="panel left-panel">
          <p>👥População: <span id="population">{resources.population}</span></p>
          <p>❤️Saúde: <span id="health">{resources.health}%</span></p>
          <p>😊Felicidade: <span id="happiness">{resources.happiness}%</span></p>
          <p>💧Água: <span id="water">{resources.water}%</span></p>
          <p>🍎Comida: <span id="food">{resources.food}%</span></p>
          <p>⚡Energia: <span id="energy">{resources.energy} kW</span></p>
          <p>💰Dinheiro: <span id="money">{resources.money}</span></p>
        </div>

        {/* Clima e pontuação no lado direito */}
        <div id="climateInfo" className="panel right-panel">
          <p>🌟Pontuação: <span id="score">{resources.score}</span></p>
          <p>🌤️Clima Atual: <span id="climate">{resources.climate}</span></p>
          <p>🌧️Impacto do Clima: <span id="impact">{resources.impact}</span></p>
        </div>

        {/* Botões de ação no centro inferior */}
        <div id="actions" className="action-panel">
          <button id="solarButton" onClick={() => handleButtonClick('solar')}>
            Construir Painéis Solares
            <div className="tooltip">
              Energia +50 <br />
              Dinheiro -50€
            </div>
          </button>
          <button id="infraButton" onClick={() => handleButtonClick('infra')}>
            Criar Infraestrutura Verde
            <div className="tooltip">
              Saúde +10 <br />
              Dinheiro -100€
            </div>
          </button>
          <button id="prepButton" onClick={() => handleButtonClick('prep')}>
            Preparar para Desastres
            <div className="tooltip">
              Comida +5 <br />
              Dinheiro -200€
            </div>
          </button>
          <button id="policyButton" onClick={() => handleButtonClick('policy')}>
            Aplicar Políticas Climáticas
            <div className="tooltip">
              Felicidade +10 <br />
              Dinheiro -300€
            </div>
          </button>
        </div>

        {/* Botão de mudar turno */}
        <div id="turnControl">
          <button id="nextTurnButton" onClick={() => handleButtonClick('nextTurn')}>Mudar Turno</button>
        </div>

        {/* Botão para voltar ao menu */}
        <div id="backToMenuControl" style={{ position: 'absolute', bottom: '30px', left: '35px' }}>
          <button id="backToMenuButton" className="back-to-menu" onClick={() => handleButtonClick('backToMenu')}>Acabar Jogo</button>
        </div>
      </div>
    </div>
  );
};

export default Game;
