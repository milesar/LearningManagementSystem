using LMS.Models.LMSModels2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        // TODO: Add a protected database context variable once you have scaffolded your team database
        // for example: 
        protected Team6Context db;

        public CommonController()
        {
            // TODO: Initialize your context once you have scaffolded your team database
            // for example:
            db = new Team6Context();
        }

        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different Context - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor (look this up if interested).
        */

        // TODO: Add a "UseContext" method if you wish to change the "db" context for unit testing
        //       See the lecture on testing

        // TODO: Uncomment this once you have created the variable "db"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            // TODO: Do not return this hard-coded array.
            
                var query =
                  from row in db.Department
                  select new
                  {
                      name = row.Name,
                      subject = row.Subject,
                  };

                return Json(query.ToArray());

        }

        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 6016)
        ///            "cname": The course name (e.g. "Database Systems and Applications")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var query = from d in db.Department
                select new
                {
                    subject = d.Subject,
                    dname = d.Name,
                    courses = ( from c in db.Course
                                where c.Department.Equals(d.Subject)
                                select new
                                {
                                    number = c.Number,
                                    cname = c.Name
                                }).ToArray()
                };
               
                return Json(query.ToArray());
            
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
           
                var query =
                    from co in db.Course
                    where co.Department.Equals(subject) && co.Number == number
                    join cl in db.Class
                    on co.CatId equals cl.CatId
                    into offerings
                    from off in offerings.DefaultIfEmpty()
                    join user in db.Users
                    on off.Teacher equals user.Uid

                    select new
                    {
                        season = off.ClassId.ToString(),
                        year = off.Year.ToString(),
                        location = off.Location,
                        start = off.Start.ToString(),
                        end = off.End.ToString(),
                        fname = user.First,
                        lname = user.Last,
                    };

                return Json(query.ToArray());
            
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
                var query =
                  from cl in db.Class
                  where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                  join ass in db.Assignment
                  on cl.CatId equals ass.Class
                  into class_ass
                  from cass in class_ass.DefaultIfEmpty()
                  where cass.AssignmentCat.Equals(category) && cass.Name.Equals(asgname)

                  select new
                  {
                      contents = cass.Contents 
                  };
                return Content(query.ToString());
            
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {
            
                var query =
                  from cl in db.Class
                  where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                  join ass in db.Assignment
                  on cl.CatId equals ass.Class
                  into class_ass
                  from cass in class_ass.DefaultIfEmpty()
                  where cass.AssignmentCat.Equals(category) && cass.Name.Equals(asgname)
                  join sub in db.Submissions
                  on cass.AssId equals sub.Assignment
                  where sub.Student.Equals(uid)

                  select new
                  {
                      contents = sub.ContentsText
                  };
                return Content(query.ToString());
            
        }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>The user JSON object or an object containing {success: false} if the user doesn't exist</returns>
    public IActionResult GetUser(string uid)
    {
        
                if (db.Students.Where(s => s.UId.Equals(uid)).Count() != 0)
                {
                    var query =
                        from user in db.Users
                        where user.Uid.Equals(uid)
                        join s in db.Students
                        on user.Uid equals s.UId


                    select new
                    {
                        fname = user.First,
                        lname = user.Last,
                        uid = user.Uid,
                        department = s.Major
                    };
                  
                    return Json(query.ToArray()[0]);
                }

                if (db.Professors.Where(p => p.UId.Equals(uid)).Count() != 0)
                {
                    var query =
                        from user in db.Users
                        where user.Uid.Equals(uid)
                        join p in db.Professors
                        on user.Uid equals p.UId

                    select new
                    {
                        fname = user.First,
                        lname = user.Last,
                        uid = user.Uid,
                        department = p.Department
                    };
                    return Json(query.ToArray()[0]);
                }
                return Json( new { success = false });
            
    }
    /*******End code to modify********/

  }
}