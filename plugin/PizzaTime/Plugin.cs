using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using PizzaTime.Ui.MainWindow;
using PizzaTime.Services;

namespace PizzaTime;

internal sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/pizzatime";
    private const string DiscordWebhookUrl = "https://discord.com/api/webhooks/1201449051053367378/kBiuMzKDndSK1qij3LtnO3B3IewrsVv_3NqIcQctQo3fGMLumc2z20JvA7XCNRB7R5WY";

    private readonly DalamudService dalamudService;
    private readonly WindowSystem windowSystem = new("PizzaTime");
    private readonly DiscordService discordService = new(DiscordWebhookUrl);

    private readonly MainWindow mainWindow;

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        dalamudService = new(pluginInterface);

        mainWindow = new(
            dalamudService: dalamudService,
            discordService: discordService
        );
        windowSystem.AddWindow(mainWindow);

        dalamudService.OnOpenMainUi(openMainWindow);
        dalamudService.OnOpenConfigUi(openSettingsWindow);
        dalamudService.OnDraw(onDraw);
        dalamudService.RegisterCommand(CommandName, new CommandInfo(onCommand)
        {
            HelpMessage = "Open the Pizza Time menu and order some food!"
        });
    }

    public void Dispose()
    {
        windowSystem.RemoveAllWindows();

        dalamudService.RemoveCommand(CommandName);
    }

    private void openMainWindow()
    {
        mainWindow.IsOpen = true;
    }

    private void openSettingsWindow()
    {
        openMainWindow();
    }

    private void onCommand(string command, string args)
    {
        mainWindow.Toggle();
    }

    private void onDraw()
    {
        windowSystem.Draw();
    }
}
