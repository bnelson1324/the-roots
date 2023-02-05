using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using roottowerdefense.tree.tower;
using Array = Godot.Collections.Array;

namespace roottowerdefense.tree.upgrade;

public enum Upgrade
{
    Damage,
    FireRate,
    Range,
    Aoe
}

public record UpgradeEffect(string Name, UpgradeEffect.UpgradeLambda Lambda)
{
    /* apply upgrade's lambda in 2 cases:
     *      when the upgrade is purchased: apply the purchased lambda to all existing towers
	        when a new tower is purchased: apply all lambdas ever purchased to just that tower
     */
    
    public static Upgrade[] GetTwoRandomUpgrades()
    {
        List<Upgrade> chosenUpgrades = new();
        var allUpgrades = Enum.GetValues(typeof(Upgrade));

        long index1 = GD.Randi() % allUpgrades.Length;
        long index2 = (index1 + 1 + (GD.Randi() % (allUpgrades.Length - 1))) % allUpgrades.Length;
        return new Upgrade[] { (Upgrade)allUpgrades.GetValue(index1)!, (Upgrade)allUpgrades.GetValue(index2)! };
    }

    public delegate void UpgradeLambda(Tower tower);

    public static readonly UpgradeEffect UpgradeDamage =
        new UpgradeEffect("Upgrade damage", (tower) => { tower.ProjectileDamage += 20; });

    public static readonly UpgradeEffect UpgradeFireRate =
        new UpgradeEffect("Upgrade firerate", (tower) => { tower.AttackDelay *= 0.8f; });

    public static readonly UpgradeEffect UpgradeRange =
        new UpgradeEffect("Upgrade range", (tower) => { tower.Range += 100; });

    public static readonly UpgradeEffect UpgradeAoe =
        new UpgradeEffect("Upgrade area of effect", (tower) => { tower.ProjectileAoeRadius += 50; });
}
