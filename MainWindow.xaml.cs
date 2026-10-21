using System.Windows;
using EpicBattle.Managers;
using EpicBattle.Services;
using EpicBattle.Views;

namespace EpicBattle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var icon = ImageService.GetUiImage("window_icon");
            if (icon != null) this.Icon = icon;
            NavigationManager.Initialize(MainContainer, this);
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
