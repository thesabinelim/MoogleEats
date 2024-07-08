using ImGuiNET;

namespace MoogleEats.Ui;

internal sealed partial class Components
{
    internal static void TextInput(
        string label,
        ref string value,
        uint maxLength
    )
    {
        ImGui.InputText(label, ref value, maxLength);
    }
}
