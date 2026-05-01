using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public List<ItemToSpawn> itemsToSpawn;
    public List<ItemSpawned> currentGameItems;

    [Range(0f, 1f)]
    public float nullProbability = 0.4f;

    [Serializable]
    public class ItemToSpawn
    {
        public GameObject item;
        public float probability;
    }

    [Serializable]
    public class ItemSpawned
    {
        public GameObject item;
        public GameObject parent;
        public bool active = true;
    }

    public GameObject GetRandomObject(GameObject itemParent)
    {
        ItemSpawned itemSpawned = new ItemSpawned();

        // 🎲 Primer comprovem si surt null
        if (UnityEngine.Random.value < nullProbability)
        {
            return null;
        }

        // 🔢 Sumar probabilitats
        float totalWeight = 0f;
        foreach (var item in itemsToSpawn)
        {
            totalWeight += item.probability;
        }

        // 🎯 Número aleatori dins el rang
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        // 🔍 Buscar quin item toca
        float current = 0f;
        foreach (var item in itemsToSpawn)
        {
            current += item.probability;

            if (randomPoint <= current)
            {
                itemSpawned.item = item.item;
                itemSpawned.parent = itemParent;
                currentGameItems.Add(itemSpawned);
                return item.item;
            }
        }

        return null; // fallback (no hauria de passar)
    }

    public ItemSpawned getItemToSearch(int category)
    {
        if(category == 1) // munició
        {
            for (int i = 0; i < currentGameItems.Count; i++)
            {
                if (currentGameItems[i].active == true && currentGameItems[i].item.tag == "Ammo")
                {
                    ItemSpawned found = currentGameItems[i];
                    currentGameItems.RemoveAt(i);
                    return found;
                }
            }
        }
        else if(category == 2) // utils
        {
            for (int i = 0; i < currentGameItems.Count; i++)
            {
                if (currentGameItems[i].active == true && currentGameItems[i].item.tag == "Coin")
                {
                    ItemSpawned found = currentGameItems[i];
                    currentGameItems.RemoveAt(i);
                    return found;
                }
            }
        }
        else if(category == 3) // boost
        {
            for (int i = 0; i < currentGameItems.Count; i++)
            {
                if(currentGameItems[i].active == true){
                    if (currentGameItems[i].item.tag == "Os Gos" || currentGameItems[i].item.tag == "Magnet" || currentGameItems[i].item.tag == "Boost Speed" || currentGameItems[i].item.tag == "Boost Ammo" )
                    {
                        ItemSpawned found = currentGameItems[i];
                        currentGameItems.RemoveAt(i);
                        return found;
                    }
                }
            }
        }

        return null;
    }

    public bool CheckItem(GameObject itemParent)
    {
        for(int i=0; i<currentGameItems.Count; i++)
        {
            if(currentGameItems[i].parent == itemParent) return true;
        }

        return false;
    }

    public void ObjectActive(GameObject itemParent, bool enabled)
    {
        for(int i=0; i<currentGameItems.Count; i++)
        {
            if(currentGameItems[i].parent == itemParent) currentGameItems[i].active = enabled;
        }
    }
}