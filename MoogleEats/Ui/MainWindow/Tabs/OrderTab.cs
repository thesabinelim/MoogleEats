using Dalamud.Interface;
using Dalamud.Interface.Colors;
using static MoogleEats.Ui.Components;
using System;

namespace MoogleEats.Ui.MainWindow
{
    public partial class Tabs
    {
        public static void OrderTab(OrderTabStore store, Action onCheckout)
        {
            NumericInput("Pizza (10,000g)", ref store.NPizzas);
            store.NPizzas = Math.Max(store.NPizzas, 0);

            NumericInput("Garlic Bread (5,000g)", ref store.NGarlicBread);
            store.NGarlicBread = Math.Max(store.NGarlicBread, 0);

            NumericInput("Bubble Tea (5,000g)", ref store.NBubbleTea);
            store.NBubbleTea = Math.Max(store.NBubbleTea, 0);

            NumericInput("Kid's Meal (20,000g)", ref store.NKidsMeal);
            store.NKidsMeal = Math.Max(store.NKidsMeal, 0);

            NumericInput("Ratatouille (Pumpkin) (20,000g)", ref store.NRatatouille);
            store.NRatatouille = Math.Max(store.NRatatouille, 0);

            NumericInput("Archon Burger (10,000g)", ref store.NArchonBurger);
            store.NArchonBurger = Math.Max(store.NArchonBurger, 0);

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
            public int NPizzas = 0;
            public int NGarlicBread = 0;
            public int NBubbleTea = 0;
            public int NKidsMeal = 0;
            public int NRatatouille = 0;
            public int NArchonBurger = 0;
            public string Notes = "";
            public bool IsCheckoutProcessing = false;
        }
    }
}
