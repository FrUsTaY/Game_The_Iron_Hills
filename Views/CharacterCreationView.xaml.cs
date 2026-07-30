using System.Windows.Controls;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class CharacterCreationView : UserControl
    {
        public CharacterCreationView(bool isArcade)
        {
            InitializeComponent();
            DataContext = new CharacterCreationViewModel(isArcade);
        }
    }
}
