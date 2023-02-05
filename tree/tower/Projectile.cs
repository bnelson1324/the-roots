using Godot;
using roottowerdefense.enemy;

namespace roottowerdefense.tree.tower;

public partial class Projectile : Area2D
{
    private int _damage;
    private float _aoeRadius;
    private float _velocity;

    private bool _hitEnemy;

    public override void _Ready()
    {
        AreaEntered += (other) =>
        {
            var otherParent = other.GetParent();
            if (otherParent is Enemy enemy && !_hitEnemy)
            {
                _hitEnemy = true;
                enemy.Health -= _damage;
                if (_aoeRadius > 0)
                {
                    foreach (var enemyNode in Game.Instance.EnemyPath.GetChildren())
                    {
                        if (enemy.GetInstanceId() == enemyNode.GetInstanceId())
                            continue;

                        var aoeEnemy = enemyNode as Enemy;
                        if (GlobalPosition.DistanceTo(aoeEnemy!.GlobalPosition) < _aoeRadius)
                        {
                            aoeEnemy.Health -= _damage;
                        }
                    }
                }

                QueueFree();
            }
        };
    }

    public override void _Process(double delta)
    {
        GlobalPosition += GlobalTransform.X * _velocity * (float)delta;
    }

    public void Launch(int damage, float velocity, float aoeRadius)
    {
        _damage = damage;
        _velocity = velocity;
        _aoeRadius = aoeRadius;
    }
}
