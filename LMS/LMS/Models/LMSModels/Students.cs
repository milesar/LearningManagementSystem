using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            Enrolled = new HashSet<Enrolled>();
            Submissions = new HashSet<Submissions>();
        }

        public string UId { get; set; }
        public string Major { get; set; }

        public Department MajorNavigation { get; set; }
        public Users U { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
        public ICollection<Submissions> Submissions { get; set; }
    }
}
