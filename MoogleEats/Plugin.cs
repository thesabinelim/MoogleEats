using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MoogleEats.Ui.MainWindow;

namespace MoogleEats;

public sealed class Plugin : IDalamudPlugin
{
    public Settings Settings { get; init; }

    public readonly WindowSystem WindowSystem = new("MoogleEats");

    private const string CommandName = "/moogleeats";

    private MainWindow MainWindow { get; init; }

    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

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
