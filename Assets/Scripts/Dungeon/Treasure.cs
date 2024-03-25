using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    private List<Item> Loot = new List<Item>();
    [SerializeField] private bool randomlyGenerateLoot = true;
    
    private void Start() {
        if(randomlyGenerateLoot){
            // access a list of all items in the game
            // pick a random number
            // add that many items randomly from the list
            
            // temporary manually adding items
            Loot.Add(new Item(){ItemName="Ruby"});
            Loot.Add(new Item(){ItemName="Diamond"});
        }
    }
    public bool HasLoot(){
        if(Loot.Count>0)
            return true;
        else
            return false;
    }
    public Item LootItem(){
        Item loot = Loot[0];
        Loot.RemoveAt(0);
        return loot;
    }
}