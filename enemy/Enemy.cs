using Godot;

namespace roottowerdefense.enemy;

public partial class Enemy : PathFollow2D
{
    
    [Export] public int Health = 100;
    [Export] private int _speed = 100;

    public override void _Process(double delta)
    {
        if (Health <= 0)
        {
            QueueFree();
        }

        Progress += _speed * (float)delta;

        // enemy reached the end of the path
        if (ProgressRatio >= 1)
        {
            // todo: implement life loss
        }
    }
}
