using System.Collections.Generic;
using Godot;

namespace roottowerdefense.enemy;

public partial class WaveManager : Node
{
    [Export] private int _timeBetweenEnemies = 4;
    [Export] private int _timeBetweenWaves = 8;
    [Export] private PackedScene _trashEnemy;

    private Timer _enemyTimer;
    private Path2D _enemyPath;

    private int _waveIndex;
    private int _wavePackIndex;
    private int _wavePackEnemiesSpawned;
    private Wave[] _wavesList;

    public override void _Ready()
    {
        // set up timer
        _enemyTimer = GetNode<Timer>("EnemyTimer");
        _enemyPath = GetNode<Path2D>("../EnemyPath");
        _enemyTimer.Timeout += NextEnemy;
        GetParent<Game>().GameLoss += () => { _enemyTimer.Stop(); };
        _enemyTimer.Start();

        // define wavelist
        _wavesList = new[]
        {
            new Wave(new WavePack(_trashEnemy, 2)),
            new Wave(new WavePack(_trashEnemy, 4)),
            new Wave(new WavePack(_trashEnemy, 8)),
            new Wave(new WavePack(_trashEnemy, 10)),
            new Wave(new WavePack(_trashEnemy, 12)),
        };
    }

    private async void NextEnemy()
    {
        // check if player beat all the waves
        if (_waveIndex >= _wavesList.Length)
        {
            // wait for all enemies to disappear
            while (_enemyPath.GetChildCount() > 0)
            {
                await ToSignal(GetTree(), "process_frame");
            }

            if (Game.Instance.Lives > 0)
            {
                GetNode<Label>("../WinText").Visible = true;
                GetNode<Control>("../PauseScreen").Visible = true;
            }

            return;
        }

        // spawn next enemy
        Wave currentWave = _wavesList[_waveIndex];
        WavePack currentPack = currentWave.WavePacks[_wavePackIndex];
        Enemy nextEnemy = currentPack.EnemyType.Instantiate() as Enemy;
        _enemyPath.AddChild(nextEnemy);

        // advance progress
        _wavePackEnemiesSpawned++;
        if (_wavePackEnemiesSpawned >= currentPack.Quantity)
        {
            // next wavepack
            _wavePackEnemiesSpawned = 0;
            _wavePackIndex++;
        }

        if (_wavePackIndex >= currentWave.WavePacks.Length)
        {
            // next wave
            _wavePackIndex = 0;
            _waveIndex++;
            _enemyTimer.Start(_timeBetweenWaves);
        }
        else
        {
            // spawn enemy
            _enemyTimer.Start(_timeBetweenEnemies);
        }
    }
}

record struct Wave(params WavePack[] WavePacks);

record struct WavePack(PackedScene EnemyType, int Quantity);
