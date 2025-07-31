using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VanyaMod;

public class Vanya_ShieldBelt : Apparel
{
    private const float MaxDamagedJitterDist = 0.05f;

    private const int JitterDurationTicks = 8;

    private readonly float ApparelScorePerEnergyMax = 0.25f;

    private readonly SoundDef EnergyShield_Broken = SoundDef.Named("EnergyShield_Broken");

    private readonly int KeepDisplayingTicks = 600;
    private readonly StatDef PackRadius = StatDef.Named("PackRadius");

    private Material bubbleMatInit;

    private bool canFreshNow = true;

    private float energy;

    private CompProperties_VanyaUniversalShieldBelt ExactComp = new();

    private Vector3 impactAngleVect;

    private int lastAbsorbDamageTick = -9999;

    private int lastKeepDisplayTick = -9999;

    private int ticksToReset = -1;

    private CompProperties_VanyaUniversalShieldBelt TempComp
    {
        get
        {
            var result = new CompProperties_VanyaUniversalShieldBelt();
            if (GetComp<Comp_VanyaUniversalShieldBelt>() != null)
            {
                result = (CompProperties_VanyaUniversalShieldBelt)GetComp<Comp_VanyaUniversalShieldBelt>().props;
            }

            return result;
        }
    }

    private int StartingTicksToReset => ExactComp.startingTicksToReset;

    private float EnergyOnReset => ExactComp.energyOnReset;

    private float EnergyLossPerDamage
    {
        get => ExactComp.energyLossPerDamage;
        set
        {
            if (ExactComp.energyLossPerDamage < 0.001f)
            {
                ExactComp.energyLossPerDamage = value;
            }
        }
    }

    private bool CanAbsorbMeleeDamage => ExactComp.canAbsorbMeleeDamage;

    private float MeleeAbsorbFactor => ExactComp.meleeAbsorbFactor;

    private float RangedAbsorbFactor => ExactComp.rangedAbsorbFactor;

    private bool CanDodge => ExactComp.canDodge;

    private float DodgeChanceFactor
    {
        get
        {
            if (ExactComp.dodgeChanceFactor > 1.5f)
            {
                return 1.5f;
            }

            return ExactComp.dodgeChanceFactor < 0f ? 0f : ExactComp.dodgeChanceFactor;
        }
    }

    private bool BreakWithSmoke => ExactComp.breakWithSmoke;

    private bool CanForceActive => ExactComp.canForceActive;

    private bool FireFoamBurster => ExactComp.fireFoamBurster;

