using Godot;
using System;
using roottowerdefense;

public partial class MatterIndicator : Label
{
    private Game _game;

    public int CurrentItemCost;

    public override void _Ready()
    {
        _game = GetParent<Game>();
        _game.UiUpdate += UpdateUi;
    }

    public void UpdateUi()
    {
        String newText = $"Matter: {_game.Matter}";
        if (CurrentItemCost > 0)
        {
            newText += $" (-{CurrentItemCost})";
        }
        Text = newText;
    }
}
