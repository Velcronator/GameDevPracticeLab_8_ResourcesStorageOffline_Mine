using System.Collections;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public static UIPanelManager Instance { get; private set; }

    private GameObject currentActiveVisual;
    private UIPanelPriority currentPriority = UIPanelPriority.Low;
    private Coroutine autoHideCoroutine;
    private readonly WaitForSeconds displayDelay = new WaitForSeconds(3f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Requests to show a specific panel visual based on its priority level.
    /// </summary>
    public void RequestShowPanel(GameObject panelVisual, UIPanelPriority priority)
    {
        // If a panel is active and the incoming request has LOWER priority, ignore it
        if (currentActiveVisual != null && priority < currentPriority)
        {
            return;
        }

        // Otherwise, override or refresh the current panel
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
        }

        // Hide the old panel if switching to a new one
        if (currentActiveVisual != null && currentActiveVisual != panelVisual)
        {
            currentActiveVisual.SetActive(false);
        }

        currentActiveVisual = panelVisual;
        currentPriority = priority;

        autoHideCoroutine = StartCoroutine(FlashPanelRoutine());
    }

    private IEnumerator FlashPanelRoutine()
    {
        currentActiveVisual.SetActive(true);

        yield return displayDelay;

        currentActiveVisual.SetActive(false);
        currentActiveVisual = null;
        currentPriority = UIPanelPriority.Low; // Reset priority
    }
}