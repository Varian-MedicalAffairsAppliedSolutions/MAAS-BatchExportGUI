using BatchExportShell.Core.Events;
using BatchTools.Events;
using BatchTools.Models;
using Microsoft.VisualBasic.FileIO;
//using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Text.Json;

namespace BatchTools.ViewModels
{
    public class CurrentBatchViewModel : BindableBase
    {
        #region Declarations
        private bool _backendReady = true;

        public bool BackendReady
        {
            get { return _backendReady; }
            set { SetProperty(ref _backendReady, value); }
        }
        private string _createBatchText = "Add Plans";

        public string CreateBatchText
        {
            get { return _createBatchText; }
            set { SetProperty(ref _createBatchText, value); }
        }
        #endregion





        #region Commands

        public DelegateCommand<object> PlanListViewItemActivateCommand { get; private set; }
        public DelegateCommand RemovePlansCommand { get; private set; }
        public DelegateCommand SaveBatchCommand { get; private set; }
        public DelegateCommand LoadBatchCommand { get; private set; }
        public DelegateCommand ClearCurrentBatchCommand { get; private set; }
        //public DelegateCommand CreateBatchCommand { get; private set; }
        public DelegateCommand<object> CreateBatchCommand { get; private set; }
        #endregion

        #region Interfaces
        private IEventAggregator _eventAggregator;
      
        #endregion

        #region Collections

       
        private ObservableCollection<Plan> _currentBatch = new ObservableCollection<Plan>();
        public ObservableCollection<Plan> CurrentBatch
        {
            get { return _currentBatch; }
            set { SetProperty(ref _currentBatch, value); }
        }
        private ObservableCollection<Plan> _selectedPlanItems = new ObservableCollection<Plan>();
        public ObservableCollection<Plan> SelectedPlanItems
        {
            get { return _selectedPlanItems; }
            set { SetProperty(ref _selectedPlanItems, value); }

        }
        #endregion

        #region Constructor
        public CurrentBatchViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        

            _eventAggregator.GetEvent<AddBatchItem>().Subscribe(AddItem);
            _eventAggregator.GetEvent<ExportLocalEvent>().Subscribe(ExportLocal);

 
            PlanListViewItemActivateCommand = new DelegateCommand<object>(PlanListViewItemActivate);
            RemovePlansCommand = new DelegateCommand(RemovePlans);
            SaveBatchCommand = new DelegateCommand(SaveBatch);
            LoadBatchCommand = new DelegateCommand(LoadBatch);
            ClearCurrentBatchCommand = new DelegateCommand(ClearCurrentBatch);
            //CreateBatchCommand = new DelegateCommand(CreateBatch);
            CreateBatchCommand = new DelegateCommand<object>(CreateBatchAsync).ObservesCanExecute(()=>BackendReady);
     

        }

