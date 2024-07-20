using Client = FFXIVClientStructs.FFXIV.Client;

namespace PizzaTime.Services.Injected;

internal sealed unsafe class ClientStructServices
{
    internal static Client.UI.Agent.AgentMap? AgentMap
    {
        get
        {
            var agentMap = Client.UI.Agent.AgentMap.Instance();
            return agentMap != null ? *agentMap : null;
        }
    }

    internal static Client.Game.HousingManager? HousingManager
    {
        get
        {
            var housingManager = Client.Game.HousingManager.Instance();
            return housingManager != null ? *housingManager : null;
        }
    }

    internal static Client.Game.UI.TerritoryInfo? TerritoryInfo
    {
        get
        {
            var territoryInfo = Client.Game.UI.TerritoryInfo.Instance();
            return territoryInfo != null ? *territoryInfo : null;
        }
    }
}
