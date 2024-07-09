using System;
using Game = FFXIVClientStructs.FFXIV.Client.Game;

namespace MoogleEats.Services.Injected;

internal sealed unsafe class ClientStructServices
{
    internal readonly Game.HousingManager HousingManager;
    internal readonly Game.UI.TerritoryInfo TerritoryInfo;

    internal ClientStructServices()
    {
        var housingManagerPtr = Game.HousingManager.Instance();
        var territoryInfoPtr = Game.UI.TerritoryInfo.Instance();
        if (housingManagerPtr == null || territoryInfoPtr == null)
        {
            throw new Exception("Failed to initialise ClientStruct services");
        }
        HousingManager = *housingManagerPtr;
        TerritoryInfo = *territoryInfoPtr;
    }
}
