using RimWorld;
using Verse;

namespace VanyaMod
{
    // Token: 0x02000002 RID: 2
    public class Comp_VanyaUniversalShieldBelt : ThingComp
    {
        // Token: 0x04000002 RID: 2
        private bool canFreshNow = true;

        // Token: 0x04000001 RID: 1
        private CompProperties_VanyaUniversalShieldBelt ExactProps = new();

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private CompProperties_VanyaUniversalShieldBelt Props => (CompProperties_VanyaUniversalShieldBelt) props;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000002 RID: 2 RVA: 0x0000205D File Offset: 0x0000025D
        private string VersionInfo => ExactProps.versionInfo;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000003 RID: 3 RVA: 0x0000206A File Offset: 0x0000026A
        private string Alias => ExactProps.alias;

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
        private string ResetingTimeSec => (ExactProps.startingTicksToReset / 60).ToString("0.0");

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000005 RID: 5 RVA: 0x000020A0 File Offset: 0x000002A0
        private string EnergyOnReset => ((int) (ExactProps.energyOnReset * 100f)) + "%";

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000006 RID: 6 RVA: 0x000020D4 File Offset: 0x000002D4
        private string EnergyLossPerDamage => (ExactProps.energyLossPerDamage * 100f).ToString("0.00") + "%";

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002109 File Offset: 0x00000309
        private bool CanAbsorbMeleeDamage => ExactProps.canAbsorbMeleeDamage;

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000008 RID: 8 RVA: 0x00002118 File Offset: 0x00000318
        private string MeleeCostFactor => ((int) (ExactProps.meleeAbsorbFactor * 100f)) + "%";

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000009 RID: 9 RVA: 0x0000214C File Offset: 0x0000034C
        private string RangedCostFactor => ((int) (ExactProps.rangedAbsorbFactor * 100f)) + "%";

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600000A RID: 10 RVA: 0x0000217D File Offset: 0x0000037D
        private bool CanDodge => ExactProps.canDodge;

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600000B RID: 11 RVA: 0x0000218C File Offset: 0x0000038C
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

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x0600000C RID: 12 RVA: 0x000021C4 File Offset: 0x000003C4
        private string DodgeChanceExact
        {
            get
            {
                if (parent is Apparel {Wearer: { }} apparel)
                {
                    return (apparel.Wearer.GetStatValue(StatDefOf.MeleeDodgeChance) * DodgeChanceFactor * 100f)
                        .ToString("0.0") + "%";
                }

                return (DodgeChanceFactor * 100f).ToString("0.0") + "%";
            }
        }

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x0600000D RID: 13 RVA: 0x00002241 File Offset: 0x00000441
        private bool BreakWithSmoke => ExactProps.breakWithSmoke;

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600000E RID: 14 RVA: 0x0000224E File Offset: 0x0000044E
        private bool CanForceActive => ExactProps.canForceActive;

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x0600000F RID: 15 RVA: 0x0000225B File Offset: 0x0000045B
        private bool FireFoamBurster => ExactProps.fireFoamBurster;

        // Token: 0x06000010 RID: 16 RVA: 0x00002268 File Offset: 0x00000468
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            ExactProps = Props;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x0000227D File Offset: 0x0000047D
        public override void CompTick()
        {
            base.CompTick();
            if (canFreshNow)
            {
                FreshPropsData();
            }
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002294 File Offset: 0x00000494
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
                if (parent is Apparel {Wearer: { }} apparel)
                {
                    text += "VSDodgeChanceWorn".Translate(DodgeChanceExact,
                        (apparel.Wearer.GetStatValue(StatDefOf.MeleeDodgeChance) * 100f).ToString("0.0") + "%",
                        (DodgeChanceFactor * 100f).ToString("0.0") + "%") + "\n";
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

        // Token: 0x06000013 RID: 19 RVA: 0x000025E9 File Offset: 0x000007E9
        private void FreshPropsData()
        {
            ExactProps = Props;
            canFreshNow = false;
        }
    }
}