using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SerializableDictionary;

[System.Serializable]
public class TreasureHunterInventory : MonoBehaviour
{

    [System.Serializable]
    public class KeyValueDictionary : SerializableDictionary<collectible, int> {}

    public KeyValueDictionary m_items;  

}