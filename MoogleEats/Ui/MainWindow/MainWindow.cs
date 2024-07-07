using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using static MoogleEats.Ui.Components;
using static MoogleEats.Ui.MainWindow.Tabs;

namespace MoogleEats.Ui.MainWindow;

public sealed class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly MainWindowStore store = new();

    public MainWindow(Plugin plugin)
        : base("Moogle Eats", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        Tabs("tabs", [
            new TabItem("Order", () => OrderTab(
                store: store.OrderTabStore,
                discordWebhookClient: plugin.DiscordWebhookClient
            )),
            new TabItem("Settings", () => SettingsTab(plugin.Settings))
        ]);
    }
}
