using Godot;

namespace roottowerdefense.tree;

public partial class RadiusIndicator : Node2D
{
    private float _radius;
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            QueueRedraw();
        }
    }

    private bool _showRadius;

    public bool ShowRadius
    {
        get => _showRadius;
        set
        {
            _showRadius = value;
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        base._Draw();
        if (ShowRadius)
        {
            DrawArc(Vector2.Zero, Radius / GlobalScale.X, 0, Mathf.Tau, 64, Colors.Red);
        }
    }
}
