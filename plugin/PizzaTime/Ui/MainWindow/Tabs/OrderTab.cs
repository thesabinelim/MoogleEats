using Dalamud.Interface;
using Dalamud.Interface.Colors;
using static PizzaTime.Ui.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using PizzaTime.Services;
using ImGuiNET;
using Dalamud.Game;
using System.Numerics;

namespace PizzaTime.Ui.MainWindow;

internal sealed partial class Tabs
{
    internal enum MenuItem
    {
        Pizza,
        GarlicBread,
        BubbleTea,
        KidsMealCombo,
        Ratatouille,
        ArchonBurger
    }

    internal static Dictionary<MenuItem, string> MenuItemNames = new() {
        { MenuItem.Pizza, "Pizza" },
        { MenuItem.GarlicBread, "Garlic Bread" },
        { MenuItem.BubbleTea, "Bubble Tea" },
        { MenuItem.KidsMealCombo, "Kid's Meal Combo" },
        { MenuItem.Ratatouille, "Ratatouille" },
        { MenuItem.ArchonBurger, "Archon Burger" },
    };

    internal static Dictionary<MenuItem, string> MenuItemDescriptions = new() {
        { MenuItem.Pizza, "A simple-but-satisfying dish of thinly worked dough topped with tomatoes and cheese, baked in a wood-fired oven until crispy." },
        { MenuItem.GarlicBread, "It's actually Flatbread." },
        { MenuItem.BubbleTea, "It's actually Tsai tou Vounou." },
        { MenuItem.KidsMealCombo, "Includes Pizza + Bubble Tea + Garlic Bread + toy" },
        { MenuItem.Ratatouille, "This colorful dish consists of a variety of vegetables, sautéed in perilla oil and then simmered to enrich their combined flavor." },
        { MenuItem.ArchonBurger, "Concocted by the staff at the Last Stand, this sandwich is as nutritionally sound as it is delicious." },
    };

    internal static Dictionary<MenuItem, ushort> MenuItemIcons = new() {
        { MenuItem.Pizza, 24034 },
        { MenuItem.GarlicBread, 24006 },
        { MenuItem.BubbleTea, 24417 },
        { MenuItem.KidsMealCombo, 40110 },
        { MenuItem.Ratatouille, 24108 },
        { MenuItem.ArchonBurger, 24039 },
    };

    internal static Dictionary<MenuItem, uint> MenuItemPrices = new() {
        { MenuItem.Pizza, 10000 },
        { MenuItem.GarlicBread, 5000 },
        { MenuItem.BubbleTea, 5000 },
        { MenuItem.KidsMealCombo, 20000 },
        { MenuItem.Ratatouille, 20000 },
        { MenuItem.ArchonBurger, 10000 },
    };

    internal static void OrderTab(
        OrderTabStore store,
        DalamudService dalamudService,
        DiscordService discordService
    )
    {
        string? constructMessage(string[] orderItemDescriptions, uint totalPrice)
        {
            var name = dalamudService.GetQualifiedPlayerName();
            var location = dalamudService.GetPlayerLocation(ClientLanguage.English);
            var world = dalamudService.GetPlayerWorldName(ClientLanguage.English);
            var dataCenter = dalamudService.GetPlayerDataCenterName(ClientLanguage.English);
            var dataCenterRegionShort = dalamudService.GetDataCenterRegionShortName();
            if (name == null
                || !location.HasValue
                || world == null
                || dataCenter == null
                || dataCenterRegionShort == null)
            {
                return null;
            }
            var parts = new List<string> {
                $"Name: {name}",
                $"Location: {location.Value.AddressString} {location.Value.CoordinateString}",
                $"World: {world}, {dataCenter} {dataCenterRegionShort}",
                $"Notes: {store.Notes}",
                "",
                string.Join('\n', orderItemDescriptions),
                $"Total: {totalPrice:n0}g",
            };
            return string.Join('\n', parts);
        }

        uint totalPrice = 0;
        string[] orderItemDescriptions = [];
        foreach (MenuItem menuItem in Enum.GetValues(typeof(MenuItem)))
        {
            var count = store.Counts[menuItem];
            if (count > 0)
            {
                var name = MenuItemNames[menuItem];
                var price = MenuItemPrices[menuItem];
                var subtotalPrice = (uint)(price * count);
                orderItemDescriptions = orderItemDescriptions.Append(count > 1
                    ? $"{count} {name}s (* {price:n0}g = {subtotalPrice:n0}g)"
                    : $"1 {name} ({price:n0}g)"
                ).ToArray();
                totalPrice += subtotalPrice;
            }
        }

        ImGui.TextUnformatted(constructMessage(orderItemDescriptions, totalPrice));

        async void onCheckout()
        {
            var message = constructMessage(orderItemDescriptions, totalPrice);
            if (message == null)
            {
                throw new Exception("TODO: handle this");
            }

            store.IsCheckoutProcessing = true;
            await discordService.SendWebhookMessage(message);
            store.IsCheckoutProcessing = false;
        }

        foreach (MenuItem menuItem in Enum.GetValues(typeof(MenuItem)))
        {
            var count = store.Counts[menuItem];
            var icon = dalamudService.GetIconImguiHandle(MenuItemIcons[menuItem]);
            if (icon.HasValue)
            {
                ImGui.Image(icon.Value, new Vector2(64, 64));
                ImGui.SameLine();
            }
            NumericInput($"{MenuItemNames[menuItem]} (${MenuItemPrices[menuItem]:n0}g)", ref count);
            count = Math.Max(count, 0);
            store.Counts[menuItem] = count;
        }

        TextInput("Notes (optional)", ref store.Notes, 256);

        Button(
            label: !store.IsCheckoutProcessing
                ? "Place order"
                : "Processing...",
            onPress: onCheckout,
            icon: !store.IsCheckoutProcessing
                ? FontAwesomeIcon.Truck
                : FontAwesomeIcon.Spinner,
            color: ImGuiColors.HealerGreen,
            disabled: store.IsCheckoutProcessing
        );
    }

    internal sealed class OrderTabStore
    {
        internal Dictionary<MenuItem, int> Counts = new()
        {
            { MenuItem.Pizza, 0 },
            { MenuItem.GarlicBread, 0 },
            { MenuItem.BubbleTea, 0 },
            { MenuItem.KidsMealCombo, 0 },
            { MenuItem.Ratatouille, 0 },
            { MenuItem.ArchonBurger, 0 },
        };
        internal string Notes = "";
        internal bool IsCheckoutProcessing = false;
    }
}
