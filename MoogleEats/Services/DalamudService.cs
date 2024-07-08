using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using MoogleEats.Services.Injected;
using System;

namespace MoogleEats.Services;

internal sealed class DalamudService
{
    private IClientState clientState;
    private ICommandManager commandManager;
    private IDataManager dataManager;
    private IDalamudPluginInterface pluginInterface;

    public DalamudService(IDalamudPluginInterface pluginInterface)
    {
        var injected = pluginInterface.Create<DalamudInjectedServices>();
        if (injected == null
            || injected.ClientState == null
            || injected.CommandManager == null
            || injected.DataManager == null)
        {
            throw new Exception("Failed to inject Dalamud plugin Services");
        }

        this.pluginInterface = pluginInterface;
        clientState = injected.ClientState;
        commandManager = injected.CommandManager;
        dataManager = injected.DataManager;
    }

    internal Settings GetSettings()
    {
        return pluginInterface.GetPluginConfig() as Settings ?? new Settings();
    }

    internal void SaveSettings(Settings settings)
    {
        pluginInterface.SavePluginConfig(settings);
    }

    internal void OnOpenMainUi(Action handler)
    {
        pluginInterface.UiBuilder.OpenMainUi += handler;
    }

    internal void OnOpenConfigUi(Action handler)
    {
        pluginInterface.UiBuilder.OpenConfigUi += handler;
    }

    internal void RegisterCommand(string name, CommandInfo info)
    {
        commandManager.AddHandler(name, info);
    }

    internal void RemoveCommand(string name)
    {
        commandManager.RemoveHandler(name);
    }

    internal void OnDraw(Action handler)
    {
        pluginInterface.UiBuilder.Draw += handler;
    }
}
