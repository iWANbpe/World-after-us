using UnityEngine;

public class Layouts : MonoBehaviour
{
	[SerializeField] private Layout[] layouts;
	public static Layouts Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance == this)
		{
			Destroy(gameObject);
		}
	}
	public void OpenLayout(LayoutType type)
	{
		for (int i = 0; i < layouts.Length; i++)
		{
			if (layouts[i].layoutType == type)
			{
				CloseAllLayouts();
				layouts[i].OpenLayout();
				break;
			}
		}
	}

	public void CloseAllLayouts()
	{
		for (int i = 0; i < layouts.Length; i++)
		{
			layouts[i].CloseLayout();
		}
	}
}

[System.Serializable]
public class Layout
{
	[SerializeField] private GameObject layout;
	[SerializeField] private LayoutType type;
	public LayoutType layoutType { get { return type; } }

	public void OpenLayout()
	{
		layout.SetActive(true);
	}

	public void CloseLayout()
	{
		layout.SetActive(false);
	}
}

public enum LayoutType
{
	Inventory,
	PlayerPanel
}