        private void CreateBatch()
        {


    
            Dictionary<string, Dictionary<string, List<string>>> request = new Dictionary<string, Dictionary<string, List<string>>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
                    string json = System.Text.Json.JsonSerializer.Serialize(patient_requests);
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
                    List<Patient> patients = System.Text.Json.JsonSerializer.Deserialize<List<Patient>>(json_input)!;
                    foreach (Patient patient in patients)
                    {
                        AddItem(patient);
                    }
               
                    



                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }



            }







        }
        private async void CreateBatchAsync(Object obj)
        {

            if (File.Exists(@"PythonScripts/tmp/shortcut.json"))
            {
                string json_input = File.ReadAllText(@"PythonScripts/tmp/shortcut.json");
                List<Patient> patients = System.Text.Json.JsonSerializer.Deserialize<List<Patient>>(json_input)!;
                foreach (Patient patient in patients)
                {
                    AddItem(patient);
                }
                BackendReady = true;
                return;

            }


            // if the output file already exists, delete it
            if (File.Exists(@"PythonScripts/tmp/output.json"))
            {
                File.Delete(@"PythonScripts/tmp/output.json");
            }
            Dictionary<string, Dictionary<string, List<string>>> request = new Dictionary<string, Dictionary<string, List<string>>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    BackendReady = false;
                    CreateBatchText = "Loading...";

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
                        try
                        {                    
                            Patient thisPatient = new Patient(entry.Key, entry.Value);
                            patient_requests.Add(thisPatient);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                    // convert request dictionary to json
                    string json = System.Text.Json.JsonSerializer.Serialize(patient_requests);
                    // write json to file
                    if (!Directory.Exists(@"PythonScripts/tmp"))
                    {
                        Directory.CreateDirectory(@"PythonScripts/tmp");
                    }
                    File.WriteAllText(@"PythonScripts/tmp/request.json", json);
                    // run python script
                    string result = await Task.Run(() => callPythonHelper());
                    // read in the json file and create a list of patients
                    string json_input = File.ReadAllText(@"PythonScripts/tmp/output.json");
                    List<Patient> patients = System.Text.Json.JsonSerializer.Deserialize<List<Patient>>(json_input)!;
                    foreach (Patient patient in patients)
                    {
                        AddItem(patient);
                    }
                    BackendReady = true;
                    CreateBatchText = "Add Plans";

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    BackendReady = true;
                    CreateBatchText = "Add Plans";
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

        #region Data Methods

        private string PayloadBuilder(string dir)
        {


            List<string> file_dir = new List<string>();
            var splitdir = dir.Split('|');
            string[] arr = splitdir[0].Split('\\');
            var args = splitdir[1].Split(',').ToList();
            foreach (string element in arr)
            {
                file_dir.Add(element);
            }
            Dictionary<string, List<List<string>>> payload = new Dictionary<string, List<List<string>>>();
            List<string> listFileDir = new List<string>(file_dir);
            List<List<string>> listlistFileDir = new List<List<string>>();
            listlistFileDir.Add(listFileDir);


            payload.Add("directory", listlistFileDir);

            List<string> listArgs = new List<string>(args);
            List<List<string>> listlistArgs = new List<List<string>>();
            listlistArgs.Add(listArgs);
            payload.Add("args", listlistArgs);


            foreach (Plan plan in CurrentBatch)
            {
                if (plan.PatientID is null)
                {
                    continue;
                }
                if (!payload.ContainsKey(plan.PatientID))
                {
                    payload[plan.PatientID] = new List<List<string>>();

                }
                try
                {
                    List<string> tmpList = new List<string>();
                    tmpList.Add(plan.Course.Replace(" ", "($)"));
                    tmpList.Add(plan.Name.Replace(" ", "($)"));
                    tmpList.Add(plan.CTSeriesUID);
                    tmpList.Add(plan.DoseUID);
                    tmpList.Add(plan.StructureSetUID);
                    tmpList.Add(plan.UID);
                    payload[plan.PatientID].Add(tmpList);

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

               


            }
            string json = JsonConvert.SerializeObject(payload);
            var thisdir = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\myFile.json";



            File.WriteAllText(thisdir, json);
            return thisdir;


            //var payloadstr = "{";
            //foreach (KeyValuePair<string, List<List<string>>> entry in payload)
            //{
            //    if (!payloadstr.Equals("{"))
            //    {
            //        payloadstr += "|";

            //    }
            //    payloadstr += entry.Key + ":"; // key == patientId
            //    payloadstr += "[";
            //    foreach (var element in entry.Value) //entry.Value == List<List<string>> -> element = List<string>(course,plan,data1,data2...)
            //    {
            //        payloadstr += "[";
            //        foreach (string item in element) //item == string
            //        {
            //            if (item != element.Last())
            //            {
            //                payloadstr += item + ",";

            //            }
            //            else
            //            {
            //                payloadstr += item + "]";
            //            }

            //        }
            //        if (element != entry.Value.Last())
            //        {
            //            payloadstr += ",";
            //        }
            //        else
            //        {
            //            //payloadstr += element;
            //            payloadstr += "]";

            //        }

            //    }


            //}


            //payloadstr += "}";
            //return payloadstr;

        }

       
        #endregion

        #region Command Methods

        private void ExportLocal(Object args)
        {
            string dir = (String)args;
            //PayloadBuilderJSON(dir);
            string payload = PayloadBuilder(dir);

            var thisdir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            var process = Process.Start(thisdir + "\\LocalMove\\EvilDICOMMoveTest.exe", payload);


        }
        private void ClearCurrentBatch()
        {
            CurrentBatch.Clear();
        }
        private void LoadBatch()
        {
            CurrentBatch.Clear();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
                //openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    string json = File.ReadAllText(filePath);
                    var CurrentBatchList = JsonConvert.DeserializeObject<List<Plan>>(json);
                    foreach (Plan plan in CurrentBatchList)
                    {
                        CurrentBatch.Add(plan);
                    }
           
                }

            }


      

        }
        private void SaveBatch()
        {
            // firtst, a save file dialog to find the location to save the file
            SaveFileDialog sd = new SaveFileDialog();
            // save type will be .json, so the filter is set to that
            sd.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
            sd.DefaultExt = "json";

            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var current_batch_json = JsonConvert.SerializeObject(CurrentBatch);
                File.WriteAllText(sd.FileName, current_batch_json);
            }



        }
        private void RemovePlans()
        {
            var selectedPlan = _selectedPlanItems.FirstOrDefault();
            CurrentBatch.Remove(selectedPlan);
            _selectedPlanItems.Clear();



        }

        private void PlanListViewItemActivate(object obj)
        {
            SelectedPlanItems.Clear();
            var args = (SelectionChangedEventArgs)obj;
            var source = (System.Windows.Controls.ListBox)args.Source;
            SelectedPlanItems.Add((Plan)source.SelectedItem);

        }
       



        private void AddItem(object obj)
        {
            if (obj is null) { return; };


            if (obj.GetType() == typeof(Plan))
            {
                Plan plan = (Plan)obj;
                CurrentBatch.Add(plan);
            }
            else if (obj.GetType() == typeof(Course))
            {
                Course course = (Course)obj;
                foreach (Plan plan in course.Plans)
                {
                    CurrentBatch.Add(plan);
                }
            }
            else if (obj.GetType() == typeof(Patient))
            {
                Patient patient = (Patient)obj;
                foreach (Course course in patient.Courses)
                {
                    foreach (Plan plan in course.Plans)
                    {
                        CurrentBatch.Add(plan);
                    }
                }
            }




        }




        #endregion


    }
}
