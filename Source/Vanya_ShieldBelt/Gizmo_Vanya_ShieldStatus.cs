using RimWorld;
using UnityEngine;
using Verse;

namespace VanyaMod;

[StaticConstructorOnStartup]
internal class Gizmo_Vanya_ShieldStatus : Gizmo
{
    private static readonly Texture2D FullShieldBarTexDefault =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

    private static readonly Texture2D FullShieldBarTexOrigin =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.3f, 0.4f));

    private static readonly Texture2D FullShieldBarTexVV =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.8f, 0.4f, 0f));

    private static readonly Texture2D FullShieldBarTexVR =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0f, 0f));

    private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

    public Vanya_ShieldBelt shield;

    public int tabID;

    public Gizmo_Vanya_ShieldStatus()
    {
        Order = -110f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 140f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        Find.WindowStack.ImmediateWindow(tabID, overRect, WindowLayer.GameUI, delegate
        {
            Rect rect2;
            var rect = rect2 = overRect.AtZero().ContractedBy(6f);
            rect2.height = overRect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect2, shield.LabelCap);
            var rect3 = rect;
            rect3.yMin = overRect.height / 2f;
            var fillPercent = shield.Energy / Mathf.Max(1f, shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax));
            Texture2D fillTex;
            var num = tabID;
            if (num != 1817000)
            {
                if (num != 1822036)
                {
                    fillTex = num == 5292001 ? FullShieldBarTexOrigin : FullShieldBarTexDefault;
                }
                else
                {
                    fillTex = FullShieldBarTexVV;
                }
            }
            else
            {
                fillTex = FullShieldBarTexVR;
            }

            Widgets.FillableBar(rect3, fillPercent, fillTex, EmptyShieldBarTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect3,
                (shield.Energy * 100f).ToString("F0") + " / " +
                (shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax) * 100f).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
        });
        return new GizmoResult(GizmoState.Clear);
    }
}