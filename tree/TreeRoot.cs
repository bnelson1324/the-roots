using System.Collections.Generic;
using System.Net.Mime;
using Godot;
using roottowerdefense.tree.tower;
using roottowerdefense.tree.ui;
using roottowerdefense.tree.upgrade;

namespace roottowerdefense.tree;

public partial class TreeRoot : Node2D
{
    [Export] public int MaxLeafRadius = 150;
    [Export] private int _perBranchPriceIncrease = 80;
    [Export] public PackedScene LeafScene;
    [Export] public PackedScene BranchScene;

    private Node2D _leaves;
    private RadiusIndicator _radiusIndicator;
    private Control _buttons;
    private PurchaseButton _btnNewBranch;

    // upgrades
    private List<UpgradeEffect> _allUpgradeEffects = new();
    private UpgradeButton _btnUpgrade1, _btnUpgrade2;

    public override void _Ready()
    {
        _leaves = GetNode<Node2D>("Leaves");

        // set up button signals
        _buttons = GetNode<Control>("Buttons");

        var btnRoot = GetNode<TextureButton>("BtnRoot");
        btnRoot.Pressed += () => { _buttons.Visible = !_buttons.Visible; };

        _btnNewBranch = _buttons.GetNode<PurchaseButton>("BtnNewBranch");
        _btnNewBranch.OnPurchased += CreateBranch;

        // upgrade buttons
        _btnUpgrade1 = _buttons.GetNode<UpgradeButton>("BtnUpgrade1");
        _btnUpgrade2 = _buttons.GetNode<UpgradeButton>("BtnUpgrade2");
        RandomizeUpgrades();
        _btnUpgrade1.OnPurchased += (cost) => { AddUpgrade(_btnUpgrade1.Effect, cost); };
        _btnUpgrade2.OnPurchased += (cost) => { AddUpgrade(_btnUpgrade2.Effect, cost); };

        // sets up radius indicator
        _radiusIndicator = GetNode<RadiusIndicator>("RadiusIndicator");
        _radiusIndicator.Radius = MaxLeafRadius;
    }

    private async void CreateBranch(int cost)
    {
        _radiusIndicator.ShowRadius = true;

        // spawn a leaf and a branch
        var leaf = LeafScene.Instantiate<TreeLeaf>();
        var branch = BranchScene.Instantiate<Branch>();
        _leaves.AddChild(leaf);
        AddChild(branch);

        // set up collision detection on the leaf
        HashSet<ulong> collidingObjects = new();
        var area2D = leaf.GetNode<Area2D>("Area2D");
        area2D.AreaEntered += other => { collidingObjects.Add(other.GetInstanceId()); };
        area2D.AreaExited += other => { collidingObjects.Remove(other.GetInstanceId()); };

        // move leaf and branch to player's mouse position until placed
        Vector2 lastMousePos = Vector2.Zero;
        while (true)
        {
            // move leaf to mouse position if mouse has moved since last frame
            Vector2 mousePos = GetViewport().GetMousePosition();
            if (mousePos != lastMousePos)
            {
                leaf.GlobalPosition = mousePos;
                branch.BendToPosition(leaf.GlobalPosition, 0.8f);
                lastMousePos = mousePos;
            }

            // if not colliding w/ anything and leaf is within max radius
            if (
                collidingObjects.Count == 0
                && GlobalPosition.DistanceTo(mousePos) < MaxLeafRadius
            )
            {
                // if user clicked, place leaf down
                if (Input.IsMouseButtonPressed(MouseButton.Left))
                {
                    _radiusIndicator.ShowRadius = false;
                    Game.Instance.Matter -= cost;
                    _buttons.Visible = false;
                    leaf.IsPlaced = true;
                    _btnNewBranch.Cost += _perBranchPriceIncrease;
                    leaf.TowerPurchased += (tower) =>
                    {
                        GD.Print("new tower upgrade");
                        tower.ApplyUpgrades(_allUpgradeEffects);
                        leaf.UpdateRangeIndicator(tower.Range);
                    };
                    break;
                }
            }

            // if user right clicked, exit
            if (Input.IsMouseButtonPressed(MouseButton.Right))
            {
                _radiusIndicator.ShowRadius = false;
                branch.QueueFree();
                leaf.QueueFree();
            }

            // wait for next frame
            await ToSignal(GetTree(), "process_frame");
        }
    }

    // upgrades
    private void AddUpgrade(UpgradeEffect effect, int cost)
    {
        Game.Instance.Matter -= cost;

        foreach (var leafNode in _leaves.GetChildren())
        {
            TreeLeaf leaf = leafNode as TreeLeaf;
            Tower tower = leaf!.GetNode("Tower") as Tower;
            effect.Lambda(tower);
            leaf.UpdateRangeIndicator(tower!.Range);
        }

        _allUpgradeEffects.Add(effect);
        RandomizeUpgrades();
    }

    private void RandomizeUpgrades()
    {
        var twoUpgrades = UpgradeEffect.GetTwoRandomUpgrades();
        _btnUpgrade1.BecomeUpgrade(twoUpgrades[0]);
        _btnUpgrade2.BecomeUpgrade(twoUpgrades[1]);
    }
}
