using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using PlaceName = Lumina.Excel.GeneratedSheets.PlaceName;
using Map = Lumina.Excel.GeneratedSheets.Map;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;
using TerritoryTypeTransient = Lumina.Excel.GeneratedSheets.TerritoryTypeTransient;
using World = Lumina.Excel.GeneratedSheets.World;
using PizzaTime.Services.Injected;
using System;
using Dalamud.Game;
using System.Numerics;
using PizzaTime.Shared;

namespace PizzaTime.Services;

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
        var name = GetPlayerName();
        var homeWorld = getPlayerHomeWorldName();
        return name != null && homeWorld != null ? $"{name}@{homeWorld}" : null;
    }

    internal LocationInfo? GetPlayerLocation()
    {
        return GetPlayerLocation(GetLanguage());
    }

    internal LocationInfo? GetPlayerLocation(ClientLanguage language)
    {
        var coords = getPlayerMapCoordinates();
        var map = getMapName(language);
        var zone = getZoneName(language);
        var region = getRegionName(language);
        if (!coords.HasValue
            || zone == null
            || region == null)
        {
            return null;
        }

        return new LocationInfo()
        {
            Coordinates = coords.Value,
            Area = getPlayerAreaInfo(language),
            Housing = getPlayerHousingInfo(),
            Map = map,
            Zone = zone,
            Region = region,
        };
    }

    internal string? GetPlayerWorldName(ClientLanguage language)
    {
        return getWorld(language)?.Name.RawString;
    }

    internal string? GetPlayerDataCenterName(ClientLanguage language)
    {
        return getWorld(language)?.DataCenter.Value?.Name.RawString;
    }

    internal string? GetDataCenterRegionName()
    {
        return getWorld()?.DataCenter.Value?.Region switch
        {
            1 => "Japan",
            2 => "North America",
            3 => "Europe",
            4 => "Oceania",
            null => null,
            _ => "Unknown",
        };
    }

    internal string? GetDataCenterRegionShortName()
    {
        return getWorld()?.DataCenter.Value?.Region switch
        {
            1 => "JP",
            2 => "NA",
            3 => "EU",
            4 => "OCE",
            null => null,
            _ => "???",
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

    private Vector3? getPlayerMapCoordinates()
    {
        return getPlayer()?.GetMapCoordinates(true);
    }

    private string? getPlayerHomeWorldName()
    {
        return getPlayerHomeWorld()?.Name.RawString;
    }

    private string? getRegionName(ClientLanguage language)
    {
        return getTerritory(language)?.PlaceNameRegion.Value?.Name.RawString;
    }

    private string? getZoneName(ClientLanguage language)
    {
        return getTerritory(language)?.PlaceName.Value?.Name.RawString;
    }

    private string? getMapName(ClientLanguage language)
    {
        var map = getMap(language)?.PlaceNameSub.Value?.Name.RawString;
        return map?.Length > 0 ? map : null;
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
