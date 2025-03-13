using BatchExportShell.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchTools.ViewModels
{
    public class ExportBatchViewModel : BindableBase
    {

        #region Declarations
        private IEventAggregator _eventAggregator;
       
        public DelegateCommand ExportLocalCommand { get; set; }
        public DelegateCommand ExportLocalCommandNoCT { get; set; }


        private bool _anonymizeCB;

        public bool AnonymizeCB
        {
            get { return _anonymizeCB; }
            set { SetProperty(ref _anonymizeCB, value); }

        }


        private string _localAETitle = "Local AE Title";
        public string LocalAETitle
        {
            get { return _localAETitle; }
            set { SetProperty(ref _localAETitle, value); }     

            
        }
        private string _localIP = "Local IP";
        public string LocalIP
        {
            get { return _localIP; }
            set { SetProperty(ref _localIP, value); }

        }
        private string _localPort = "Local Port";
        public string LocalPort
        {
            get { return _localPort; }
            set { SetProperty(ref _localPort, value); }

        }
        private string _varianAETitle = "Varian AE Title";
        public string VarianAETitle
        {
            get { return _varianAETitle; }
            set { SetProperty(ref _varianAETitle, value); }

        }
        private string _varianIP = "Varian IP";
        public string VarianIP
        {
            get { return _varianIP; }
            set { SetProperty(ref _varianIP, value); }

        }
        private string _varianPort = "Varian Port";
        public string VarianPort
        {
            get { return _varianPort; }
            set { SetProperty(ref _varianPort, value); }

        }
        #endregion


        #region Constructor
        public ExportBatchViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        
            ExportLocalCommand = new DelegateCommand(ExportLocal);
            ExportLocalCommandNoCT = new DelegateCommand(ExportLocalNoCT);
      



        }
        #endregion

        #region Methods


        private void ExportLocal()
        {
            string args = _localAETitle + "," + _localIP + "," + _localPort + ",true," + Convert.ToString(AnonymizeCB) + "," + VarianAETitle + "," +
                 VarianIP + "," + VarianPort;
            using (var fbd = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _eventAggregator.GetEvent<ExportLocalEvent>().Publish(fbd.SelectedPath + "|" + args);

                }
            }

        }
        private void ExportLocalNoCT()
        {
            string args = _localAETitle + "," + _localIP + "," + _localPort + ",false," + Convert.ToString(AnonymizeCB) + "," + VarianAETitle + "," +
                 VarianIP + "," + VarianPort;
            using (var fbd = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _eventAggregator.GetEvent<ExportLocalEvent>().Publish(fbd.SelectedPath + "|" + args);

                }
            }
        }
        #endregion
    }
}
