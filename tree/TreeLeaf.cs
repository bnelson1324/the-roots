using Godot;
using roottowerdefense.tree.tower;
using roottowerdefense.tree.ui;

namespace roottowerdefense.tree;

public partial class TreeLeaf : Node2D
{
    [Export] public PackedScene AcornShooter;
    [Export] public PackedScene Cactus;
    [Export] public PackedScene MushroomMortar;

    public bool IsPlaced = false;
    
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
            if (IsPlaced && !_towerPurchased)
            {
                _buttons.Visible = !_buttons.Visible;
            }
        };

        var btnAcornShooter = _buttons.GetNode<PurchaseButton>("BtnAcornShooter");
        btnAcornShooter.OnPurchased += (int cost) => { BecomeTower(AcornShooter, cost); };

        var btnCactus = _buttons.GetNode<PurchaseButton>("BtnCactus");
        btnCactus.OnPurchased += (int cost) => { BecomeTower(Cactus, cost); };

        var btnMushroomMortar = _buttons.GetNode<PurchaseButton>("BtnMushroomMortar");
        btnMushroomMortar.OnPurchased += (int cost) => { BecomeTower(MushroomMortar, cost); };

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
