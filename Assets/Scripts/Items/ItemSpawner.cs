using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;
    void Start()
    {
        StartCoroutine(SpawnItem(itemInfo, transform.position, 20));   
    }

    private IEnumerator SpawnItem(ItemInfo itemInfo, Vector3 postion, int numOfItems) 
    {
        if (numOfItems == 0)
        {
            StopAllCoroutines();
        }

        yield return new WaitForSeconds(1f);
        ObjectPooler.Instance.SpawnItem(itemInfo, postion, Quaternion.identity);
        StartCoroutine(SpawnItem(itemInfo, postion, numOfItems - 1));
    }
}
