using System.Numerics;

namespace MoogleEats.Shared;

internal readonly record struct LocationInfo
{
    internal readonly Vector2 Coordinates { get; init; }
    internal readonly AreaInfo? Area { get; init; }
    internal readonly HousingInfo? Housing { get; init; }
    internal readonly string Zone { get; init; }
    internal readonly string Region { get; init; }
    internal readonly string World { get; init; }
    internal readonly string DataCenter { get; init; }
    internal readonly string DataCenterRegion { get; init; }
}

internal readonly record struct AreaInfo
{
    internal readonly string Name { get; init; }
    internal readonly string? SubArea { get; init; }
}

internal readonly record struct HousingInfo
{
    internal readonly BuildingInfo? Building { get; init; }
    internal readonly uint Ward { get; init; }
    internal readonly bool IsWardSubdivision { get; init; }
}

internal interface BuildingInfo;

internal readonly record struct PlotInfo : BuildingInfo
{
    internal readonly uint Number { get; init; }
    internal readonly uint? Room { get; init; }
}

internal readonly record struct ApartmentLobbyInfo : BuildingInfo;

internal readonly record struct ApartmentRoomInfo : BuildingInfo
{
    internal readonly uint Number { get; init; }
}
