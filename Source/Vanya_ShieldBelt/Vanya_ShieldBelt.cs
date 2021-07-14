using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VanyaMod
{
    // Token: 0x02000005 RID: 5
    public class Vanya_ShieldBelt : Apparel
    {
        // Token: 0x04000020 RID: 32
        private const float MaxDamagedJitterDist = 0.05f;

        // Token: 0x04000021 RID: 33
        private const int JitterDurationTicks = 8;

        // Token: 0x04000023 RID: 35
        private readonly float ApparelScorePerEnergyMax = 0.25f;

        // Token: 0x04000022 RID: 34
        private readonly int KeepDisplayingTicks = 600;

        // Token: 0x04000025 RID: 37
        private Material bubbleMatInit;

        // Token: 0x04000026 RID: 38
        private bool canFreshNow = true;

        // Token: 0x0400001B RID: 27
        private float energy;

        // Token: 0x04000024 RID: 36
        private CompProperties_VanyaUniversalShieldBelt ExactComp = new CompProperties_VanyaUniversalShieldBelt();

        // Token: 0x0400001E RID: 30
        private Vector3 impactAngleVect;

        // Token: 0x0400001F RID: 31
        private int lastAbsorbDamageTick = -9999;

        // Token: 0x0400001D RID: 29
        private int lastKeepDisplayTick = -9999;

        // Token: 0x0400001C RID: 28
        private int ticksToReset = -1;

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x0600001A RID: 26 RVA: 0x000027D0 File Offset: 0x000009D0
        private CompProperties_VanyaUniversalShieldBelt TempComp
        {
            get
            {
                var result = new CompProperties_VanyaUniversalShieldBelt();
                if (GetComp<Comp_VanyaUniversalShieldBelt>() != null)
                {
                    result = (CompProperties_VanyaUniversalShieldBelt) GetComp<Comp_VanyaUniversalShieldBelt>().props;
                }

                return result;
            }
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x0600001B RID: 27 RVA: 0x000027FD File Offset: 0x000009FD
        private int StartingTicksToReset => ExactComp.startingTicksToReset;

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x0600001C RID: 28 RVA: 0x0000280A File Offset: 0x00000A0A
        private float EnergyOnReset => ExactComp.energyOnReset;

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600001D RID: 29 RVA: 0x00002817 File Offset: 0x00000A17
        // (set) Token: 0x0600001E RID: 30 RVA: 0x00002824 File Offset: 0x00000A24
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

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600001F RID: 31 RVA: 0x00002844 File Offset: 0x00000A44
        private bool CanAbsorbMeleeDamage => ExactComp.canAbsorbMeleeDamage;

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000020 RID: 32 RVA: 0x00002851 File Offset: 0x00000A51
        private float MeleeAbsorbFactor => ExactComp.meleeAbsorbFactor;

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000021 RID: 33 RVA: 0x0000285E File Offset: 0x00000A5E
        private float RangedAbsorbFactor => ExactComp.rangedAbsorbFactor;

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000022 RID: 34 RVA: 0x0000286B File Offset: 0x00000A6B
        private bool CanDodge => ExactComp.canDodge;

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000023 RID: 35 RVA: 0x00002878 File Offset: 0x00000A78
        private float DodgeChanceFactor
        {
            get
            {
                if (ExactComp.dodgeChanceFactor > 1.5f)
                {
                    return 1.5f;
                }

                if (ExactComp.dodgeChanceFactor < 0f)
                {
                    return 0f;
                }

                return ExactComp.dodgeChanceFactor;
            }
        }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x06000024 RID: 36 RVA: 0x000028B5 File Offset: 0x00000AB5
        private bool BreakWithSmoke => ExactComp.breakWithSmoke;

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x06000025 RID: 37 RVA: 0x000028C2 File Offset: 0x00000AC2
        private bool CanForceActive => ExactComp.canForceActive;

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000026 RID: 38 RVA: 0x000028CF File Offset: 0x00000ACF
        private bool FireFoamBurster => ExactComp.fireFoamBurster;

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000027 RID: 39 RVA: 0x000028DC File Offset: 0x00000ADC
        private float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax);

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x06000028 RID: 40 RVA: 0x000028EA File Offset: 0x00000AEA
        private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate) / 60f;

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x06000029 RID: 41 RVA: 0x000028FE File Offset: 0x00000AFE
        public float Energy => energy;

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x0600002A RID: 42 RVA: 0x00002906 File Offset: 0x00000B06
        private ShieldState ShieldState
        {
            get
            {
                if (ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }

                return ShieldState.Active;
            }
        }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x0600002B RID: 43 RVA: 0x00002914 File Offset: 0x00000B14
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

        // Token: 0x0600002C RID: 44 RVA: 0x00002996 File Offset: 0x00000B96
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            ExactComp = TempComp;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x000029AC File Offset: 0x00000BAC
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

        // Token: 0x0600002E RID: 46 RVA: 0x00002A0D File Offset: 0x00000C0D
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

        // Token: 0x0600002F RID: 47 RVA: 0x00002A1D File Offset: 0x00000C1D
        public override float GetSpecialApparelScoreOffset()
        {
            return EnergyMax * ApparelScorePerEnergyMax;
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00002A2C File Offset: 0x00000C2C
        public override void Tick()
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

        // Token: 0x06000031 RID: 49 RVA: 0x00002ACC File Offset: 0x00000CCC
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

        // Token: 0x06000032 RID: 50 RVA: 0x00002CDF File Offset: 0x00000EDF
        private void KeepDisplaying()
        {
            lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002CF4 File Offset: 0x00000EF4
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            if (Wearer.Map != null)
            {
                SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
                impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                var loc = Wearer.TrueCenter() + (impactAngleVect.RotatedBy(180f) * 0.5f);
                var num = Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
                FleckMaker.Static(loc, Wearer.Map, FleckDefOf.ExplosionFlash, num);
                var num2 = (int) num;
                for (var i = 0; i < num2; i++)
                {
                    FleckMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
                }
            }

            lastAbsorbDamageTick = Find.TickManager.TicksGame;
            KeepDisplaying();
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00002DF8 File Offset: 0x00000FF8
        private void Break()
        {
            if (Wearer.Map != null)
            {
                SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
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

        // Token: 0x06000035 RID: 53 RVA: 0x00002F00 File Offset: 0x00001100
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

        // Token: 0x06000036 RID: 54 RVA: 0x00002F78 File Offset: 0x00001178
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

            var angle = (float) Rand.Range(0, 360);
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, bubbleMatInit, 0);
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00003064 File Offset: 0x00001264
        private void BurstSmoke()
        {
            var position = Wearer.Position;
            var map = Wearer.Map;
            var statValue = this.GetStatValue(StatDefOf.SmokepopBeltRadius);
            var smoke = DamageDefOf.Smoke;
            var gas_Smoke = ThingDefOf.Gas_Smoke;
            GenExplosion.DoExplosion(position, map, statValue, smoke, null, -1, -1f, null, null, null, null, gas_Smoke,
                1f);
        }

        // Token: 0x06000038 RID: 56 RVA: 0x000030D8 File Offset: 0x000012D8
        private void FreshCompData()
        {
            ExactComp = TempComp;
            canFreshNow = false;
        }

        // Token: 0x06000039 RID: 57 RVA: 0x000030F0 File Offset: 0x000012F0
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
}