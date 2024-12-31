using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;
    void Start()
    {
        itemInfo.InstaniateItem(transform.position, Quaternion.identity);   
    }
}
