using Dalamud.Interface;
using Dalamud.Interface.Colors;
using static MoogleEats.Ui.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using MoogleEats.Services;
using ImGuiNET;
using MoogleEats.Shared;
using System.Numerics;

namespace MoogleEats.Ui.MainWindow;

internal sealed partial class Tabs
{
    internal enum MenuItem
    {
        Pizza,
        GarlicBread,
        BubbleTea,
        KidsMeal,
        Ratatouille,
        ArchonBurger
    }

    internal static Dictionary<MenuItem, string> MenuItemNames = new() {
        { MenuItem.Pizza, "Pizza" },
        { MenuItem.GarlicBread, "Garlic Bread" },
        { MenuItem.BubbleTea, "Bubble Tea" },
        { MenuItem.KidsMeal, "Kid's Meal" },
        { MenuItem.Ratatouille, "Ratatouille" },
        { MenuItem.ArchonBurger, "Archon Burger" },
    };

    internal static Dictionary<MenuItem, uint> MenuItemPrices = new() {
        { MenuItem.Pizza, 10000 },
        { MenuItem.GarlicBread, 5000 },
        { MenuItem.BubbleTea, 5000 },
        { MenuItem.KidsMeal, 20000 },
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
            var location = dalamudService.GetPlayerLocation();
            if (name == null || !location.HasValue)
            {
                return null;
            }
            return $"Name: {name}\nLocation: {location}\nNotes: {store.Notes}\n\n{string.Join('\n', orderItemDescriptions)}\nTotal: {totalPrice:n0}g";
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
            { MenuItem.KidsMeal, 0 },
            { MenuItem.Ratatouille, 0 },
            { MenuItem.ArchonBurger, 0 },
        };
        internal string Notes = "";
        internal bool IsCheckoutProcessing = false;
    }
}
