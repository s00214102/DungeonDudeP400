using UnityEngine;

// uses a sphere collider check to find heroes
// calls their AddFear on HeroStatus
[RequireComponent(typeof(EnemyData))]
[RequireComponent(typeof(EntityProximityDetection))]
public class FearAura : MonoBehaviour
{
    // fear aura radius
    [SerializeField] private float fearRadius = 1;
    // fear damage value
    [SerializeField] private int fearDamage = 10;
    // how often to apply the fear damage
    [SerializeField] private float fearFrequency = 0.5f;


    EntityProximityDetection detection;

    private void Awake()
    {
        detection = GetComponent<EntityProximityDetection>();
    }
    private void Start()
    {
        InvokeRepeating("ApplyFear", 0, fearFrequency);
    }
    private void ApplyFear()
    {
        // has this entity detected any heroes?
        if (detection.Entities.Count > 0)
        {
            foreach (var entity in detection.Entities)
            {
                // check the distance between the entity using the fear radius
                float dist = Vector3.Distance(entity.transform.position, transform.position);
                if (dist < fearRadius)
                {
                    HeroStatus heroStatus;
                    if (entity.gameObject.TryGetComponent<HeroStatus>(out heroStatus))
                        heroStatus.AddFear(fearDamage);
                }
            }
        }
    }
}