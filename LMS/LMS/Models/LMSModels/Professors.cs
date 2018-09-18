using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Class = new HashSet<Class>();
        }

        public string UId { get; set; }
        public string Department { get; set; }

        public Department DepartmentNavigation { get; set; }
        public Users U { get; set; }
        public ICollection<Class> Class { get; set; }
    }
}
