using Dalamud.Interface.Components;
using ImGuiNET;
using PizzaTime.Services;

namespace PizzaTime.Ui.MainWindow;

internal sealed partial class Tabs
{
    internal static void SettingsTab(DalamudService dalamudService)
    {
        var settings = dalamudService.GetSettings();
        var enableOrderStatusNotifications = settings.EnableOrderStatusNotifications;
        if (ImGuiComponents.ToggleButton("enableOrderStatusNotifications", ref enableOrderStatusNotifications))
        {
            settings.EnableOrderStatusNotifications = enableOrderStatusNotifications;
            dalamudService.SaveSettings(settings);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        ImGui.SameLine();
        ImGui.Text("Enable order status notifications");
    }
}
