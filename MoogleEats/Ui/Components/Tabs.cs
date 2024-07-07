using ImGuiNET;
using System;

namespace MoogleEats.Ui
{
    public partial class Components
    {
        public static void Tabs(
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
        public record TabItem(string Label, Action RenderContent);
    }
}
