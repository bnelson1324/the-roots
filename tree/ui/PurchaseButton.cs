using Godot;

namespace roottowerdefense.tree.ui;

public partial class PurchaseButton : TextureButton
{
    [Signal]
    public delegate void OnPurchasedEventHandler(int cost);

    [Export] public int Cost;

    public override void _Ready()
    {
        // purchasing
        Pressed += () =>
        {
            if (Game.Instance.Matter >= Cost)
            {
                EmitSignal(SignalName.OnPurchased, Cost);
            }
        };

        // hover
        MouseEntered += () => { UpdateMatterIndicator(Cost); };
        MouseExited += () => { UpdateMatterIndicator(0); };
    }

    private void UpdateMatterIndicator(int currentItemCost)
    {
        var matterIndicator = Game.Instance?.GetNode<MatterIndicator>("MatterIndicator");
        if (matterIndicator != null)
        {
            matterIndicator.CurrentItemCost = currentItemCost;
            matterIndicator.UpdateUi();
        }
    }
}
