using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchExportShell.Core.Events;
using System.Windows.Controls;
using System.Windows.Input;
using BatchTools.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.IO;
using System.Text.Json;
using Microsoft.Windows.Themes;
using System.Windows;
using BatchTools.Events;
using Squirrel;
using System.Diagnostics;

namespace BatchTools.ViewModels
{
    public class BuildBatchViewModel : BindableBase
    {
        #region Declarations
        //UpdateManager _squirrelUpdateManager;
        private bool _backendReady = true;
        public bool BackendReady
        {
            get { return _backendReady; }
            set { SetProperty(ref _backendReady, value); }
        }

        private string _importButtonText = "Import";
        public string ImportButtonText
        {
            get { return _importButtonText; }
            set { SetProperty(ref _importButtonText, value); }
        }
        private ObservableCollection<object> _selectedItem = new ObservableCollection<object>();

        public ObservableCollection<object> SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public object _treeViewSelectionChanged { get; set; }
       



        // collections declaration
        private ObservableCollection<Patient> _currentResults = new ObservableCollection<Patient>();
        public ObservableCollection<Patient> CurrentResults
        {
            get { return _currentResults; }
            set { SetProperty(ref _currentResults, value); }
        }

        private IEventAggregator _eventAggregator;



        #endregion
        #region Command Declarations

        public DelegateCommand<object> ImportQueryCommand { get;  set; }   
        public DelegateCommand<object> StackPanelClickCommand { get; private set; }
        public DelegateCommand ResetCurrentResultsCommand { get; private set; }
        public DelegateCommand AddSelectionToBatchCommand { get; private set; }
        #endregion
        #region Constructor

        public BuildBatchViewModel(IEventAggregator eventAggregator)
        {
            ImportQueryCommand = new DelegateCommand<object>(ImportQueryAsync).ObservesCanExecute(()=>BackendReady);
            StackPanelClickCommand = new DelegateCommand<object>(StackPanelClick);
            ResetCurrentResultsCommand = new DelegateCommand(ResetCurrentResults);
            AddSelectionToBatchCommand = new DelegateCommand(AddSelectionToBatch);

            _eventAggregator = eventAggregator;
            //CheckForUpdates();

        


 

        }

        //private async void CheckForUpdates()
        //{
        //    _squirrelUpdateManager = await UpdateManager.GitHubUpdateManager("https://github.com/lcmoore/BatchExport");
        //    var updateInfo = await _squirrelUpdateManager.CheckForUpdate();
        //    if (updateInfo.ReleasesToApply.Any())
        //    {
        //        var release = updateInfo.FutureReleaseEntry;
        //        var version = release.Version.ToString();
        //        var message = $"Version {version} is available. Would you like to update?";
        //        var result = MessageBox.Show(message, "Update Available", MessageBoxButton.YesNo);
        //        if (result == MessageBoxResult.Yes)
        //        {
        //            await _squirrelUpdateManager.DownloadReleases(updateInfo.ReleasesToApply);
        //            await _squirrelUpdateManager.ApplyReleases(updateInfo);
        //            await _squirrelUpdateManager.UpdateApp();
        //            MessageBox.Show("Update Complete. Please restart the application.");
        //            Application.Current.Shutdown();
        //        }
        //    }
           
        //}
        #endregion
        #region Methods

        private void AddSelectionToBatch()
        {        
            _eventAggregator.GetEvent<AddBatchItem>().Publish(SelectedItem.FirstOrDefault());

        }

        private void ResetCurrentResults()
        {
            CurrentResults.Clear();
        }



        private void StackPanelClick(object obj)
        {

            var args = (MouseButtonEventArgs)obj;
            var tv = (TreeView)args.Source;
            SelectedItem.Clear();
            SelectedItem.Add(tv.SelectedItem);


        }




        private void ImportQuery()
        {

            // if the output file already exists, delete it
            if (File.Exists(@"PythonScripts/tmp/output.json"))
            {
                File.Delete(@"PythonScripts/tmp/output.json");
            }
            Dictionary<string, Dictionary<string, List<string>>> request = new Dictionary<string, Dictionary<string, List<string>>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {

                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    foreach (string line in lines)
                    {
                        string course = null;
                        string plan = null;
           

                        string[] columns = line.Split(',');
                

                        string id = columns.ElementAt(0);
                        // add leading 0 if necessary, since csv removes leading 0s
                        if (columns.ElementAt(0).Length == 7)
                        {
                            id = "0" + columns.ElementAt(0);
                        }
                        // check if columns length is > 1
                        if (columns.Length > 1)
                        {
                            course = columns.ElementAt(1);
                        }
    
                        // check if columns length is > 2
                        if (columns.Length > 2)
                        {
                            plan = columns.ElementAt(2);
                        }
                       // if id not in requests keys, create a new object
                       if (!request.ContainsKey(id))
                        {
                            request.Add(id, new Dictionary<string, List<string>>());
                        }
                        // if course not in requests[id] keys, create a new object
                        if (course != null && !request[id].ContainsKey(course))
                        {
                            request[id].Add(course, new List<string>());
                        }
                        // if plan not in requests[id][course] keys, add it
                        if (course != null && plan != null && !request[id][course].Contains(plan))
                        {
                            request[id][course].Add(plan);
                        }   
  


                   

                       
    
                   

                    }
                    List<Patient> patient_requests = new List<Patient>();
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> entry in request)
                    {
                        Patient thisPatient = new Patient(entry.Key, entry.Value);
                        patient_requests.Add(thisPatient);


                        
                    }
                    // convert request dictionary to json
                    string json = JsonSerializer.Serialize(patient_requests);
                    // write json to file
                    File.WriteAllText(@"PythonScripts/tmp/request.json", json);
                    // run python script
                    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                    //python interprater location
                    start.FileName = "PythonScripts/venv/Scripts/python.exe";
                    //argument with file name and input parameters
                    start.Arguments = string.Format("{0}", "PythonScripts/GetPatients.py");
                    start.UseShellExecute = false;// Do not use OS shell
                    start.CreateNoWindow = true; // We don't need new window
                    start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
                    start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
                    start.LoadUserProfile = true;
                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                            string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                            

                        }
                    }

