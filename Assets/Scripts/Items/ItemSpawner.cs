using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<ItemInfo> itemInfo = new List<ItemInfo>();
    [SerializeField] private int itemsCount;
    [Header("Item coordinates spawn range")]
    [SerializeField] private CoordRange xRange = new CoordRange();
    [SerializeField] private CoordRange yRange = new CoordRange();
    [SerializeField] private CoordRange zRange = new CoordRange();

    void Start()
    {
        StartCoroutine(SpawnItem(itemInfo, GetNewRandomPosition(xRange, yRange, zRange), itemsCount));   
    }

    private IEnumerator SpawnItem(List<ItemInfo> itemInfo, Vector3 postion, int numOfItems) 
    {
        if (numOfItems == 0)
        {
            StopAllCoroutines();
        }

        yield return new WaitForSeconds(1f);
        ObjectPooler.Instance.SpawnItem(itemInfo[numOfItems%2], postion, Quaternion.identity);
        StartCoroutine(SpawnItem(itemInfo, GetNewRandomPosition(xRange, yRange, zRange), numOfItems - 1));
    }

    private Vector3 GetNewRandomPosition(CoordRange x, CoordRange y, CoordRange z) 
    {
        Vector3 newPosDelta = new Vector3(Random.Range(x.minCoord, x.maxCoord), Random.Range(y.minCoord, y.maxCoord), Random.Range(z.minCoord, z.maxCoord));
        return transform.position + newPosDelta;    
    }
}

[System.Serializable]
public class CoordRange
{
    public float minCoord;
    public float maxCoord;
}
