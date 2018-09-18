using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels2
{
    public partial class AssignmentCat
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public int AcatId { get; set; }
        public int Class { get; set; }

        public Class ClassNavigation { get; set; }
    }
}
