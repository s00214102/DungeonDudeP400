using System.Collections.Generic;
using UnityEngine;

// Inventory for a character to hold items
public class Inventory : MonoBehaviour {
    [SerializeField] private List<Item> Items = new List<Item>();
    
    public void AddItem(Item item){
        Items.Add(item);
    }
}