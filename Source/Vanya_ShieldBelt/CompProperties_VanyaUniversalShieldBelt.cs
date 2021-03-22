using UnityEngine;
using Verse;

namespace VanyaMod
{
    // Token: 0x02000003 RID: 3
    public class CompProperties_VanyaUniversalShieldBelt : CompProperties
    {
        // Token: 0x0400000F RID: 15
        public readonly string alias = string.Empty;

        // Token: 0x04000010 RID: 16
        public readonly string bubbleTexPath = "Other/Vanya_ShieldBubble";

        // Token: 0x04000006 RID: 6
        public readonly bool canAbsorbMeleeDamage = true;

        // Token: 0x0400000A RID: 10
        public readonly float dodgeChanceFactor = 0.5f;

        // Token: 0x04000004 RID: 4
        public readonly float energyOnReset = 0.2f;

        // Token: 0x04000007 RID: 7
        public readonly float meleeAbsorbFactor = 2f;

        // Token: 0x04000008 RID: 8
        public readonly float rangedAbsorbFactor = 1f;

        // Token: 0x04000003 RID: 3
        public readonly int startingTicksToReset = 2400;

        // Token: 0x04000013 RID: 19
        public readonly int tabID = 5292000;

        // Token: 0x0400000E RID: 14
        public readonly string versionInfo = string.Empty;

        // Token: 0x0400000B RID: 11
        public bool breakWithSmoke;

        // Token: 0x04000011 RID: 17
        public Color bubbleColor;

        // Token: 0x04000009 RID: 9
        public bool canDodge;

        // Token: 0x0400000C RID: 12
        public bool canForceActive;

        // Token: 0x04000012 RID: 18
        public bool destroyOnBreak;

        // Token: 0x04000005 RID: 5
        public float energyLossPerDamage = 0.033f;

        // Token: 0x0400000D RID: 13
        public bool fireFoamBurster;

        // Token: 0x06000015 RID: 21 RVA: 0x00002618 File Offset: 0x00000818
        public CompProperties_VanyaUniversalShieldBelt()
        {
            compClass = typeof(Comp_VanyaUniversalShieldBelt);
        }
    }
}