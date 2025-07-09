public interface IInteract
{
	bool CanInteract();
	void Interact();
}

public interface IOnShoot
{
	void OnShoot();
}

public interface ILocalizationItem
{
	string GetLocalizedItemName();
	string GetLocalizedItemDescription();
}