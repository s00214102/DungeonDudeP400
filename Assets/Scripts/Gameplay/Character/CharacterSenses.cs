using UnityEngine;
using UnityEngine.Events;

public class CharacterSenses : MonoBehaviour {
    
        private Vector3 rayOrigin;
        [SerializeField] Transform eyes;
        [SerializeField] int rayCount = 5;
        [SerializeField] float horizontalAngle = 90f;
        [SerializeField] float verticalAngle = 30f;
        [SerializeField] float rayLength = 5f;
        [SerializeField] LayerMask mask;
        bool hitSomething = false;
        public UnityEvent<GameObject> OnSight;
    
    /// <summary>
    /// To make a sound a character just performs an overlap sphere check around itself to find characters that can hear
    /// if it finds any it calls this method passing itself as a reference
    /// </summary>
    /// <param name="gameObject"></param>
    private void Awake() {

    }
    public void Hear(GameObject gameObject){

    }

    private void Update() {
        hitSomething = CastVisionRays();
    }
    // Function to cast rays in a fan shape from an origin point
    public bool CastVisionRays()
    {
        rayOrigin = eyes.position;
        hitSomething = false;

        // Calculate the angle between rays
        float angleBetweenRays = horizontalAngle / (rayCount - 1);

        // Cast rays in a fan shape
        for (int i = 0; i < rayCount; i++)
        {
            // Calculate the direction of the current ray based on the angle
            float angle = -horizontalAngle / 2 + i * angleBetweenRays;
            Vector3 forwardDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Cast the horizontal ray
            if (CastRay(rayOrigin, forwardDirection))
            {
                hitSomething = true;
            }

            // Calculate the vertical direction of the current ray based on the angle
            Vector3 upDirection = Quaternion.Euler(-verticalAngle, 0, 0) * forwardDirection;
            Vector3 downDirection = Quaternion.Euler(verticalAngle, 0, 0) * forwardDirection;

            // Cast the vertical rays
            if (CastRay(rayOrigin, upDirection))
            {
                hitSomething = true;
            }
            if (CastRay(rayOrigin, downDirection))
            {
                hitSomething = true;
            }

            // Add another layer of vertical rays
            upDirection = Quaternion.Euler(-verticalAngle*2, 0, 0) * forwardDirection;
            downDirection = Quaternion.Euler(verticalAngle*2, 0, 0) * forwardDirection;

            // Cast the vertical rays
            if (CastRay(rayOrigin, upDirection))
            {
                hitSomething = true;
            }
            if (CastRay(rayOrigin, downDirection))
            {
                hitSomething = true;
            }
        }

        return hitSomething;
    }
    // Function to cast a single ray and return true if it hits something
    private bool CastRay(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayLength, mask))
        {
            // Ray hit something
            Debug.DrawLine(origin, hit.point, Color.red, 0.1f); // Visualize the ray
            OnSight?.Invoke(hit.collider.gameObject);

            return true;
        }
        else
        {
            // Ray didn't hit anything
            Debug.DrawRay(origin, direction * rayLength, Color.green, 0.1f); // Visualize the ray
            return false;
        }
    }
}