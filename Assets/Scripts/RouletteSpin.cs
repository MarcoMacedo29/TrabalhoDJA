using UnityEngine;
using System.Collections;

public class RouletteSpin : MonoBehaviour
{
    public float spinSpeed = 200f;  
    private bool isSpinning = false; 
    private float rotationAngle = 0f;  


    public void StartSpin()
    {
        if (!isSpinning)  
        {
            isSpinning = true;
            rotationAngle = 0f;  
            StartCoroutine(SpinRoulette());
        }
    }


    private IEnumerator SpinRoulette()
    {
        float spinDuration = 3f;  
        float timeElapsed = 0f;


        while (timeElapsed < spinDuration)
        {

            float angleThisFrame = spinSpeed * Time.deltaTime;
            rotationAngle += angleThisFrame;
            transform.Rotate(0f, 0f, angleThisFrame);  

            timeElapsed += Time.deltaTime;
            yield return null;  
        }

        
        isSpinning = false;

        
    }
}
