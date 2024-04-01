using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{
    public GameplayControls controls;
    Vector3 moveVector;

    Transform cam;
    Vector3 camF;
    Vector3 camR;

    Rigidbody Body;

    float turnSpeed = 5; //speed of change in rotation to new look rotation
    [SerializeField] float turnSpeedLow = 15;
    [SerializeField] float turnSpeedHigh = 30;
    Vector3 intent; //camera direction and input
    Vector3 velocity; //current direction and speed
    Vector3 velocityXZ;
    [SerializeField] float speed = 20; //movement speed
    [SerializeField] float accel = 15; //speed of change in velocity to new velocity

    private void Awake()
    {
        controls = new GameplayControls();

        moveVector = controls.Gameplay.MovePlayer.ReadValue<Vector3>();

        controls.Gameplay.Interact.performed += OnInteract;

        cam = Camera.main.transform;

        Body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CalculateCamera();

        moveVector = controls.Gameplay.MovePlayer.ReadValue<Vector3>();

        //Vector3 camVector = CameraTransform.position - transform.position;

        //Vector3 newMove = new Vector3(camVector.x * moveVector.x, camVector.y * moveVector.y, 0);

        //transform.position += newMove * moveSpeed * Time.deltaTime;

        if (moveVector.magnitude > 0)
        {
            // rotate to face movement direction
            intent = camF * -moveVector.z + camR * -moveVector.x;

            float tS = velocity.magnitude / 5;
            turnSpeed = Mathf.Lerp(turnSpeedHigh, turnSpeedLow, tS);

            Quaternion rot = Quaternion.LookRotation(intent); //make a new variable for rotation, which is the direction we are looking, using intent
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }


        //var velocityXZ = velocity;
        //velocityXZ.y = 0; //to make sure the Y is not included in our movement
        //velocityXZ = Vector3.Lerp(velocityXZ, transform.forward * moveVector.magnitude * speed, accel);
        //velocity = new Vector3(velocityXZ.x, 0, velocityXZ.z); // so velocity is actually two vector3s, the Y is now seperate

        //Body.AddForce(velocity * Time.deltaTime);

        velocity = (transform.right * moveVector.magnitude) * speed;

        Body.AddForce(velocity * Time.deltaTime);
        //transform.position += velocity * Time.deltaTime;
    }
    void CalculateCamera()
    {
        camF = cam.forward;
        camR = cam.right;

        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.ReadValueAsObject());
    }
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interact pressed");
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
