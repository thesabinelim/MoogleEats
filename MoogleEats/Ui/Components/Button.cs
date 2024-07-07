using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Numerics;

namespace MoogleEats.Ui
{
    public partial class Components
    {
        public static void Button(
            string label,
            Action? onPress = null,
            FontAwesomeIcon? icon = null,
            Vector4? color = null,
            float paddingX = 20f,
            float paddingY = 10f,
            float rounding = 5f,
            bool disabled = false
        )
        {
            var colorInit = color.HasValue ? color.Value : ImGuiColors.DalamudGrey;
            if (disabled)
            {
                colorInit = new Vector4(colorInit.X, colorInit.Y, colorInit.Z, colorInit.W * 0.5f);
            }
            var colorHover = !disabled
                ? new Vector4(colorInit.X, colorInit.Y, colorInit.Z, colorInit.W * 0.8f)
                : colorInit;
            ImGui.PushStyleColor(ImGuiCol.Button, ColorHelpers.RgbaVector4ToUint(colorInit));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ColorHelpers.RgbaVector4ToUint(colorHover));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, ColorHelpers.RgbaVector4ToUint(colorHover));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(paddingX, paddingY));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, rounding);

            var isPressed = icon.HasValue
                ? ImGuiComponents.IconButtonWithText(icon.Value, label)
                : ImGui.Button(label);

            if (onPress != null && isPressed)
            {
                onPress();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }

            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            if (color.HasValue)
            {
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();
            }
        }
    }
}
