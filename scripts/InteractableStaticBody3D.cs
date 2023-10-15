using Godot;
using System;

public partial class InteractableStaticBody3D : StaticBody3D
{
    protected bool selected;
    [Export] Node3D selection;

    public override void _Ready()
    {
        Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
        player.OnIteractableSelected += OnInteractableSelected;
    }

    void OnInteractableSelected(Player.OnIteractableSelectedParams args)
    {
        selected = args.selected == this;
        if (selected)
        {
            selection?.Show();
        }
        else
        {
            selection?.Hide();
        }
    }
}
