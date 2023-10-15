using Godot;
using System;

public partial class KitchenObjectResource : Resource
{
    [Export] public string ScenePath { get; set; }
    [Export] public CompressedTexture2D Sprite { get; set; }
    [Export] public string Name { get; set; }

    public KitchenObjectResource() : this(null, null, "") { }
    public KitchenObjectResource(string scenePath, CompressedTexture2D sprite, string name)
    {
        ScenePath = scenePath;
        Sprite = sprite;
        Name = name;
    }

    PackedScene LoadScene()
    {
        return ResourceLoader.Load<PackedScene>(ScenePath);
    }

    public T Instantiate<T>() where T : Node
    {
        PackedScene packedScene = LoadScene();
        return packedScene.Instantiate<T>();
    }
}
