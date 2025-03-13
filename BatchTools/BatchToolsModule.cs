using BatchTools.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchTools
{
    public class BatchToolsModule : IModule
    {
        private readonly IRegionManager _regionManager;
       



        public BatchToolsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;

        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // view injection 
     
            IRegion region = _regionManager.Regions["ContentRegion"];

            var bbv = containerProvider.Resolve<BuildBatchView>();
            var cbv = containerProvider.Resolve<CurrentBatchView>();
            var ebv = containerProvider.Resolve<ExportBatchView>();
            region.Add(bbv);
            region.Add(cbv);
            region.Add(ebv);


   

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // registers the views with the navigator
            containerRegistry.RegisterForNavigation<BuildBatchView>();
            containerRegistry.RegisterForNavigation<CurrentBatchView>();
            containerRegistry.RegisterForNavigation<ExportBatchView>();

        }

    }
}
