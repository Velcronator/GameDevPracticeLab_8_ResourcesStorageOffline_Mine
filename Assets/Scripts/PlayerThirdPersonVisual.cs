using UnityEngine;

public class PlayerThirdPersonVisual : MonoBehaviour {



    private const string IS_WALKING = "IsWalking";


    [SerializeField] private Animator animator;
    [SerializeField] private PlayerThirdPersonCharacterController playerThirdPersonCharacterController;


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

}