    private float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax);

    private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate) / 60f;

    public float Energy => energy;

    private ShieldState ShieldState => ticksToReset > 0 ? ShieldState.Resetting : ShieldState.Active;

    private bool ShouldDisplay
    {
        get
        {
            var wearer = Wearer;
            return wearer.Spawned && !wearer.Dead && !wearer.Downed && !canFreshNow && (wearer.InAggroMentalState ||
                wearer.Drafted || wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner ||
                Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks);
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        ExactComp = TempComp;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref energy, "energy");
        Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1);
        Scribe_Values.Look(ref lastKeepDisplayTick, "lastKeepDisplayTick");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            ExactComp = TempComp;
        }
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        if (Find.Selector.SingleSelectedThing != Wearer)
        {
            yield break;
        }

        if (ShieldState != ShieldState.Active && CanForceActive)
        {
            yield return new Command_Action
            {
                action = delegate
                {
                    if (HitPoints <= 20)
                    {
                        Messages.Message("VSBNoEnoughHitPointsToReset".Translate(), Wearer,
                            MessageTypeDefOf.NegativeEvent);
                        return;
                    }

                    HitPoints -= 20;
                    Reset();
                },
                defaultLabel = "VSBForceResetLabel".Translate(),
                defaultDesc = "VSBForceResetDESC".Translate(),
                icon = TexCommand.DesirePower,
                hotKey = KeyBindingDefOf.Designator_Cancel
            };
        }

        yield return new Gizmo_Vanya_ShieldStatus
        {
            shield = this,
            tabID = ExactComp.tabID
        };
    }

    public override float GetSpecialApparelScoreOffset()
    {
        return EnergyMax * ApparelScorePerEnergyMax;
    }

    protected override void Tick()
    {
        base.Tick();
        if (Wearer == null)
        {
            energy = ExactComp.energyOnReset;
            return;
        }

        if (canFreshNow)
        {
            FreshCompData();
            FreshBubble();
        }

        if (ShieldState == ShieldState.Resetting)
        {
            ticksToReset--;
            if (ticksToReset <= 0)
            {
                Reset();
            }
        }
        else if (ShieldState == ShieldState.Active)
        {
            energy += EnergyGainPerTick;
            if (energy > EnergyMax)
            {
                energy = EnergyMax;
            }
        }
    }

    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (ShieldState != ShieldState.Active)
        {
            return false;
        }

        if (dinfo.Instigator == Wearer)
        {
            return true;
        }

        if (dinfo.Def == DamageDefOf.SurgicalCut)
        {
            return false;
        }

        if (dinfo.Def == DamageDefOf.Extinguish)
        {
            return true;
        }

        if (FireFoamBurster && dinfo.Def == DamageDefOf.Flame && dinfo.Instigator.def == ThingDefOf.Fire)
        {
            GenExplosion.DoExplosion(Wearer.PositionHeld, Wearer.MapHeld, 1.5f, DamageDefOf.Extinguish, Wearer,
                9999, -1f, DamageDefOf.Extinguish.soundExplosion, def, null, null, ThingDefOf.Filth_FireFoam, 1.5f,
                2);
            return true;
        }

        if (CanDodge && Wearer != null &&
            Rand.Chance(Wearer.GetStatValue(StatDefOf.MeleeDodgeChance) * DodgeChanceFactor))
        {
            MoteMaker.ThrowText(Wearer.DrawPos, Wearer.Map, "TextMote_Dodge".Translate(), 1.9f);
            return true;
        }

        if (dinfo.Instigator != null)
        {
            if (Wearer != null && dinfo.Instigator.Position.AdjacentTo8WayOrInside(Wearer.Position))
            {
                if (!CanAbsorbMeleeDamage)
                {
                    return false;
                }

                energy -= dinfo.Amount * EnergyLossPerDamage * MeleeAbsorbFactor;
            }
            else
            {
                energy -= dinfo.Amount * EnergyLossPerDamage * RangedAbsorbFactor;
            }

            if (energy < 0f)
            {
                Break();
            }
            else
            {
                AbsorbedDamage(dinfo);
            }

            return true;
        }

        energy -= dinfo.Amount * EnergyLossPerDamage;
        if (energy < 0f)
        {
            Break();
        }
        else
        {
            AbsorbedDamage(dinfo);
        }

        return true;
    }

    private void KeepDisplaying()
    {
        lastKeepDisplayTick = Find.TickManager.TicksGame;
    }

    private void AbsorbedDamage(DamageInfo dinfo)
    {
        if (Wearer.Map != null)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            var loc = Wearer.TrueCenter() + (impactAngleVect.RotatedBy(180f) * 0.5f);
            var num = Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
            FleckMaker.Static(loc, Wearer.Map, FleckDefOf.ExplosionFlash, num);
            var num2 = (int)num;
            for (var i = 0; i < num2; i++)
            {
                FleckMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
        }

        lastAbsorbDamageTick = Find.TickManager.TicksGame;
        KeepDisplaying();
    }

    private void Break()
    {
        if (Wearer.Map != null)
        {
            EnergyShield_Broken.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            FleckMaker.Static(Wearer.TrueCenter(), Wearer.Map, FleckDefOf.ExplosionFlash, 12f);
            for (var i = 0; i < 6; i++)
            {
                FleckMaker.ThrowDustPuff(
                    Wearer.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) *
                                           Rand.Range(0.3f, 0.6f)), Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
        }

        energy = 0f;
        ticksToReset = StartingTicksToReset;
        if (BreakWithSmoke)
        {
            BurstSmoke();
        }

        if (ExactComp.destroyOnBreak)
        {
            Destroy();
        }
    }

    private void Reset()
    {
        if (Wearer.Spawned)
        {
            SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            FleckMaker.ThrowLightningGlow(Wearer.TrueCenter(), Wearer.Map, 3f);
        }

        ticksToReset = -1;
        energy = EnergyOnReset;
    }

    public override void DrawWornExtras()
    {
        if (ShieldState != ShieldState.Active || !ShouldDisplay)
        {
            return;
        }

        var num = Mathf.Lerp(1.2f, 2f, energy / 3f);
        var vector = Wearer.Drawer.DrawPos;
        vector.y = AltitudeLayer.Blueprint.AltitudeFor();
        var num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
        if (num2 < 8)
        {
            var num3 = (8 - num2) / 8f * 0.05f;
            vector += impactAngleVect * num3;
            num -= num3;
        }

        var angle = (float)Rand.Range(0, 360);
        var s = new Vector3(num, 1f, num);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, bubbleMatInit, 0);
    }

    private void BurstSmoke()
    {
        var position = Wearer.Position;
        var map = Wearer.Map;
        var statValue = this.GetStatValue(PackRadius);
        var smoke = DamageDefOf.Smoke;
        GenExplosion.DoExplosion(position, map, statValue, smoke, null, -1, -1f, null, null, null, null, null, 0f, 1,
            GasType.BlindSmoke);
    }

    private void FreshCompData()
    {
        ExactComp = TempComp;
        canFreshNow = false;
    }

    private void FreshBubble()
    {
        if (ExactComp.bubbleColor != default)
        {
            bubbleMatInit = MaterialPool.MatFrom(ExactComp.bubbleTexPath, ShaderDatabase.Transparent,
                ExactComp.bubbleColor);
            return;
        }

        bubbleMatInit = MaterialPool.MatFrom(ExactComp.bubbleTexPath, ShaderDatabase.Transparent);
    }
}