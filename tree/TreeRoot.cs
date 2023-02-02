using System.Collections.Generic;
using System.Data;
using Godot;

namespace roottowerdefense.tree;

public partial class TreeRoot : Node2D
{
    [Export] public int MaxLeafRadius = 150;
    [Export] public PackedScene LeafScene;
    [Export] public PackedScene BranchScene;

    public override void _Ready()
    {
        // set up button signals
        var buttons = GetNode<Control>("Buttons");

        var btnRoot = GetNode<TextureButton>("BtnRoot");
        btnRoot.Pressed += () => { buttons.Visible = !buttons.Visible; };

        var btnNewBranch = buttons.GetNode<TextureButton>("BtnNewBranch");
        btnNewBranch.Pressed += CreateBranch;
    }

    private async void CreateBranch()
    {
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
                    break;
                }
            }

            // if user right clicked, exit
            if (Input.IsMouseButtonPressed(MouseButton.Right))
            {
                branch.QueueFree();
                leaf.QueueFree();
            }

            // wait for next frame
            await ToSignal(GetTree(), "process_frame");
        }
    }
}
