using System;
using Godot;
using roottowerdefense.enemy;

namespace roottowerdefense;

public partial class Game : Node2D
{
    public static Game Instance { get; private set; }

    public Path2D EnemyPath;

    [Signal]
    public delegate void UiUpdateEventHandler();

    [Signal]
    public delegate void GameLossEventHandler();

    [Export] private int _startingLives = 3;
    [Export] private int _startingMatter; // player's currency
    [Export] private PackedScene _trashEnemy;


    private int _matter;

    public int Matter
    {
        get => _matter;
        set
        {
            _matter = value;
            EmitSignal(SignalName.UiUpdate);
        }
    }

    private int _lives;

    public int Lives
    {
        get => _lives;
        set
        {
            _lives = value;
            GetNode<Label>("LivesIndicator").Text = $"Lives: {Math.Max(_lives, 0)}";
            if (_lives <= 0)
            {
                _enemyTimer.Stop();
                EmitSignal(SignalName.GameLoss);
            }

            EmitSignal(SignalName.UiUpdate);
        }
    }

    private Timer _enemyTimer;

    public override void _Ready()
    {
        Lives = _startingLives;
        Matter = _startingMatter;

        // enemies
        _enemyTimer = GetNode<Timer>("EnemyTimer");
        EnemyPath = GetNode<Path2D>("EnemyPath");
        _enemyTimer.Timeout += () =>
        {
            Enemy trashEnemy = _trashEnemy.Instantiate() as Enemy;
            EnemyPath.AddChild(trashEnemy);
        };
        _enemyTimer.Start();

        // misc
        Instance = this;
    }
}
