using Godot;
using roottowerdefense.enemy;

namespace roottowerdefense;

public partial class Game : Node2D
{
    public static Game Instance { get; private set; }

    public Path2D EnemyPath;

    [Signal]
    public delegate void UiUpdateEventHandler();

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

    public override void _Ready()
    {
        // matter
        _matter = _startingMatter;

        // enemies
        var enemyTimer = GetNode<Timer>("EnemyTimer");
        EnemyPath = GetNode<Path2D>("EnemyPath");
        enemyTimer.Timeout += () =>
        {
            Enemy trashEnemy = _trashEnemy.Instantiate() as Enemy;
            EnemyPath.AddChild(trashEnemy);
        };

        // misc
        Instance = this;
        EmitSignal(SignalName.UiUpdate);
    }
}
