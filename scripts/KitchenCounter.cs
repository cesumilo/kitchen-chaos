using Godot;
using System;

public partial class KitchenCounter : InteractableStaticBody3D, Interactable, IKitchenObjectHolder
{
	[Export] KitchenObjectResource kitchenObject;
	Marker3D spawnPosition;
	KitchenObject objectHolder;

	public override void _Ready()
	{
		base._Ready();
		spawnPosition = GetNode<Marker3D>("SpawnPosition");
	}

	public void Interact(Player player)
	{
		if (objectHolder == null && !player.IsHoldingKitchenObject())
		{
			KitchenObject obj = kitchenObject.Instantiate<KitchenObject>();
			SetKitchenObject(obj);
		}
		else if (objectHolder == null) {
			SetKitchenObject(player.GetKitchenObject());
			player.ClearKitchenObject();
		}
		else if (!player.IsHoldingKitchenObject())
		{
			player.SetKitchenObject(objectHolder);
			ClearKitchenObject();
		}
	}

	public Marker3D GetKitchenObjectSpawn()
	{
		return spawnPosition;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		objectHolder = kitchenObject;
		objectHolder.SetHolder(this);
		spawnPosition.AddChild(objectHolder);
	}

	public KitchenObject GetKitchenObject()
	{
		return objectHolder;
	}

	public void ClearKitchenObject()
	{
		if (objectHolder != null)
		{
			spawnPosition.RemoveChild(objectHolder);
		}
		objectHolder = null;
	}

	public bool IsHoldingKitchenObject()
	{
		return objectHolder != null;
	}
}
