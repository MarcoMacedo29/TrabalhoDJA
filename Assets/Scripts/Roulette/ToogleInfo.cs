using UnityEngine;
using UnityEngine.UI;

public class ToggleInfo : MonoBehaviour
{
    public CanvasGroup contentGroup;

    public GameObject infoText;

    void Start()
    {
        if (contentGroup == null) Debug.LogError("ContentGroup not assigned!");
        if (infoText == null) Debug.LogError("InfoText not assigned!");

        infoText.SetActive(false);
        contentGroup.interactable = true;
        contentGroup.blocksRaycasts = true;
    }

    public void OnSpinInformationClicked()
    {
        bool show = !infoText.activeSelf;
        infoText.SetActive(show);

        contentGroup.interactable = !show;
        contentGroup.blocksRaycasts = !show;
    }
}