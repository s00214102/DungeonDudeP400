using UnityEngine;

[RequireComponent(typeof(HeroKnowledge))]
public class HeroStatus : MonoBehaviour
{
    [SerializeField] private int fear;
    private int maxFear = 100;
    [SerializeField] private int fearDecay = 1;
    public int Fear { get => fear; }

    private HeroKnowledge knowledge;

    private void Start()
    {
        InvokeRepeating("FearDecay", 1, 0.5f);
    }

    private void Awake()
    {
        knowledge = GetComponent<HeroKnowledge>();
    }
    public void AddFear(int value)
    {
        fear += value;
        if (fear > maxFear)
            fear = maxFear;
    }
    public void ReduceFear(int value)
    {
        fear -= value;
        if (fear < 0)
            fear = 0;
    }
    private void FearDecay()
    {
        fear -= fearDecay;
        if (fear < 0)
            fear = 0;
    }
}