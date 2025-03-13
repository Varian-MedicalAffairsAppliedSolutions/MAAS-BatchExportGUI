using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace BatchExportShell.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;


        public DelegateCommand<string> NavigateCommand { get; set; }
        public MainWindowViewModel(IRegionManager regionManager)
        {
        
            NavigateCommand = new DelegateCommand<string>(Navigate);
            _regionManager = regionManager;


        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("ContentRegion", uri);
        }


    }
}
