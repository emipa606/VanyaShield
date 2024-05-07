using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Verse.DamageWorker;

namespace Vanya_ShieldBelt
{
    [HarmonyPatch(typeof(HealthUtility), "SimulateKilled")]
    public class PatchTest
    {
        [HarmonyPrefix]
        public static void prefix(Pawn p, DamageDef damage, ThingDef sourceDef, Tool sourceTool)
        {
            List<Apparel> wornApparel = p.apparel.WornApparel.ToList();
            for (int i = 0; i < wornApparel.Count; ++i)
            {
                if (wornApparel[i] is VanyaMod.Vanya_ShieldBelt)
                {
                    p.apparel.Remove(wornApparel[i]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public class Patchtkdmg
    {
        [HarmonyPostfix]
        public static void post(DamageInfo dinfo, ref DamageResult __result)
        {
            if (__result.hediffs == null) __result.hediffs = new List<Hediff>();
        }

    }
}
