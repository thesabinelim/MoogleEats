using ImGuiNET;
using System;

namespace PizzaTime.Ui;

internal sealed partial class Components
{
    internal static void Tabs(
        string id,
        TabItem[] items,
        float rounding = 5f
    )
    {
        if (ImGui.BeginTabBar(id))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, rounding);

            foreach (var item in items)
            {
                var isSelected = ImGui.BeginTabItem(item.Label);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                if (isSelected)
                {
                    item.RenderContent();
                    ImGui.EndTabItem();
                }
            }

            ImGui.PopStyleVar();

            ImGui.EndTabBar();
        }
    }

    internal record TabItem(string Label, Action RenderContent);
}
