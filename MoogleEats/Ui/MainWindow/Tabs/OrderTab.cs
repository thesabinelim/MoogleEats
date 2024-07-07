using Dalamud.Interface;
using Dalamud.Interface.Colors;
using static MoogleEats.Ui.Components;
using System;
using Discord.Webhook;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;

namespace MoogleEats.Ui.MainWindow
{
    public partial class Tabs
    {
        public enum MenuItem
        {
            Pizza,
            GarlicBread,
            BubbleTea,
            KidsMeal,
            Ratatouille,
            ArchonBurger
        }

        public static Dictionary<MenuItem, string> MenuItemNames = new() {
            { MenuItem.Pizza, "Pizza" },
            { MenuItem.GarlicBread, "Garlic Bread" },
            { MenuItem.BubbleTea, "Bubble Tea" },
            { MenuItem.KidsMeal, "Kid's Meal" },
            { MenuItem.Ratatouille, "Ratatouille" },
            { MenuItem.ArchonBurger, "Archon Burger" },
        };

        public static Dictionary<MenuItem, uint> MenuItemPrices = new() {
            { MenuItem.Pizza, 10000 },
            { MenuItem.GarlicBread, 5000 },
            { MenuItem.BubbleTea, 5000 },
            { MenuItem.KidsMeal, 20000 },
            { MenuItem.Ratatouille, 20000 },
            { MenuItem.ArchonBurger, 10000 },
        };

        public static void OrderTab(OrderTabStore store, DiscordWebhookClient discordWebhookClient)
        {

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
            var message = $"Name: @catharsis_\nWorld: Is His Oyster\nLocation: In High Places (Just Like His Friends)\nNotes: {store.Notes}\n\n{string.Join('\n', orderItemDescriptions)}\nTotal: {totalPrice:n0}g";

            Action onCheckout = async () =>
            {
                store.IsCheckoutProcessing = true;
                await discordWebhookClient.SendMessageAsync(message);

                store.IsCheckoutProcessing = false;
            };

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

        public sealed class OrderTabStore
        {
            public Dictionary<MenuItem, int> Counts = new()
            {
                { MenuItem.Pizza, 0 },
                { MenuItem.GarlicBread, 0 },
                { MenuItem.BubbleTea, 0 },
                { MenuItem.KidsMeal, 0 },
                { MenuItem.Ratatouille, 0 },
                { MenuItem.ArchonBurger, 0 },
            };
            public string Notes = "";
            public bool IsCheckoutProcessing = false;
        }
    }
}
