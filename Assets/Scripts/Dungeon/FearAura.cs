using UnityEngine;

// uses a sphere collider check to find heroes
// calls their AddFear on HeroStatus
[RequireComponent(typeof(EntityData))]
[RequireComponent(typeof(EntityProximityDetection))]
public class FearAura : MonoBehaviour {
    EntityData data;
    EntityProximityDetection detection;
    private float frequency;
    private void Awake() {
        data=GetComponent<EntityData>();
        detection=GetComponent<EntityProximityDetection>();
    }
    private void Start() {
        frequency = data.FearFrequency;
        InvokeRepeating("ApplyFear", 0, frequency);
    }
    private void ApplyFear(){
        if(detection.Entities.Count<0){
            foreach (var entity in detection.Entities) {
                // check the distance between the entity
                float dist = Vector3.Distance(entity.transform.position, transform.position);
                if (dist < minDistance && !target.Health.isDead)
                {
                    minDistance = dist;
                    closestTarget = target;
                }
            }
        }
    }
}