using System;
using System.Collections.Generic;
using Godot;

namespace roottowerdefense.enemy;

public partial class WaveManager : Node
{
    [Export] private PackedScene _trashEnemy;
    [Export] private PackedScene _sodaEnemy;

    private Timer _enemyTimer;
    private Path2D _enemyPath;

    // progress
    private int _waveIndex;
    private int _wavePackIndex;
    private int _wavePackEnemiesSpawned;
    private WaveEvent[] _wavesList;

    // difficulty
    private float _timeBetweenEnemies = 4;
    private float _timeBetweenWaves = 5;
    
    // wave indicator
    private int _currentWaveIndicatorNum = 1;
    private int _totalWaves;

    public override void _Ready()
    {
        // set up timer
        _enemyTimer = GetNode<Timer>("EnemyTimer");
        _enemyPath = GetNode<Path2D>("../EnemyPath");
        _enemyTimer.Timeout += NextWaveEvent;
        GetParent<Game>().GameLoss += () => { _enemyTimer.Stop(); };
        _enemyTimer.Start();

        // DEFINE WAVESLIST
        _wavesList = new WaveEvent[]
        {
            // 1-5
            new Wave(new WavePack(_trashEnemy, 2)),
            new Wave(new WavePack(_trashEnemy, 4), new WavePack(_sodaEnemy, 1)),
            new WaveLambda(() => { _timeBetweenEnemies = 3; }),
            new Wave(new WavePack(_trashEnemy, 8)),
            new WaveLambda(() => { _timeBetweenEnemies = 2.5f; }),
            new Wave(new WavePack(_trashEnemy, 8), new WavePack(_sodaEnemy, 3)),
            new WaveLambda(() => { _timeBetweenEnemies = 1; }),
            new Wave(new WavePack(_trashEnemy, 8), new WavePack(_sodaEnemy, 4)),

            // 6-10
            new Wave(new WavePack(_sodaEnemy, 6)),
            new WaveLambda(() => { _timeBetweenEnemies = 0.6f; }),
            new Wave(new WavePack(_sodaEnemy, 8)),
            new Wave(
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 1),
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 1),
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 1)
            ),
            new WaveLambda(() => { _timeBetweenEnemies = 0.2f; }),
            new Wave(
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 1),
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 1)
            ),
            new Wave(
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 2),
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 2),
                new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 2)
            ),
            
            // 11-15
            new Wave( new WavePack(_sodaEnemy, 8)),
            new WaveLambda(() => { _timeBetweenEnemies = 0.1f; }),
            new Wave(new WavePack(_trashEnemy, 2), new WavePack(_sodaEnemy, 6)),
            new Wave(new WavePack(_sodaEnemy, 8)),
            new Wave(new WavePack(_sodaEnemy, 16)),
            new WaveLambda(() => { _timeBetweenEnemies = 0.05f; }),
            new Wave(new WavePack(_sodaEnemy, 14)),
            
            // 16-20
            new Wave(new WavePack(_sodaEnemy, 33)),
            
        };

        // calc totalWaves
        foreach (var waveEvent in _wavesList)
        {
            if (waveEvent is Wave)
                _totalWaves++;
        }

        UpdateWaveIndicator();
    }

    private async void NextWaveEvent()
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

        // update wave indicator
        UpdateWaveIndicator();

        // spawn next enemy
        WaveEvent currentEvent = _wavesList[_waveIndex];
        if (currentEvent is Wave currentWave)
        {
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
                // next waveevent
                _wavePackIndex = 0;
                _waveIndex++;
                _currentWaveIndicatorNum++;
                _enemyTimer.Start(_timeBetweenWaves);
            }
            else
            {
                // next enemy
                _enemyTimer.Start(_timeBetweenEnemies);
            }
        }
        else if (currentEvent is WaveLambda currentLambda)
        {
            _waveIndex++;
            currentLambda.Lambda.Invoke();
            _enemyTimer.Start(0);
        }
    }

    private void UpdateWaveIndicator()
    {
        GetNode<Label>("../WaveIndicator").Text = $"Wave {_currentWaveIndicatorNum}/{_totalWaves}";
    }
}

abstract record WaveEvent();

// waves
record Wave(params WavePack[] WavePacks) : WaveEvent;

record WavePack(PackedScene EnemyType, int Quantity);

// wave lambdas
record WaveLambda(WaveLambdaDelegate Lambda) : WaveEvent;

delegate void WaveLambdaDelegate();
