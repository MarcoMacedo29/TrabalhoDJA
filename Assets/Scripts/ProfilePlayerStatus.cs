using UnityEngine;

public class ProfilePlayerStatus : MonoBehaviour
{
    public GameObject PlayerStatusCanvas;

    public void Start()
    {
        if (PlayerStatusCanvas != null && PlayerStatusCanvas.activeSelf)
        {
            PlayerStatusCanvas.SetActive(false);
        }
    }

    public void TogglePlayerStatus()
    {
        PlayerStatusCanvas.SetActive(!PlayerStatusCanvas.activeSelf);
    }
}
