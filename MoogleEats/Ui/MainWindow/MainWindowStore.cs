using static MoogleEats.Ui.MainWindow.Tabs;

namespace MoogleEats.Ui.MainWindow;

internal sealed class MainWindowStore
{
    internal readonly OrderTabStore OrderTabStore = new();
}
