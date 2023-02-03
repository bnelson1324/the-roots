using Godot;

namespace roottowerdefense.enemy;

public partial class Enemy : Node2D
{

	[Export] public int Health = 100;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}