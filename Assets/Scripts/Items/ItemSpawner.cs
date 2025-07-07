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
		StartCoroutine(SpawnItemCoroutine(items, GetNewRandomPosition(itemsSpawnRange), itemsCount));
	}

	private void SpawnItem(List<SpawnItem> items, Vector3 postion)
	{
		ItemInfo spawnedItemInfo = null;

		while (!spawnedItemInfo)
		{
			SpawnItem possibleSpawnItem = items[Random.Range(0, items.Count)];
			if (RandomChance(possibleSpawnItem.spawnProbability))
				spawnedItemInfo = possibleSpawnItem.itemInfo;
		}

		ObjectPooler.Instance.SpawnItem(spawnedItemInfo, GetNewRandomPosition(itemsSpawnRange), Quaternion.identity);
	}

	private void SpawnItem(List<SpawnItem> items, Vector3 postion, int numOfItems)
	{
		for (int i = 0; i < numOfItems; i++)
		{
			SpawnItem(items, postion);
		}
	}

	private IEnumerator SpawnItemCoroutine(List<SpawnItem> items, Vector3 postion, int numOfItems)
	{
		if (numOfItems == 0)
		{
			StopAllCoroutines();
		}

		yield return new WaitForSeconds(1f);

		SpawnItem(items, postion);
		numOfItems--;
		StartCoroutine(SpawnItemCoroutine(items, postion, numOfItems));
	}

	private Vector3 GetNewRandomPosition(Vector3 position)
	{
		Vector3 newPosDelta = new Vector3(Random.Range(-position.x / 2, position.x / 2), Random.Range(-position.y / 2, position.y / 2), Random.Range(-position.z / 2, position.z / 2));
		return transform.position + newPosDelta;
	}

	private bool RandomChance(float chance)
	{
		if (Random.Range(0, 100) <= chance) return true;
		return false;
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
	[SerializeField] private float _spawnProbability;
	#region Getters
	public ItemInfo itemInfo { get { return _itemInfo; } }
	public float spawnProbability { get { return _spawnProbability; } }
	#endregion
}