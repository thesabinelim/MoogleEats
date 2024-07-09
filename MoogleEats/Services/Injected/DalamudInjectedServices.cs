using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace MoogleEats.Services.Injected;

internal sealed class DalamudInjectedServices
{
    [PluginService]
    internal IClientState? ClientState { get; set; }
    [PluginService]
    internal ICommandManager? CommandManager { get; set; }
    [PluginService]
    internal IDataManager? DataManager { get; set; }
}
