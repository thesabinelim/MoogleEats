using System;
using System.Collections.Generic;
using System.Numerics;

namespace MoogleEats.Shared;

internal readonly record struct LocationInfo
{
    internal readonly Vector3 Coordinates { get; init; }
    internal readonly AreaInfo? Area { get; init; }
    internal readonly HousingInfo? Housing { get; init; }
    internal readonly string? Map { get; init; }
    internal readonly string Zone { get; init; }
    internal readonly string Region { get; init; }

    internal readonly string CoordinateString
    {
        get
        {
            return $"(X: {formatCoordinate(Coordinates.X)}, Y: {formatCoordinate(Coordinates.Y)}, Z: {formatCoordinate(Coordinates.Z)})";

            static string formatCoordinate(float coord)
            {
                return $"{Math.Floor(coord * 10) / 10:0.0}";
            }
        }
    }

    internal readonly string AddressString
    {
        get
        {
            var parts = new List<string>();
            switch (Housing?.Building)
            {
                case PlotInfo plot:
                    parts.Add($"Plot {plot.Number}{(plot.Room.HasValue ? $" {formatRoom(plot.Room.Value)}" : "")}");
                    parts.Add(formatWard(Housing.Value.Ward));
                    parts.Add(Map ?? Zone);
                    break;
                case ApartmentLobbyInfo:
                    parts.Add($"{Map ?? Zone} {formatWing(Housing.Value.IsWardSubdivision)}");
                    parts.Add(formatWard(Housing.Value.Ward));
                    break;
                case ApartmentRoomInfo room:
                    parts.Add($"{Map ?? Zone} {formatWing(Housing.Value.IsWardSubdivision)} {formatRoom(room.Number)}");
                    parts.Add(formatWard(Housing.Value.Ward));
                    break;
                default:
                    if (Housing.HasValue && Map != null)
                    {
                        if (Area.HasValue)
                        {
                            parts.AddRange(formatArea(Area.Value));
                        }
                        parts.Add(Map);
                        parts.Add(Region);
                    }
                    else
                    {
                        if (Area.HasValue)
                        {
                            parts.AddRange(formatArea(Area.Value));
                            parts.Add(Zone);
                        }
                        else if (Map != null)
                        {
                            parts.Add(Map);
                            parts.Add(Zone);
                        }
                        else
                        {
                            parts.Add(Zone);
                            parts.Add(Region);
                        }
                    }
                    break;
            }
            return string.Join(", ", parts);

            static List<string> formatArea(AreaInfo area)
            {
                var parts = new List<string>();
                if (area.SubArea != null)
                {
                    parts.Add(area.SubArea);
                }
                parts.Add(area.Name);
                return parts;
            }

            static string formatWard(uint ward)
            {
                return $"{StringUtils.ToOrdinalString((int)ward)} Ward";
            }

            static string formatWing(bool? isSubdivision)
            {
                return $"Wing {(isSubdivision == true ? 2 : 1)}";
            }

            static string formatRoom(uint room)
            {
                return $"Room #{room}";
            }
        }
    }
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
