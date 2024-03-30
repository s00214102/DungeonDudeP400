using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Item
{
    public string ItemName;
    public int Count; // (multiple of the same item are stacked)
    public int Value; // (how much it sells for)
    public Image Image; // image to display for the item
}