using ImGuiNET;

namespace MoogleEats.Ui
{
    public partial class Components
    {
        public static void TextInput(
            string label,
            ref string value,
            uint maxLength
        )
        {
            ImGui.InputText(label, ref value, maxLength);
        }
    }
}
