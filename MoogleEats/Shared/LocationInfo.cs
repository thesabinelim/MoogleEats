using System.Numerics;
using System.Text;

namespace MoogleEats.Shared;

internal readonly record struct LocationInfo
{
    internal readonly Vector3 Coordinates { get; init; }
    internal readonly AreaInfo? Area { get; init; }
    internal readonly HousingInfo? Housing { get; init; }
    internal readonly string Zone { get; init; }
    internal readonly string Region { get; init; }
    internal readonly string World { get; init; }
    internal readonly string DataCenter { get; init; }
    internal readonly string DataCenterRegion { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Region);
        sb.Append($", {Zone}");
        if (Area.HasValue)
        {
            sb.Append($", {Area.Value}");
        } else if (Housing?.IsWardSubdivision == true)
        {
            sb.Append(" Subdivision");
        }
        if (Housing.HasValue)
        {
            sb.Append($", {Housing.Value}");
        }
        sb.Append($", ( X: {Coordinates.X} Y: {Coordinates.Y} Z: {Coordinates.Z} )");
        sb.Append($", {World}");
        sb.Append($", {DataCenter}");
        sb.Append($", {DataCenterRegion}");
        return sb.ToString();
    }
}

internal readonly record struct AreaInfo
{
    internal readonly string Name { get; init; }
    internal readonly string? SubArea { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Name);
        if (SubArea != null)
        {
            sb.Append($", {SubArea}");
        }
        return sb.ToString();
    }
}

internal readonly record struct HousingInfo
{
    internal readonly BuildingInfo? Building { get; init; }
    internal readonly uint Ward { get; init; }
    internal readonly bool IsWardSubdivision { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Building != null)
        {
            sb.Append($"{Building}, ");
        }
        sb.Append($"{StringUtils.ToOrdinalString((int)Ward)} Ward");
        return sb.ToString();
    }
}

internal interface BuildingInfo;

internal readonly record struct PlotInfo : BuildingInfo
{
    internal readonly uint Number { get; init; }
    internal readonly uint? Room { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Plot {Number}");
        if (Room.HasValue)
        {
            sb.Append($" Room #{Room.Value}");
        }
        return sb.ToString();
    }
}

internal readonly record struct ApartmentLobbyInfo : BuildingInfo
{
    public override string ToString()
    {
        return "Lobby";
    }
}

internal readonly record struct ApartmentRoomInfo : BuildingInfo
{
    internal readonly uint Number { get; init; }

    public override string ToString()
    {
        return $"Room #{Number}";
    }
}
