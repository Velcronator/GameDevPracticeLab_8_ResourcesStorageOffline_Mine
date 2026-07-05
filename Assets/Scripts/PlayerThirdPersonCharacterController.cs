using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThirdPersonCharacterController : MonoBehaviour {


    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityForce;


    private CharacterController characterController;
    private float characterVelocityY;
    private float lastMoveTimer;


    private void Awake() {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        HandleCharacterMovement();
    }

    private void HandleCharacterMovement() {
        float moveX = 0;
        float moveZ = 0;

        if (Keyboard.current.wKey.isPressed) {
            moveZ = +1f;
        }
        if (Keyboard.current.sKey.isPressed) {
            moveZ = -1f;
        }
        if (Keyboard.current.aKey.isPressed) {
            moveX = -1f;
        }
        if (Keyboard.current.dKey.isPressed) {
            moveX = +1f;
        }

        Vector3 characterVelocity = Camera.main.transform.right * moveX + Camera.main.transform.forward * moveZ;
        characterVelocity.y = 0f;
        characterVelocity = characterVelocity.normalized * moveSpeed;

        if (characterVelocity != Vector3.zero) {
            lastMoveTimer = Time.time;
        }

        float rotationSpeed = 20f;
        transform.forward = Vector3.Slerp(transform.forward, characterVelocity, rotationSpeed * Time.deltaTime);

        if (characterController.isGrounded) {
            characterVelocityY = 0f;
            // Jump
            if (IsInputJumpDown()) {
                characterVelocityY = jumpForce;
            }
        }

        characterVelocityY += gravityForce * Time.deltaTime;

        characterVelocity.y = characterVelocityY;

        characterController.Move(characterVelocity * Time.deltaTime);
    }

    public float GetLastMoveTimer() {
        return lastMoveTimer;
    }

    private bool IsInputJumpDown() {
        return Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    public void Move(Vector3 moveVector) {
        characterController.Move(moveVector * Time.deltaTime);
    }

}
