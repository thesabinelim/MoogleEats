using Dalamud.Interface.Components;
using ImGuiNET;

namespace MoogleEats.Ui.MainWindow
{
    public partial class Tabs
    {
        public static void SettingsTab(Settings settings)
        {
            var enableOrderStatusNotifications = settings.EnableOrderStatusNotifications;
            if (ImGuiComponents.ToggleButton("enableOrderStatusNotifications", ref enableOrderStatusNotifications))
            {
                settings.EnableOrderStatusNotifications = enableOrderStatusNotifications;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }
            ImGui.SameLine();
            ImGui.Text("Enable order status notifications");
        }
    }
}
