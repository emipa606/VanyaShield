using Verse;
using HarmonyLib;
using System.Reflection;

namespace Vanya_ShieldBelt
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            Harmony instance = new Harmony("Vanya_ShieldBelt");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
