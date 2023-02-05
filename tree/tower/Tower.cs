using System.Collections.Generic;
using Godot;
using roottowerdefense.enemy;
using roottowerdefense.tree.upgrade;

namespace roottowerdefense.tree.tower;

public partial class Tower : Node2D
{
    [Export] private PackedScene _projectile;
    [Export] public int ProjectileDamage;
    [Export] public float AttackDelay = 2;
    [Export] public int Range;
    [Export] public int ProjectileVelocity;
    [Export] public float ProjectileAoeRadius = 0;

    private Timer _attackTimer;
    private Node2D _projectileOrigin;

    private bool _canAttack;

    private List<Enemy> _enemiesInRange = new();

    public override void _Ready()
    {
        _projectileOrigin = GetNode<Node2D>("ProjectileOrigin");

        // set up timer
        _attackTimer = GetNode<Timer>("AttackTimer");
        _attackTimer.WaitTime = AttackDelay;
        _attackTimer.Timeout += () => { _canAttack = true; };
        _canAttack = true;
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
        bool enemyFound = false;
        Enemy enemy = null;
        while (!enemyFound)
        {
            foreach (var enemyNode in Game.Instance.EnemyPath.GetChildren())
            {
                enemy = enemyNode as Enemy;
                if (_projectileOrigin.GlobalPosition.DistanceTo(enemy!.GlobalPosition) < Range)
                {
                    enemyFound = true;
                    break;
                }
            }

            await ToSignal(GetTree(), "process_frame");
        }

        // launch projectile
        if (IsInstanceValid(enemy))
        {
            Vector2 enemyDir = enemy.GlobalPosition - _projectileOrigin.GlobalPosition;
            float enemyAngle = Mathf.Atan2(enemyDir.Y, enemyDir.X);
            _projectileOrigin.GlobalRotation = enemyAngle;

            Projectile projectile = (Projectile)_projectile.Instantiate();
            _projectileOrigin.AddChild(projectile);
            projectile.Launch(ProjectileDamage, ProjectileVelocity, ProjectileAoeRadius);

            _attackTimer.Start();
            
            // sfx
            Game.Instance.GetAudioPlayer("TowerShoot").Play();
        }
        else
        {
            _attackTimer.Start(0);
        }
    }

    public void ApplyUpgrades(params UpgradeEffect[] upgrades)
    {
        // add all current upgrades to node
        foreach (var upgradeEffect in upgrades)
        {
            upgradeEffect.Lambda(this);
        }

        _attackTimer.WaitTime = AttackDelay;
    }
}
