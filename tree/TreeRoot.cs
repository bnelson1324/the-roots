using System.Collections.Generic;
using Godot;
using roottowerdefense.tree.ui;

namespace roottowerdefense.tree;

public partial class TreeRoot : Node2D
{
    [Export] public int MaxLeafRadius = 150;
    [Export] public PackedScene LeafScene;
    [Export] public PackedScene BranchScene;
    
    private RadiusIndicator _radiusIndicator;

    public override void _Ready()
    {
        // set up button signals
        var buttons = GetNode<Control>("Buttons");

        var btnRoot = GetNode<TextureButton>("BtnRoot");
        btnRoot.Pressed += () => { buttons.Visible = !buttons.Visible; };

        var btnNewBranch = buttons.GetNode<PurchaseButton>("BtnNewBranch");
        btnNewBranch.OnPurchased += CreateBranch;

        // sets up radius indicator
        _radiusIndicator = GetNode<RadiusIndicator>("RadiusIndicator");
        _radiusIndicator.Radius = MaxLeafRadius;
    }

    private async void CreateBranch(int cost)
    {
        _radiusIndicator.ShowRadius = true;

        // spawn a leaf and a branch
        var leaf = LeafScene.Instantiate<Node2D>();
        var branch = BranchScene.Instantiate<Branch>();
        AddChild(leaf);
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
}
