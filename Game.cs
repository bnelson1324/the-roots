using Godot;

namespace roottowerdefense;

public partial class Game : Node2D
{
    public static Game Instance
    {
        get;
        private set;
    }
    
    [Signal]
    public delegate void UiUpdateEventHandler();

    [Export] private int _matter; // player's currency

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
        Instance = this;
        EmitSignal(SignalName.UiUpdate);
    }
    
}
