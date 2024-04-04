using UnityEngine;

/// <summary>
/// Moves a projectile prefab from point A to B then destroys it
/// </summary>
public class Projectile : MonoBehaviour {
    public Vector3 targetPosition;
    private void Update() {
        if(targetPosition==null)
            return;

        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 15f * Time.deltaTime);
        // move to target
        if (Vector3.Distance(this.transform.position, targetPosition)< 0.5f)
            {
                //TODO calling destroy
                Destroy(this.gameObject);
            }
    }
}