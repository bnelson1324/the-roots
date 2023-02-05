using Godot;

namespace roottowerdefense;

public partial class TutorialScreen : Control
{

    public Button BtnStart;
    public override void _Ready()
    {
        BtnStart = GetNode<Button>("BtnStart");
        var text = GetNode<Label>("Text");
        BtnStart.Pressed += () => { Visible = false; };
    }
}
