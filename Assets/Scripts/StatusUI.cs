using UnityEngine;
using TMPro;

public class StatusUI : MonoBehaviour
{
    public TMP_Text healthText; 
    public PlayerStatus playerStatus; 

    private void Update()
    {
        if (playerStatus != null && healthText != null)
        {
            healthText.text = $"Health: {playerStatus.currentHealth} / {playerStatus.MaxHealth}";
        }
    }
}
