using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] private List<Item> Loot = new List<Item>();
    public bool randomlyGenerateLoot = false;

    private void Start()
    {
        if (randomlyGenerateLoot)
        {
            //TODO proper random generation of items
            // access a list of all items in the game
            // pick a random number
            // add that many items randomly from the list

            // temporary manually adding items
            Loot.Add(new Item() { ItemName = "Ruby" });
            Loot.Add(new Item() { ItemName = "Diamond" });
        }
    }
    public bool HasLoot()
    {
        if (Loot.Count > 0)
            return true;
        else
            return false;
    }
    public Item LootItem()
    {
        Item loot = Loot[0];
        Loot.RemoveAt(0);
        return loot;
    }
    public void AddItem(string itemName)
    {
        Item item = new Item() { ItemName = itemName };
        Loot.Add(item);
    }
}