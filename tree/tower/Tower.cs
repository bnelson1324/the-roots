using System.Collections.Generic;
using Godot;
using roottowerdefense.enemy;

namespace roottowerdefense.tree.tower;

public partial class Tower : Node2D
{
    [Export] private PackedScene _projectile;
    [Export] private int _projectileDamage;
    [Export] private float _attackDelay = 2;
    [Export] private int _range = 500;
    [Export] private int _projectileVelocity = 300;
    [Export] private float _projectileAoeRadius = 0;

    private Timer _attackTimer;
    private Node2D _projectileOrigin;

    private bool _canAttack;

    private List<Enemy> _enemiesInRange = new();

    public override void _Ready()
    {
        _projectileOrigin = GetNode<Node2D>("ProjectileOrigin");

        // set delay
        _attackTimer = GetNode<Timer>("AttackTimer");
        _attackTimer.Timeout += () => { _canAttack = true; };
    }

    public override void _Process(double delta)
    {
        if (_canAttack)
        {
            Attack();
        }
    }

    private async void Attack()
    {
        _canAttack = false;

        // await an enemy to come in range
        // await ...

        return; // temp
        Enemy enemy = null;
        Vector2 enemyDir = enemy.GlobalPosition - GlobalPosition;
        float enemyAngle = Mathf.Atan2(enemyDir.Y, enemyDir.X);
        _projectileOrigin.GlobalRotation = enemyAngle;

        // launch projectile
        Projectile projectile = (Projectile)_projectile.Instantiate();
        projectile.GlobalRotation = enemyAngle;
        AddChild(projectile);
        projectile.Launch(_projectileDamage, _projectileVelocity, _projectileAoeRadius);

        _attackTimer.Start();
    }
}
