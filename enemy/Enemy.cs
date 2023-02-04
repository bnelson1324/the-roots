using Godot;

namespace roottowerdefense.enemy;

public partial class Enemy : PathFollow2D
{
    [Export] private int _health = 100;

    private ProgressBar _healthBar;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthBar.Value = _health;
            if (Health <= 0)
            {
                QueueFree();
            }
        }
    }

    [Export] private int _speed = 100;

    public override void _Ready()
    {
        _healthBar = GetNode<ProgressBar>("HealthBar");
        _healthBar.MaxValue = Health;
        _healthBar.Value = Health;
    }

    public override void _Process(double delta)
    {
        Progress += _speed * (float)delta;

        // enemy reached the end of the path
        if (GlobalPosition.Y < 0)
        {
            Game.Instance.Lives--;
            GD.Print("end reached");
            QueueFree();
        }
    }
}
