using System.Reflection;
using HarmonyLib;
using Verse;

namespace Vanya_ShieldBelt;

[StaticConstructorOnStartup]
public class PatchMain
{
    static PatchMain()
    {
        new Harmony("Vanya_ShieldBelt").PatchAll(Assembly.GetExecutingAssembly());
    }
}