                    // read in the json file and create a list of patients
                    string json_input = File.ReadAllText(@"PythonScripts/tmp/output.json");
                    List<Patient> patients = JsonSerializer.Deserialize<List<Patient>>(json_input)!;
                    // replace the CurrentResults with the new results
                    CurrentResults = new ObservableCollection<Patient>(patients);



                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }



            }

 

        }
        private async void ImportQueryAsync(Object obj)
        {

            // if the output file already exists, delete it
            if (File.Exists(@"PythonScripts/tmp/output.json"))
            {
                File.Delete(@"PythonScripts/tmp/output.json");
            }
            Dictionary<string, Dictionary<string, List<string>>> request = new Dictionary<string, Dictionary<string, List<string>>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BackendReady = false;
                    ImportButtonText = "Loading...";

                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    foreach (string line in lines)
                    {
                        string course = null;
                        string plan = null;


                        string[] columns = line.Split(',');


                        string id = columns.ElementAt(0);
                        // add leading 0 if necessary, since csv removes leading 0s
                        if (columns.ElementAt(0).Length == 7)
                        {
                            id = "0" + columns.ElementAt(0);
                        }
                        // check if columns length is > 1
                        if (columns.Length > 1)
                        {
                            course = columns.ElementAt(1);
                        }

                        // check if columns length is > 2
                        if (columns.Length > 2)
                        {
                            plan = columns.ElementAt(2);
                        }
                        // if id not in requests keys, create a new object
                        if (!request.ContainsKey(id))
                        {
                            request.Add(id, new Dictionary<string, List<string>>());
                        }
                        // if course not in requests[id] keys, create a new object
                        if (course != null && !request[id].ContainsKey(course))
                        {
                            request[id].Add(course, new List<string>());
                        }
                        // if plan not in requests[id][course] keys, add it
                        if (course != null && plan != null && !request[id][course].Contains(plan))
                        {
                            request[id][course].Add(plan);
                        }


                    }
                    List<Patient> patient_requests = new List<Patient>();
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> entry in request)
                    {
                        Patient thisPatient = new Patient(entry.Key, entry.Value);
                        patient_requests.Add(thisPatient);



                    }
                    // convert request dictionary to json
                    string json = JsonSerializer.Serialize(patient_requests);
                    if (!Directory.Exists("PythonScripts/tmp"))
                    {
                        Directory.CreateDirectory("PythonScripts/tmp");
                    }
                    // write json to file
                    File.WriteAllText(@"PythonScripts/tmp/request.json", json);
                    // run python script

                    string result = await Task.Run(()=>callPythonHelper());
                    // read in the json file and create a list of patients
                    string json_input = File.ReadAllText(@"PythonScripts/tmp/output.json");
                    List<Patient> patients = JsonSerializer.Deserialize<List<Patient>>(json_input)!;
                    // replace the CurrentResults with the new results
                    CurrentResults = new ObservableCollection<Patient>(patients);
                    BackendReady = true;
                    ImportButtonText = "Import";



                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    BackendReady = true;
                    ImportButtonText = "Import";
                }



            }







        }
        private string callPythonHelper()
        {
            //string result = "";
            //System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            ////python interprater location
            //start.FileName = "PythonScripts/venv/Scripts/python.exe";
            ////argument with file name and input parameters
            //start.Arguments = string.Format("{0}", "PythonScripts/GetPatients.py");
            //start.UseShellExecute = false;// Do not use OS shell
            //start.CreateNoWindow = true; // We don't need new window
            //start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            //start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            //start.LoadUserProfile = true;
            //using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            //{
            //    using (StreamReader reader = process.StandardOutput)
            //    {
            //        string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
            //        result =  reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")


            //    }
            //}
            //return result;

            // start the GetPatients.exe file ocated in PythonScripts/dist/Getpatients
            var thisdir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
           
            //var process = Process.Start(thisdir + "/PythonScripts/dist/GetPatients/GetPatients.exe");
            //process.WaitForExit();
            // same as before but without showing a console window
            var start = new ProcessStartInfo();
            start.FileName = thisdir + "/PythonScripts/dist/GetPatients/GetPatients.exe";

            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;
            start.LoadUserProfile = true;
            var process = Process.Start(start);
            process.WaitForExit();
            return "success";
        }
        #endregion

    }
}
