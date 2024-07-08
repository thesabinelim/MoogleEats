using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using MoogleEats.Services;
using static MoogleEats.Ui.Components;
using static MoogleEats.Ui.MainWindow.Tabs;

namespace MoogleEats.Ui.MainWindow;

internal sealed class MainWindow : Window
{
    private readonly DalamudService dalamudService;
    private readonly DiscordService discordService;
    private readonly MainWindowStore store = new();

    internal MainWindow(DalamudService dalamudService, DiscordService discordService)
        : base("Moogle Eats", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.dalamudService = dalamudService;
        this.discordService = discordService;
    }

    public override void Draw()
    {
        Tabs("tabs", [
            new TabItem("Order", () => OrderTab(
                store: store.OrderTabStore,
                dalamudService: dalamudService,
                discordService: discordService
            )),
            new TabItem("Settings", () => SettingsTab(
                dalamudService: dalamudService
            ))
        ]);
    }
}
