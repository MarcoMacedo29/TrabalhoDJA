using UnityEngine;
using System.Collections;

public class RouletteSpin : MonoBehaviour
{
    public float spinSpeed = 200f;  // A velocidade com que a roleta gira
    private bool isSpinning = false;  // Para evitar que a roleta gire várias vezes ao clicar várias vezes
    private float rotationAngle = 0f;  // O ângulo atual da roleta

    // Função que é chamada ao pressionar o botão de "Spin"
    public void StartSpin()
    {
        if (!isSpinning)  // Impede múltiplos giros simultâneos
        {
            isSpinning = true;
            rotationAngle = 0f;  // Reseta o ângulo da roleta
            StartCoroutine(SpinRoulette());
        }
    }

    // Corrotina para girar a roleta
    private IEnumerator SpinRoulette()
    {
        float spinDuration = 3f;  // Tempo que a roleta vai girar (3 segundos)
        float timeElapsed = 0f;

        // Enquanto o tempo total de rotação não for alcançado
        while (timeElapsed < spinDuration)
        {
            // Cada frame, a roleta gira por uma quantidade de ângulo
            float angleThisFrame = spinSpeed * Time.deltaTime;
            rotationAngle += angleThisFrame;
            transform.Rotate(0f, 0f, angleThisFrame);  // Rotaciona no eixo Z (para o 2D)

            timeElapsed += Time.deltaTime;
            yield return null;  // Espera um frame
        }

        // Após o giro, podemos definir o estado de "parado"
        isSpinning = false;

        // Aqui você pode adicionar lógica para determinar o prêmio ou efeito da roleta.
    }
}
