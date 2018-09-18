using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submissions>();
        }

        public int AssId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public string Contents { get; set; }
        public sbyte Handin { get; set; }
        public DateTime Due { get; set; }
        public int AssignmentCat { get; set; }
        public int Class { get; set; }

        public ICollection<Submissions> Submissions { get; set; }
    }
}
