using ImGuiNET;

namespace PizzaTime.Ui;

internal sealed partial class Components
{
    internal static void NumericInput(
        string label,
        ref int value
    )
    {
        ImGui.PushItemWidth(120);
        ImGui.InputInt(label, ref value);
        ImGui.PopItemWidth();
    }
}
