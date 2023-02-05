using Godot;
using roottowerdefense.tree.ui;

namespace roottowerdefense.tree.upgrade;

public partial class UpgradeButton : PurchaseButton
{
    [Export] private Texture2D _upgradeDamageTexture;
    [Export] private Texture2D _upgradeFireRateTexture;
    [Export] private Texture2D _upgradeRangeTexture;
    [Export] private Texture2D _upgradeAoeTexture;

    public UpgradeEffect Effect { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        OnPurchased += (cost) => { Game.Instance.GetAudioPlayer("Purchase").Play(); };
    }

    public void BecomeUpgrade(Upgrade upgrade)
    {
        switch (upgrade)
        {
            case Upgrade.Damage:
                TextureNormal = _upgradeDamageTexture;
                Effect = UpgradeEffect.UpgradeDamage;
                break;
            case Upgrade.FireRate:
                TextureNormal = _upgradeFireRateTexture;
                Effect = UpgradeEffect.UpgradeFireRate;
                break;
            case Upgrade.Range:
                TextureNormal = _upgradeRangeTexture;
                Effect = UpgradeEffect.UpgradeRange;
                break;
            case Upgrade.Aoe:
                TextureNormal = _upgradeAoeTexture;
                Effect = UpgradeEffect.UpgradeAoe;
                break;
        }

        TooltipText = Effect.Name;
    }
}
