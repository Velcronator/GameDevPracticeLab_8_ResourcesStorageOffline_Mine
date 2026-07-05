using UnityEngine;

public class PlayerActionUI : MonoBehaviour {



    [SerializeField] private Transform inputVisualContainer;


    private void Update() {
        HideInputVisual();

        if (PlayerInteractionSystem.Instance.GetCanInteractable() != null) {
            ShowInputVisual();
        }
    }

    private void ShowInputVisual() {
        inputVisualContainer.gameObject.SetActive(true);
    }

    private void HideInputVisual() {
        inputVisualContainer.gameObject.SetActive(false);
    }


}