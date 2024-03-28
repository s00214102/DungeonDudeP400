using UnityEngine;

[RequireComponent(typeof(HeroKnowledge))]
public class HeroStatus : MonoBehaviour {
    private int fear;
    private int maxFear = 100;
	public int Fear {get=>fear;}
    [SerializeField] private float fearCheckFrequency = 0.5f;
    private HeroKnowledge knowledge;

    private void Awake() {
        knowledge = GetComponent<HeroKnowledge>();
    }   
    public void AddFear(int value){
        fear+=value;
        if(fear>maxFear)
            fear=maxFear;
    }
    public void ReduceFear(int value){
        fear-=value;
        if(fear<0)
            fear = 0;
    }
}