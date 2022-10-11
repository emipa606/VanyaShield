using UnityEngine;
using Verse;

namespace VanyaMod;

public class CompProperties_VanyaUniversalShieldBelt : CompProperties
{
    public readonly string alias = string.Empty;

    public readonly string bubbleTexPath = "Other/Vanya_ShieldBubble";

    public readonly bool canAbsorbMeleeDamage = true;

    public readonly float dodgeChanceFactor = 0.5f;

    public readonly float energyOnReset = 0.2f;

    public readonly float meleeAbsorbFactor = 2f;

    public readonly float rangedAbsorbFactor = 1f;

    public readonly int startingTicksToReset = 2400;

    public readonly int tabID = 5292000;

    public readonly string versionInfo = string.Empty;

    public bool breakWithSmoke;

    public Color bubbleColor;

    public bool canDodge;

    public bool canForceActive;

    public bool destroyOnBreak;

    public float energyLossPerDamage = 0.033f;

    public bool fireFoamBurster;

    public CompProperties_VanyaUniversalShieldBelt()
    {
        compClass = typeof(Comp_VanyaUniversalShieldBelt);
    }
}