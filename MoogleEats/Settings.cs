using Dalamud.Configuration;
using System;

namespace MoogleEats;

[Serializable]
public sealed class Settings : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool EnableOrderStatusNotifications { get; set; } = true;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
