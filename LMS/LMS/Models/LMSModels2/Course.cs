using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels2
{
    public partial class Course
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string Department { get; set; }
        public int CatId { get; set; }

        public Department DepartmentNavigation { get; set; }
    }
}
