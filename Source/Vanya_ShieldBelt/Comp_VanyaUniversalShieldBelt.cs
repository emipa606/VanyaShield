using RimWorld;
using Verse;

namespace VanyaMod;

public class Comp_VanyaUniversalShieldBelt : ThingComp
{
    private bool canFreshNow = true;

    private CompProperties_VanyaUniversalShieldBelt ExactProps = new CompProperties_VanyaUniversalShieldBelt();

    private CompProperties_VanyaUniversalShieldBelt Props => (CompProperties_VanyaUniversalShieldBelt)props;

    private string VersionInfo => ExactProps.versionInfo;

    private string Alias => ExactProps.alias;

    private string ResetingTimeSec => ExactProps.startingTicksToReset.ToStringSecondsFromTicks();

    private string EnergyOnReset => ExactProps.energyOnReset.ToStringPercent();

    private string EnergyLossPerDamage => ExactProps.energyLossPerDamage.ToStringPercent();

    private bool CanAbsorbMeleeDamage => ExactProps.canAbsorbMeleeDamage;

    private string MeleeCostFactor => ExactProps.meleeAbsorbFactor.ToStringPercent();

    private string RangedCostFactor => ExactProps.rangedAbsorbFactor.ToStringPercent();

    private bool CanDodge => ExactProps.canDodge;

    private float DodgeChanceFactor
    {
        get
        {
            var num = ExactProps.dodgeChanceFactor;
            if (num < 0f)
            {
                num = 0f;
            }

            if (num > 1.5f)
            {
                num = 1.5f;
            }

            return num;
        }
    }

    private string DodgeChanceExact
    {
        get
        {
            if (parent is Apparel { Wearer: not null } apparel)
            {
                return (apparel.Wearer.GetStatValue(StatDefOf.MeleeDodgeChance) * DodgeChanceFactor).ToStringPercent();
            }

            return DodgeChanceFactor.ToStringPercent();
        }
    }

    private bool BreakWithSmoke => ExactProps.breakWithSmoke;

    private bool CanForceActive => ExactProps.canForceActive;

    private bool FireFoamBurster => ExactProps.fireFoamBurster;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        ExactProps = Props;
    }

    public override void CompTick()
    {
        base.CompTick();
        if (canFreshNow)
        {
            FreshPropsData();
        }
    }

    public override string GetDescriptionPart()
    {
        var text = string.Empty;
        if (!ExactProps.versionInfo.NullOrEmpty())
        {
            text += "VSVersionInfo".Translate(VersionInfo) + "\n";
        }

        if (!ExactProps.alias.NullOrEmpty())
        {
            text += "VSAlias".Translate(Alias) + "\n";
        }

        text += "VSResetingTimeSec".Translate(ResetingTimeSec) + "\n";
        text += "VSEnergyOnReset".Translate(EnergyOnReset) + "\n";
        text += "VSEnergyLossPerDamage".Translate(EnergyLossPerDamage) + "\n";
        if (CanAbsorbMeleeDamage)
        {
            text += "VSMeleeCostFactor".Translate(MeleeCostFactor) + "\n";
        }

        text += "VSRangedCostFactor".Translate(RangedCostFactor) + "\n";
        if (CanDodge)
        {
            if (parent is Apparel { Wearer: not null } apparel)
            {
                text += "VSDodgeChanceWorn".Translate(DodgeChanceExact,
                    apparel.Wearer.GetStatValue(StatDefOf.MeleeDodgeChance).ToStringPercent(),
                    DodgeChanceFactor.ToStringPercent()) + "\n";
            }
            else
            {
                text += "VSDodgeChanceDropped".Translate(DodgeChanceExact) + "\n";
            }
        }

        if (CanForceActive || CanDodge || BreakWithSmoke || FireFoamBurster)
        {
            text += "VSExtraFunction".Translate();
            if (BreakWithSmoke)
            {
                text += "VSBreakWithSmoke".Translate() + " ";
            }

            if (CanForceActive)
            {
                text += "VSForceActive".Translate() + " ";
            }

            if (CanDodge)
            {
                text += "VSCanDodge".Translate() + " ";
            }

            if (FireFoamBurster)
            {
                text += "VSFireFoamBurster".Translate() + " ";
            }

            text += "\n";
        }

        if (!CanAbsorbMeleeDamage)
        {
            text += "VSCantAbsorbMeleeDamage".Translate() + "\n";
        }

        return text;
    }

    private void FreshPropsData()
    {
        ExactProps = Props;
        canFreshNow = false;
    }
}