using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<ItemInfo> itemInfo = new List<ItemInfo>();
    void Start()
    {
        StartCoroutine(SpawnItem(itemInfo, transform.position, 20));   
    }

    private IEnumerator SpawnItem(List<ItemInfo> itemInfo, Vector3 postion, int numOfItems) 
    {
        if (numOfItems == 0)
        {
            StopAllCoroutines();
        }

        yield return new WaitForSeconds(1f);
        ObjectPooler.Instance.SpawnItem(itemInfo[numOfItems%2], postion, Quaternion.identity);
        StartCoroutine(SpawnItem(itemInfo, postion, numOfItems - 1));
    }
}
