using System.Windows.Controls;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class SkillTreeView : UserControl
    {
        public SkillTreeView(System.Action closeAction)
        {
            InitializeComponent();
            BgImage.Source = EpicBattle.Services.ImageService.GetImage("Backgrounds/skill_tree");
            var vm = new SkillTreeViewModel();
            vm.CloseAction = closeAction;
            DataContext = vm;
        }
    }
}
