using static PizzaTime.Ui.MainWindow.Tabs;

namespace PizzaTime.Ui.MainWindow;

internal sealed class MainWindowStore
{
    internal readonly OrderTabStore OrderTabStore = new();
}
