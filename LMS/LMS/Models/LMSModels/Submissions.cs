using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public DateTime Time { get; set; }
        public int Assignment { get; set; }
        public string Student { get; set; }
        public int? Score { get; set; }
        public byte[] ContentsBin { get; set; }
        public string ContentsText { get; set; }

        public Assignment AssignmentNavigation { get; set; }
        public Students StudentNavigation { get; set; }
    }
}
