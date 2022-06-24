using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float xRotation;
    private float mouseX, mouseY, charHeight;
    private Vector3 move, velocity;
    [SerializeField] private bool isGronded, jumped;
    private CharacterController controller;
    [SerializeField] LayerMask grondMask;
    [SerializeField] private Transform playerCamera, grondCheck;
    // INPUTS
    [SerializeField] private AgentController _playerController;
    [SerializeField] private float gravity = -9.81f, gravMod = 1f, grondDist = 0.4f;
    // PHYS STUFF
    [SerializeField] private float speed = 5f, jumpHeight = 3f, jumpDelay;
    // MOVEMENT STATS
    [SerializeField] private float mouseSens = 10f, crouchMod = 0.5f;
    // CROUCHING STATS
    [SerializeField] private float crouchHeight = 0.6f;
    // MOUSE

    private void Start() {
        jumped = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        charHeight = transform.localScale.y;
    }

    private void Update() {
        // GROND CHECK ==========================================================================================
        isGronded = Physics.CheckSphere(grondCheck.position, grondDist, grondMask);

        if (isGronded && velocity.y < 0) {
            velocity.y = -2f;
        }

        // Gravity ==============================================================================================
        velocity.y += gravity * gravMod * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // TODO: Implement freezing delay

        // Looking ==============================================================================================
        mouseX = _playerController.mouseInputX * mouseSens;
        mouseY = _playerController.mouseInputY * mouseSens * -1f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(-180f, 0f, xRotation);
        transform.Rotate(Vector3.up * mouseX);

        // Movement =============================================================================================
        move = transform.right * _playerController.xInput + transform.forward * _playerController.zInput;
        controller.Move(move * speed * (_playerController.crouchInput ? crouchMod : 1f) * Time.deltaTime);

        // Jumping
        if (_playerController.jumpInput && isGronded && !jumped) {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity * gravMod);
            jumped = true;
            Invoke("allowJump", jumpDelay);
        }

        // Crouching
        transform.localScale = new Vector3(transform.localScale.x, _playerController.crouchInput ? crouchHeight : charHeight, transform.localScale.y);
    }

    private void allowJump() {
        jumped = false;
    }
}
