using System.Collections.Generic;
using UnityEngine;

// Inventory for a character to hold items
public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();
    public List<Item> Items { get { return items; } }

    public void AddItem(Item item)
    {
        items.Add(item);
    }
    public void UseItem()
    {

    }
}