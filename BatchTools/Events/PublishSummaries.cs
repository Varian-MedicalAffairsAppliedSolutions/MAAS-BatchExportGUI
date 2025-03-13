using BatchTools.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchTools.Events
{
    public class PublishSummaries : PubSubEvent<ObservableCollection<UISummary>>
    {
    }
}
