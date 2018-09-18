using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels2
{
    public partial class Enrolled
    {
        public string StudentId { get; set; }
        public int ClassId { get; set; }
        public string Grade { get; set; }

        public Class Class { get; set; }
        public Students Student { get; set; }
    }
}
