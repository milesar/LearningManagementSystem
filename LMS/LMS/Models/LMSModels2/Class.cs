using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels2
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCat = new HashSet<AssignmentCat>();
            Enrolled = new HashSet<Enrolled>();
        }

        public string Semester { get; set; }
        public string Location { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int ClassId { get; set; }
        public string Teacher { get; set; }
        public int CatId { get; set; }
        public int Year { get; set; }

        public Professors TeacherNavigation { get; set; }
        public ICollection<AssignmentCat> AssignmentCat { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
    }
}
