using Godot;

public interface IKitchenObjectHolder
{
    public Marker3D GetKitchenObjectSpawn();
    public void SetKitchenObject(KitchenObject kitchenObject);
    public KitchenObject GetKitchenObject();
    public void ClearKitchenObject();
    public bool IsHoldingKitchenObject();
}
