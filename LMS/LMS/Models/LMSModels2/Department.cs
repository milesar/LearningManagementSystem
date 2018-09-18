using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels2
{
    public partial class Department
    {
        public Department()
        {
            Course = new HashSet<Course>();
            Professors = new HashSet<Professors>();
            Students = new HashSet<Students>();
        }

        public string Subject { get; set; }
        public string Name { get; set; }

        public ICollection<Course> Course { get; set; }
        public ICollection<Professors> Professors { get; set; }
        public ICollection<Students> Students { get; set; }
    }
}
