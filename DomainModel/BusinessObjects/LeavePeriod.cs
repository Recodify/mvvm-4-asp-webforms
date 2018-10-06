using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel.BusinessObjects
{
    public enum LeavePeriodType
    {
        Annual, Unplanned, Sick, Gardening
    }

    [Serializable]
    public class LeavePeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeavePeriodType Type { get; set; }
    }
}
