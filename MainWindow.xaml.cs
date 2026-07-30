using System.Windows;
using EpicBattle.Managers;
using EpicBattle.Views;

namespace EpicBattle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationManager.Initialize(MainContainer, this);
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
