using System.Windows.Controls;
using EpicBattle.Services;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class CharacterCreationView : UserControl
    {
        public CharacterCreationView(bool isArcade, bool isMapMode = false)
        {
            InitializeComponent();
            BgImage.Source = ImageService.GetImage("Backgrounds/char_creation");
            DataContext = new CharacterCreationViewModel(isArcade, isMapMode);
        }
    }
}
