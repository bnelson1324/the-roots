using Godot;
using roottowerdefense.tree.tower;
using roottowerdefense.tree.ui;

namespace roottowerdefense.tree;

public partial class TreeLeaf : Node2D
{
    [Export] public PackedScene AcornShooter;

    private RadiusIndicator _radiusIndicator;

    private bool _towerPurchased;

    private Control _buttons;
    private TextureButton _btnLeaf;

    public override void _Ready()
    {
        // set up button signals
        _buttons = GetNode<Control>("Buttons");

        _btnLeaf = GetNode<TextureButton>("BtnLeaf");
        _btnLeaf.Pressed += () =>
        {
            if (!_towerPurchased)
            {
                _buttons.Visible = !_buttons.Visible;
            }
        };

        var btnAcornShooter = _buttons.GetNode<PurchaseButton>("BtnAcornShooter");
        btnAcornShooter.OnPurchased += (int cost) => { BecomeTower(AcornShooter, cost); };

        // sets up radius indicator
        _radiusIndicator = GetNode<RadiusIndicator>("RadiusIndicator");
    }

    private void BecomeTower(PackedScene towerScene, int cost)
    {
        Game.Instance.Matter -= cost;

        _towerPurchased = true;
        var tower = towerScene.Instantiate() as Tower;
        AddChild(tower);

        // remove all the leaf's buy buttons
        _buttons.QueueFree();

        // set up range indicator for tower
        _radiusIndicator.Radius = tower!.Range;
        _btnLeaf.Pressed += () => { _radiusIndicator.ShowRadius = !_radiusIndicator.ShowRadius; };
    }
}