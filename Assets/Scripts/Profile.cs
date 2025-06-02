using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Profile : MonoBehaviour
{
    public GameObject PlayerStatusCanvas;
    public GameObject inventoryCanvas;
    public CanvasGroup bottomBarCanvasGroup; 
    public float fadeDuration = 0.5f;
    public float fadeDelay = 1f;
    private Coroutine fadeCoroutine;

    public GameObject inventoryPanel; 
    public GameObject craftingPanel; 


    private void Start()
    {
        if (PlayerStatusCanvas != null)
            PlayerStatusCanvas.SetActive(false);

        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);

        if (bottomBarCanvasGroup != null)
            bottomBarCanvasGroup.alpha = 1; 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI())
            {
                CloseAllCanvases();
            }
        }
    }

    public void ToggleInventory()
    {
        bool isInventoryOpen = inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(!isInventoryOpen);

        if (!isInventoryOpen)
        {
            ShowInventoryPanelA();

            if (PlayerStatusCanvas.activeSelf)
            {
                PlayerStatusCanvas.SetActive(false);
            }
        }


        HandlePauseState();
    }

    public void TogglePlayerStatus()
    {
        bool isPlayerStatusOpen = PlayerStatusCanvas.activeSelf;
        PlayerStatusCanvas.SetActive(!isPlayerStatusOpen);

        if (!isPlayerStatusOpen && inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
        }


        HandlePauseState();
    }

    public void ShowInventoryPanelA()
    {
        inventoryPanel.SetActive(true);
        craftingPanel.SetActive(false);
    }

    public void ShowInventoryPanelB()
    {
        inventoryPanel.SetActive(false);
        craftingPanel.SetActive(true);
    }
        
    private void HandlePauseState()
    {
        if (inventoryCanvas.activeSelf || PlayerStatusCanvas.activeSelf)
        {
            GameManager.Instance.PauseGame();
        }
        else
        {
            GameManager.Instance.ResumeGame();
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void CloseAllCanvases()
    {
        inventoryCanvas.SetActive(false);
        PlayerStatusCanvas.SetActive(false);
        HandlePauseState();
    }

    public void OnMouseEnterBottomBar()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(bottomBarCanvasGroup, 1, fadeDuration));
    }

    public void OnMouseExitBottomBar()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(HideBottomBarAfterDelay());
    }

    private IEnumerator HideBottomBarAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);
        yield return StartCoroutine(FadeCanvasGroup(bottomBarCanvasGroup, 0, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
