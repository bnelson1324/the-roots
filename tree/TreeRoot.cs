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

        // move leaf and branch to player's mouse position until placed
        Vector2 lastMousePos = Vector2.Zero;
        while (true)
        {
            // randomize leaf position
            Vector2 mousePos = GetViewport().GetMousePosition();
            if (
                mousePos != lastMousePos // mouse moved since last frame
                && GlobalPosition.DistanceTo(mousePos) < MaxLeafRadius) // leaf within max radius
            {
                leaf.GlobalPosition = mousePos;
                branch.BendToPosition(leaf.GlobalPosition, 0.8f);
                lastMousePos = mousePos;
            }

            // if user clicked, place leaf down
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                break;
            }

            // wait for next frame
            await ToSignal(GetTree(), "process_frame");
        }
    }
}
