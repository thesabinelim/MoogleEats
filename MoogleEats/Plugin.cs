using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MoogleEats.Ui.MainWindow;
using Discord.Webhook;

namespace MoogleEats;

public sealed class Plugin : IDalamudPlugin
{

    private const string CommandName = "/moogleeats";
    private const string DiscordWebhookUrl = "https://discord.com/api/webhooks/1201449051053367378/kBiuMzKDndSK1qij3LtnO3B3IewrsVv_3NqIcQctQo3fGMLumc2z20JvA7XCNRB7R5WY";

    public Settings Settings { get; init; }
    public readonly WindowSystem WindowSystem = new("MoogleEats");
    private MainWindow MainWindow { get; init; }
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    public DiscordWebhookClient DiscordWebhookClient = new DiscordWebhookClient(DiscordWebhookUrl);

    public Plugin()
    {
        Settings = PluginInterface.GetPluginConfig() as Settings ?? new Settings();

        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the Moogle Eats menu and order some food!"
        });

        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        PluginInterface.UiBuilder.OpenMainUi += OpenMainWindow;
        PluginInterface.UiBuilder.OpenConfigUi += OpenSettingsWindow;
    }

    public void OpenMainWindow()
    {
        MainWindow.IsOpen = true;
    }

    public void OpenSettingsWindow()
    {
        OpenMainWindow();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }
}
