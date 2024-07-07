using ImGuiNET;

namespace MoogleEats.Ui
{
    public partial class Components
    {
        public static void NumericInput(
            string label,
            ref int value
        )
        {
            ImGui.InputInt(label, ref value);
        }
    }
}
