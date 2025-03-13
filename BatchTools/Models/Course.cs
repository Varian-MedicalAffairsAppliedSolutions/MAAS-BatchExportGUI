using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BatchTools.Models
{
    public class Course
    {
        public string? Name { get; set; }
        public string? PatientId { get; set; }
        public string? ExpandedDisplay { get; set; }

      
        public string? ClinicalStatus { get; set; }
        public string? Comment { get; set; }
        public string? CompletedDateTime { get; set; }
        public string? Diagnoses { get; set; }
        public string HistoryDateTime { get; set; }
        public string? HistoryDisplayName { get; set; }
        public string? HistoryUserName { get; set; }
        public string? Id { get; set; }
        public string?   Intent { get; set; }
        public string? StartDateTime { get; set; }
        public ObservableCollection<Plan>? Plans { get; set; }

        public Course()
        {
            
        }

        public Course(string id, string plan)
        {
            Id = id.Trim();
            Plans = new ObservableCollection<Plan>();
            Plans.Add(new Plan(plan));
        }
        public Course(string id, List<string> plans)
        {
            Id = id.Trim();
            Plans = new ObservableCollection<Plan>();
            foreach (var plan in plans)
            {
                Plans.Add(new Plan(plan));
            }
        }
    }

}
