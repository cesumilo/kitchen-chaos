using Godot;
using System;

public partial class KitchenObject : Node3D
{
    [Export] KitchenObjectResource kitchenObject;
    IKitchenObjectHolder holder;

    public void SetHolder(IKitchenObjectHolder newHolder)
    {
        holder?.ClearKitchenObject();
        holder = newHolder;
    }

    public KitchenObjectResource GetKitchenObject()
    {
        return kitchenObject;
    }

    public IKitchenObjectHolder GetHolder()
    {
        return holder;
    }
}
