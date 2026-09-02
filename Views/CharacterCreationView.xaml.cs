using System.Windows.Controls;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class CharacterCreationView : UserControl
    {
        public CharacterCreationView(bool isArcade, bool isMapMode = false)
        {
            InitializeComponent();
            DataContext = new CharacterCreationViewModel(isArcade, isMapMode);
        }
    }
}
