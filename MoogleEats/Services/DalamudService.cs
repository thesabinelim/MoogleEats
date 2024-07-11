using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using PlaceName = Lumina.Excel.GeneratedSheets.PlaceName;
using Map = Lumina.Excel.GeneratedSheets.Map;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;
using TerritoryTypeTransient = Lumina.Excel.GeneratedSheets.TerritoryTypeTransient;
using World = Lumina.Excel.GeneratedSheets.World;
using MoogleEats.Services.Injected;
using System;
using Dalamud.Game;
using System.Numerics;
using MoogleEats.Shared;

namespace MoogleEats.Services;

internal sealed class DalamudService
{
    private readonly IClientState clientState;
    private readonly ICommandManager commandManager;
    private readonly IDataManager dataManager;
    private readonly IDalamudPluginInterface pluginInterface;

    public DalamudService(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;

        var injected = pluginInterface.Create<DalamudInjectedServices>();
        if (injected == null
            || injected.ClientState == null
            || injected.CommandManager == null
            || injected.DataManager == null)
        {
            throw new Exception("Failed to inject Dalamud plugin services");
        }
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

    internal ClientLanguage GetLanguage()
    {
        return clientState.ClientLanguage;
    }

    internal string? GetPlayerName()
    {
        return getPlayer()?.Name.ToString();
    }

    internal string? GetQualifiedPlayerName()
    {
        return GetQualifiedPlayerName(GetLanguage());
    }

    internal string? GetQualifiedPlayerName(ClientLanguage language)
    {
        var name = GetPlayerName();
        var homeWorld = getPlayerHomeWorldName(language);
        return name != null && homeWorld != null ? $"{name}@{homeWorld}" : null;
    }

    internal LocationInfo? GetPlayerLocation()
    {
        return GetPlayerLocation(GetLanguage());
    }

    internal LocationInfo? GetPlayerLocation(ClientLanguage language)
    {
        var coords = getPlayerMapCoordinates();
        var zone = getZoneName(language);
        var region = getRegionName(language);
        var world = getWorldName(language);
        var dataCenter = getDataCenterName(language);
        var dataCenterRegion = getDataCenterRegionName();
        if (!coords.HasValue
            || zone == null
            || region == null
            || world == null
            || dataCenter == null
            || dataCenterRegion == null)
        {
            return null;
        }

        return new LocationInfo() {
            Coordinates = coords.Value,
            Area = getPlayerAreaInfo(language),
            Housing = getPlayerHousingInfo(),
            Zone = zone,
            Region = region,
            World = world,
            DataCenter = dataCenter,
            DataCenterRegion = dataCenterRegion,
        };
    }

    private AreaInfo? getPlayerAreaInfo(ClientLanguage language)
    {
        var area = getAreaName(language);
        var subArea = getSubAreaName(language);
        return area?.Length > 0 ? new AreaInfo()
        {
            Name = area,
            SubArea = subArea?.Length > 0 ? subArea : null,
        } : null;
    }

    private static HousingInfo? getPlayerHousingInfo()
    {
        var ward = getHousingWard();
        if (!ward.HasValue)
        {
            return null;
        }

        return new HousingInfo()
        {
            Building = getPlayerBuildingInfo(),
            Ward = ward.Value,
            IsWardSubdivision = isHousingWardSubdivision(),
        };
    }

    private static BuildingInfo? getPlayerBuildingInfo()
    {
        var room = getHousingRoom();
        if (isHousingApartment() == true)
        {
            return room.HasValue
                ? new ApartmentRoomInfo()
                    {
                        Number = room.Value,
                    }
                : new ApartmentLobbyInfo();
        }

        var plot = getHousingPlot();
        return plot.HasValue
            ? new PlotInfo()
            {
                Number = plot.Value,
                Room = room,
            }
            : null;
    }

    /**
     * https://github.com/goatcorp/Dalamud/issues/1916
     * Can't use MapUtil.GetMapCoordinates() until this is fixed
     */
    private Vector3? getPlayerMapCoordinates()
    {
        var player = getPlayer();
        var map = getMap();
        var territoryTransient = getTerritoryTransient();
        if (player == null || map == null || territoryTransient == null)
        {
            return null;
        }
        return MapUtil.WorldToMap(player.Position, map, territoryTransient, true);
    }

    private string? getPlayerHomeWorldName(ClientLanguage language)
    {
        return getPlayerHomeWorld(language)?.Name.RawString;
    }

    private string? getRegionName(ClientLanguage language)
    {
        return getTerritory(language)?.PlaceNameRegion.Value?.Name.RawString;
    }

    private string? getZoneName(ClientLanguage language)
    {
        return getTerritory(language)?.PlaceName.Value?.Name.RawString;
    }

    private string? getAreaName(ClientLanguage language)
    {
        var areaPlaceNameId = ClientStructServices.TerritoryInfo?.AreaPlaceNameId;
        return areaPlaceNameId.HasValue
            ? dataManager.GetExcelSheet<PlaceName>(language)?.GetRow(areaPlaceNameId.Value)?.Name.RawString
            : null;
    }

    private string? getSubAreaName(ClientLanguage language)
    {
        var subAreaPlaceNameId = ClientStructServices.TerritoryInfo?.SubAreaPlaceNameId;
        return subAreaPlaceNameId.HasValue
            ? dataManager.GetExcelSheet<PlaceName>(language)?.GetRow(subAreaPlaceNameId.Value)?.Name.RawString
            : null;
    }

    private string? getWorldName(ClientLanguage language)
    {
        return getWorld(language)?.Name.RawString;
    }

    private string? getDataCenterName(ClientLanguage language)
    {
        return getWorld(language)?.DataCenter.Value?.Name.RawString;
    }

    private string? getDataCenterRegionName()
    {
        return getWorld()?.DataCenter.Value?.Region switch
        {
            1 => "Japan",
            2 => "North America",
            3 => "Europe",
            4 => "Australia",
            null => null,
            _ => "Unknown",
        };
    }

    private static uint? getHousingRoom()
    {
        var room = ClientStructServices.HousingManager?.GetCurrentRoom();
        return room > 0 ? (uint)room : null;
    }

    private static uint? getHousingPlot()
    {
        var plot = ClientStructServices.HousingManager?.GetCurrentPlot() + 1;
        return plot > 0 ? (uint)plot : null;
    }

    private static bool? isHousingApartment()
    {
        return ClientStructServices.HousingManager?.GetCurrentPlot() < -1;
    }

    private static uint? getHousingWard()
    {
        var ward = ClientStructServices.HousingManager?.GetCurrentWard() + 1;
        return ward > 0 ? (uint)ward : null;
    }

    private static bool isHousingWardSubdivision()
    {
        return ClientStructServices.HousingManager?.GetCurrentDivision() == 2
            || ClientStructServices.HousingManager?.GetCurrentPlot() is >= 30 or -127;
    }

    private IPlayerCharacter? getPlayer()
    {
        return clientState.LocalPlayer;
    }

    private World? getPlayerHomeWorld()
    {
        return getPlayerHomeWorld(GetLanguage());
    }

    private World? getPlayerHomeWorld(ClientLanguage language)
    {
        return getPlayer()?.HomeWorld.GetWithLanguage(language);
    }

    private World? getWorld()
    {
        return getWorld(GetLanguage());
    }

    private World? getWorld(ClientLanguage language)
    {
        return getPlayer()?.CurrentWorld.GetWithLanguage(language);
    }

    private TerritoryType? getTerritory()
    {
        return getTerritory(GetLanguage());
    }

    private TerritoryType? getTerritory(ClientLanguage language)
    {
        return dataManager.GetExcelSheet<TerritoryType>(language)?.GetRow(clientState.TerritoryType);
    }

    private TerritoryTypeTransient? getTerritoryTransient()
    {
        return getTerritoryTransient(GetLanguage());
    }

    private TerritoryTypeTransient? getTerritoryTransient(ClientLanguage language)
    {
        return dataManager.GetExcelSheet<TerritoryTypeTransient>(language)?.GetRow(clientState.TerritoryType);
    }

    private Map? getMap()
    {
        return getMap(GetLanguage());
    }

    private Map? getMap(ClientLanguage language)
    {
        return dataManager.GetExcelSheet<Map>(language)?.GetRow(clientState.MapId);
    }
}
