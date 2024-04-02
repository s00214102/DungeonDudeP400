using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Treasure : MonoBehaviour
{
    [SerializeField] private List<Item> Loot = new List<Item>();
    public bool randomlyGenerateLoot = false;
    public int maxLootCount = 4;
    public UnityEvent OnLooted;

    private List<string> possibleLoot = new List<string>()
    {"Rust Dagger","Chipped Gemstone","Silver Necklace","Enchanted Ring","Antique Coin"
    ,"Ancient Tome","Golden Idol","Dragon Egg","Lost Artifact"};

    private void Start()
    {
        if (randomlyGenerateLoot)
        {
            ChooseRandomLoot();
        }
    }
    // Function to choose random loot items
    public void ChooseRandomLoot()
    {
        // Choose a random number between 1 and the length of the array
        int numberOfItems = Random.Range(1, maxLootCount);

        // Shuffle the lootItems list to ensure randomness
        List<string> shuffledLoot = ShuffleList(possibleLoot);

        // Select that many random elements from the array
        for (int i = 0; i < numberOfItems; i++)
        {
            Loot.Add(new Item() { ItemName = shuffledLoot[i] });
        }
    }
    // Function to shuffle a list
    private List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledList = new List<T>(list);
        for (int i = 0; i < shuffledList.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledList.Count);
            T temp = shuffledList[i];
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }
        return shuffledList;
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
        OnLooted?.Invoke();
        return loot;
    }
    public void AddItem(string itemName)
    {
        Item item = new Item() { ItemName = itemName };
        Loot.Add(item);
    }
}