using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels2
{
    public partial class Users
    {
        public string Uid { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public DateTime Dob { get; set; }
        public string Password { get; set; }

        public Administrators Administrators { get; set; }
        public Professors Professors { get; set; }
        public Students Students { get; set; }
    }
}
