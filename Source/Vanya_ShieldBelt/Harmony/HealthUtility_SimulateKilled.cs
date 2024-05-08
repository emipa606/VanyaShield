using System.Linq;
using HarmonyLib;
using Verse;

namespace Vanya_ShieldBelt;

[HarmonyPatch(typeof(HealthUtility), nameof(HealthUtility.SimulateKilled))]
public class HealthUtility_SimulateKilled
{
    public static void Prefix(Pawn p)
    {
        var wornApparel = p.apparel.WornApparel.ToList();
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < wornApparel.Count; ++i)
        {
            if (wornApparel[i] is VanyaMod.Vanya_ShieldBelt)
            {
                p.apparel.Remove(wornApparel[i]);
            }
        }
    }
}