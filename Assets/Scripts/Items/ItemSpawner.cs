using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	[SerializeField] private Vector3 itemsSpawnRange;
	[SerializeField] private List<SpawnItem> items = new List<SpawnItem>();
	[SerializeField] private int itemsCount;

	void Start()
	{
		SpawnItem(items, itemsCount);
	}

	private void SpawnItem(List<SpawnItem> items)
	{
		ItemInfo spawnedItemInfo = PickRandomItem(items).itemInfo;
		ObjectPooler.Instance.SpawnItem(spawnedItemInfo, GetNewRandomPosition(itemsSpawnRange), Quaternion.identity);
	}

	private void SpawnItem(List<SpawnItem> items, int numOfItems)
	{
		for (int i = 0; i < numOfItems; i++)
		{
			SpawnItem(items);
		}
	}

	private IEnumerator SpawnItemCoroutine(List<SpawnItem> items, int numOfItems)
	{
		if (numOfItems == 0)
		{
			StopAllCoroutines();
		}

		yield return new WaitForSeconds(1f);
		SpawnItem(items);
		numOfItems--;
		StartCoroutine(SpawnItemCoroutine(items, numOfItems));
	}

	private Vector3 GetNewRandomPosition(Vector3 position)
	{
		Vector3 newPosDelta = new Vector3(Random.Range(-position.x / 2, position.x / 2), Random.Range(-position.y / 2, position.y / 2), Random.Range(-position.z / 2, position.z / 2));
		return transform.position + newPosDelta;
	}

	private SpawnItem PickRandomItem(List<SpawnItem> spawnItems)
	{
		int chance = Random.Range(0, 100);
		int itemProbability = 0;

		foreach (SpawnItem spawnItem in spawnItems)
		{
			itemProbability += spawnItem.spawnProbability;
			if (chance <= itemProbability)
			{
				return spawnItem;
			}
		}

		return (spawnItems[spawnItems.Count]);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position, itemsSpawnRange);
	}
}

[System.Serializable]
public class SpawnItem
{
	[SerializeField] private ItemInfo _itemInfo;
	[SerializeField] [Range(1, 100)] private int _spawnProbability;
	#region Getters
	public ItemInfo itemInfo { get { return _itemInfo; } }
	public int spawnProbability { get { return _spawnProbability; } }
	#endregion
}