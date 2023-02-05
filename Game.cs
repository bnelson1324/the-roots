using System;
using Godot;
using roottowerdefense.enemy;

namespace roottowerdefense;

public partial class Game : Node2D
{
    public static Game Instance { get; private set; }

    [Signal]
    public delegate void UiUpdateEventHandler();

    [Signal]
    public delegate void GameLossEventHandler();

    [Export] private int _startingLives = 3;
    [Export] private int _startingMatter; // player's currency

    public Path2D EnemyPath;
    private Node _sfxManager;

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
            if (_lives == 0)
            {
                EmitSignal(SignalName.GameLoss);
                GetNode<Label>("LoseText").Visible = true;
                _pauseScreen.Visible = true;
            }

            EmitSignal(SignalName.UiUpdate);
        }
    }

    private Control _pauseScreen;

    public override void _Ready()
    {
        Lives = _startingLives;
        Matter = _startingMatter;

        // enemies
        EnemyPath = GetNode<Path2D>("EnemyPath");

        // pause screen
        _pauseScreen = GetNode<Control>("PauseScreen");
        _pauseScreen.GetNode<Button>("BtnRestart").Pressed += () => { GetTree().ReloadCurrentScene(); };
        _pauseScreen.GetNode<Button>("BtnExit").Pressed += () => { GetTree().Quit(); };

        // misc
        _sfxManager = GetNode("SfxManager");
        Instance = this;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Pause"))
        {
            _pauseScreen.Visible = !_pauseScreen.Visible;
        }
    }

    public AudioStreamPlayer2D GetAudioPlayer(string name)
    {
        return _sfxManager.GetNode<AudioStreamPlayer2D>(name);
    }
}
