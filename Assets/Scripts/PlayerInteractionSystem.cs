using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionSystem : MonoBehaviour {


    public static PlayerInteractionSystem Instance { get; private set; }


    private IInteractable canInteractable;


    private void Awake() {
        Instance = this;
    }

    private void Update() {
        HandleCanInteractable();

        if (Keyboard.current.eKey.wasPressedThisFrame) {
            if (canInteractable != null) {
                canInteractable.Interact();
            }
        }
    }

    public void HandleCanInteractable() {
        canInteractable = null;

        float interactDistance = 1.5f;
        RaycastHit[] raycastHitArray = Physics.RaycastAll(transform.position, transform.forward, interactDistance);
        foreach (RaycastHit raycastHit in raycastHitArray) {
            if (raycastHit.transform.TryGetComponent(out IInteractable interactable)) {
                canInteractable = interactable;
                break;
            }
        }
    }

    public IInteractable GetCanInteractable() {
        return canInteractable;
    }

}