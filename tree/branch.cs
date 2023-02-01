using Godot;

namespace roottowerdefense.tree;

public partial class Branch : Sprite2D
{
    private Vector2 _startScale;

    public override void _Ready()
    {
        _startScale = Scale;
    }

    public bool BendToPosition(Vector2 newPosition, float mainBranchScale)
    {
        // bend main branch
        Scale = new Vector2(_startScale.X * (GD.Randf() + mainBranchScale), _startScale.Y);
        Vector2 posDiff = newPosition - GlobalPosition;
        float angle = Mathf.Atan2(posDiff.Y, posDiff.X);
        Rotation = angle + RandomRotationInRange(-Mathf.Pi / 4, Mathf.Pi / 4);

        // bend stem branch
        Sprite2D stem = GetNode<Sprite2D>("Stem");
        Vector2 stemPosDiff = newPosition - stem.GlobalPosition;
        stem.GlobalRotation = Mathf.Atan2(stemPosDiff.Y, stemPosDiff.X);

        // resize stem branch
        float newScale = stemPosDiff.Length() / Texture.GetWidth();
        stem.GlobalScale = new Vector2(newScale, GlobalScale.Y);

        return true;
    }

    private float RandomRotationInRange(float minRot, float maxRot)
    {
        return minRot + GD.Randf() * (maxRot - minRot);
    }
}
