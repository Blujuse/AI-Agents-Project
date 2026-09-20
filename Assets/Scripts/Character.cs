using System.Collections.Specialized;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;

    [Header("Camera")]
    private Transform camTransform;

    [Header("Player")]
    private Transform playerTransform;
    public float speed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Locks the cursor to the center of the screen and hides it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Gets character controller component on object
        controller = GetComponent<CharacterController>();

        // Gets the camera transform from its tag
        camTransform = GameObject.FindWithTag("MainCamera").transform;

        // Assigns the player transform
        playerTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets the forward and right vectors of the camera
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;

        // Sets y to 0 to prevent vertical movement on player
        camForward.y = 0;
        camRight.y = 0;

        // Normalizes the vectors
        Vector3 forwardRelative = Input.GetAxis("Vertical") * camForward;
        Vector3 rightRelative = Input.GetAxis("Horizontal") * camRight;

        // Combines the two vectors to get the final movement vector
        Vector3 moveRelative = forwardRelative + rightRelative;

        // Moves the character controller
        Vector3 move = new Vector3(moveRelative.x, 0, moveRelative.z);

        // Uses built in SimpleMove function to move the character controller
        controller.SimpleMove(move * speed);

        // Rotate the player to face the movement direction
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}