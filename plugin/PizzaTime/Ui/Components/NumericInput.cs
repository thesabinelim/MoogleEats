using ImGuiNET;

namespace PizzaTime.Ui;

internal sealed partial class Components
{
    internal static void NumericInput(
        string label,
        ref int value
    )
    {
        ImGui.InputInt(label, ref value);
    }
}
