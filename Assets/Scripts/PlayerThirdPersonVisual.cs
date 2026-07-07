using UnityEngine;

public class PlayerThirdPersonVisual : MonoBehaviour {

    private const string IS_WALKING = "IsWalking";
    private const string IS_PICKING_UP = "Pickup";


    [SerializeField] private Animator animator;
    [SerializeField] private PlayerThirdPersonCharacterController playerThirdPersonCharacterController;

    bool isPickingUp;

    private void Start() {
        UpdateAnimatorParameters();
    }

    private void Update() {
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters() {
        float wasLastRunningTimeMax = .03f;
        float wasLastRunningTime = Time.time - playerThirdPersonCharacterController.GetLastMoveTimer();

        animator.SetBool(IS_WALKING, wasLastRunningTime < wasLastRunningTimeMax);
    }

    public void PlayPickupEffect()
    {
        isPickingUp  = true;
        Debug.Log("playing pickup effect");
        animator.SetTrigger(IS_PICKING_UP);
    }

    public bool FinishedPickup()
    {
        Debug.Log("FinishedPickup from the animation event");
        isPickingUp = false;
        return isPickingUp;
    }

}
