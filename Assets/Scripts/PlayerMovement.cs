using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float xInput, zInput, xRotation;
    private Vector3 move, velocity;
    private bool isGronded;
    private CharacterController controller;
    [SerializeField] LayerMask grondMask;
    [SerializeField] private Transform playerCamera, grondCheck;
    [SerializeField] private float gravity = -9.81f, grondDist = 0.4f;
    // PHYS SHIT
    [SerializeField] private float speed = 5f, jumpHeight = 3f;
    // MOVEMENT STATS
    [SerializeField] private float mouseX, mouseY, mouseSens = 10f;
    // MOUSE

    // vvv DEBUG STUFF REMEMBER TO REMOVE vvv
    private string[] debug = new string[100];
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // GROND CHECK ==========================================================================================
        isGronded = Physics.CheckSphere(grondCheck.position, grondDist, grondMask);

        if (isGronded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Looking ==============================================================================================
        mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(0f, 0f, xRotation);
        transform.Rotate(Vector3.up * mouseX);

        // Movement =============================================================================================
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        // Movement
        move = transform.right * xInput + transform.forward * zInput;
        controller.Move(move * speed * Time.deltaTime);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGronded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity ==============================================================================================
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // TODO: REMOVE THIS DEBUG 
        debug[0] = $"Mouse\t: {mouseX}, {mouseY}";
        debug[1] = $"Head\t: {playerCamera.localRotation}";
        debug[2] = $"Axis Move\t: {xInput}, {zInput}";
        // debug[3] = $"Transform\t: {body.velocity}";
    }

    void OnGUI()
    {
        GUI.Label(
           new Rect(
               5,                   // x, left offset
               Screen.height - 150, // y, bottom offset
               300f,                // width
               150f                 // height
           ),
           string.Join("\n", debug),    // the display text
           GUI.skin.textArea            // use a multi-line text area
        );
    }
}
