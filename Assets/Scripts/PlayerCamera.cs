using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    Transform playerTransform;
    InputAction moveAction;
    Camera cam;
    [SerializeField] float deadZone = 0.2f;
    [SerializeField] float rotationSpeed = 10f;
    float horizontalAngle = 0f;

    public float angleX;
    public float angleY;
    public float radius = 10;
    public bool invertY = false;

    void Start()
    {
        //playerTransform = transform.parent;
        playerTransform = GameObject.Find("Player").transform;

        cam = GetComponent<Camera>();

        //PlayerController controller = GetComponentInParent<PlayerController>();
        PlayerController controller = GameObject.Find("Player").GetComponent<PlayerController>();

        //moveAction = controller.controls.FindAction("MoveCamera");
    }
    private void LateUpdate()
    { 
        //var moveVector = moveAction.ReadValue<Vector3>().normalized;

        //Debug.Log(moveVector);

        //if(moveVector.x > deadZone || moveVector.x < -deadZone)
        //{
        //    //horizontalAngle += moveVector.x * rotationSpeed;
        //    //Vector3 newRotation = new Vector3(transform.rotation.x, horizontalAngle, transform.rotation.z);
        //    //transform.eulerAngles = newRotation;



        //}
        //if (moveVector.y > deadZone || moveVector.y < -deadZone)
        //{

        //}

        //transform.RotateAround(playerTransform.position, Vector3.up, moveVector.x * rotationSpeed);
        //transform.RotateAround(playerTransform.position, transform.right, moveVector.y * rotationSpeed);

        // using Input.GetAxis works much better, it aligns the rotation with your mouse movement convincingly
        angleX += Input.GetAxis("Mouse X");
        //angleX += moveVector.x;
        angleY = invertY? Mathf.Clamp(angleY -= Input.GetAxis("Mouse Y"), -89, 89): Mathf.Clamp(angleY += Input.GetAxis("Mouse Y"), -89, 89);
        radius = Mathf.Clamp(radius -= Input.mouseScrollDelta.y, 1, 10);

        if (angleX > 360)
        {
            angleX -= 360;
        }
        else if (angleX < 0)
        {
            angleX += 360;
        }
        Vector3 orbit = Vector3.forward * radius;
        orbit = Quaternion.Euler(angleY, angleX, 0) * orbit;
        transform.position = playerTransform.position + orbit;

        transform.LookAt(playerTransform.position);
    }
    private void OnEnable()
    {
        //moveAction.Enable();
    }
    private void OnDisable()
    {
        //moveAction.Disable();
    }
    void OnGUI()
    {
        //Press this button to lock the Cursor
        if (GUI.Button(new Rect(0, 0, 100, 50), "Lock Cursor"))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Press this button to confine the Cursor within the screen
        if (GUI.Button(new Rect(125, 0, 100, 50), "Confine Cursor"))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
