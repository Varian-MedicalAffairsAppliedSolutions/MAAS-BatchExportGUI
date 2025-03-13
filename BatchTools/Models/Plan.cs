using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchTools.Models
{
    public class Plan
    {
        public string? UID { get; set; }
        public string? PatientID { get; set; }
        public string? ApprovalStatus { get;  set; }
        public string? Comment { get;  set; }
        public string? CreationDateTime { get;  set; }
        public string? CreationUserName { get;  set; }
        public string? HistoryDateTime { get;  set; }
        public string? HistoryUserDisplayName { get;  set; }
        public string? HistoryUserName { get;  set; }
        public string? PlanIntent { get;  set; }
        public string? PlanningApprovalDate { get;  set; }
        public string? PlanningApprover { get;  set; }
        public string? PlanningApproverDisplayName { get;  set; }
        public string? TreatmentApprovalDate { get;  set; }
        public string? TreatmentApprover { get;  set; }
        public string? TreatmentApproverDisplayName { get;  set; }
        public string? Name { get; set; }
        public string? Course { get; set; }
        public string? DoseUID { get; set; }
        public string? StructureSetUID { get; set; }
        public string? CTSeriesUID { get; set; }
        public string? PlanType { get; set; }
        public string? ExpandedDisplay { get; set; }
        public string?   BulkDataString { get; set; }

        public Plan()
        {
        }

        public Plan(string name)
        {
            Name = name.Trim();
        }
    }

}
