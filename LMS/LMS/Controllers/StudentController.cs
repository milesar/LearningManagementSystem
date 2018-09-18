using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 6016)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            
                var query =
                    from stud in db.Enrolled
                    where stud.Student.Equals(uid)
                    join cl in db.Class
                    on stud.ClassId equals cl.ClassId
                    into classes
                    from cs in classes
                    join co in db.Course
                    on cs.CatId equals co.CatId
                    
                    select new
                    {
                        subject = co.Department,
                        number = co.Number,
                        name = co.Name,
                        season = cs.Semester,
                        year = cs.Year,
                        grade = stud == null ? "--" : stud.Grade
                    };

                return Json(query.ToArray());
            
        }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
        
            var query =
                from stud in db.Enrolled
                where stud.Student.Equals(uid)
                join cl in db.Class
                on stud.ClassId equals cl.ClassId
                into classes
                from cs in classes
                join ass in db.Assignment
                on cs.ClassId equals ass.Class
                into final
                from f in final
                join cat in db.AssignmentCat
                on f.AssignmentCat equals cat.AcatId
                    

                select new
                {
                    aname = f.Name,
                    cname = cat.Name,
                    due = f.Due,
                    score = (from stud in db.Enrolled
                                where stud.Student.Equals(uid)
                                join cl in db.Class
                                on stud.ClassId equals cl.ClassId
                                into classes
                                from cs in classes
                                join ass in db.Assignment
                                on cs.ClassId equals ass.Class
                                into final
                                from f in final
                                join subs in db.Submissions
                                on f.AssId equals subs.Assignment
                                select subs ).ToArray()[0]
                };

                return Json(query.ToArray());
        
    }

    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, string category, string asgname, string uid, string contents)
    {
            
                var query =
                    from stud in db.Enrolled
                    where stud.Student.Equals(uid)
                    join cl in db.Class
                    on stud.ClassId equals cl.ClassId
                    into classes
                    from cs in classes
                    where cs.Year.Equals(year) && cs.Semester.Equals(season)
                    join ass in db.Assignment
                    on cs.ClassId equals ass.Class
                    into final
                    from f in final
                    join subs in db.Submissions
                    on f.AssId equals subs.Assignment
                    select subs;

                Submissions s = new Submissions()
                {
                    Time = DateTime.Now,
                    Assignment = query.ToArray()[0].Assignment,
                    Student = uid,
                    ContentsText = contents,
                    ContentsBin = System.Text.Encoding.ASCII.GetBytes(contents)
                };
          
                // Add the new object to the AssignmentCat collection.
                db.Submissions.Add(s);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Json(new { success = false });
                }
                return Json(new { success = true });
           
    }
    /*
     * Stil trying to get this to work... 8/2/18
     * 
    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// Otherwise, the point-value of a letter grade for the UofU is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {
        using (Team6Context db = new Team6Context())
        {
            var qcat =
                from stud in db.Enrolled
                where stud.Student.Equals(uid)
                join cl in db.Class
                on stud.ClassId equals cl.ClassId
                into classes
                from cs in classes
                join ass in db.Assignment
                on cs.ClassId equals ass.Class
                into final
                from f in final
                join cat in db.AssignmentCat
                on f.AssignmentCat equals cat.AcatId
                into finalfinal
                from ff in finalfinal
                join co in db.Course
                on ff.AcatId equals co.CatId

                select new
                {
                    category = co.CatId,
                    season = cs.Semester,
                    num = co.Number,
                    subject = co.Department,
                    year = cs.Year
                };

            var allCats = 0;
            var numericGradeTotal = 0; //total weighted score for the class

            var categories = qcat.ToArray();

            foreach (var cat in categories) {
                var catTotalPoints = 0;
                    // query to get assignments for that category
                var qasg = from cl in db.Class
                            where cl.Semester.Equals(cat.season) && cl.Year == cat.year && cl.CatID == cat.num
                            join co in db.Course
                            on cl.CatID equals co.CatId
                            into classes
                            from _cl in classes
                            where _cl.Department.Equals(cat.subject)
                            join ass_cat in db.AssignmentCat
                            on cl.CatID equals ass_cat.Class
                            into catset
                            from cats in catset
                            join asss in db.Assignment
                            on cats.AcatId equals asss.AssignmentCat

                            select new
                            {
                                asgns = asss.AssId,
                                score = asss.Points
                            };

                    var asgns = qcat.ToArray();
                    foreach (var asg in asgns) {
                        catTotalPoints += asg..Max(); //maximum points possible in the assignment
                        var qsub = query.ToArray(); //to get the submission to the specific assignment(gets either 0 or 1 things) for 1 student

                        if (qsub.count() == 0)
                        {
                                catEarned += 0;
                        }
                                
                        else { catEarned += qsub.first().score }
                    }

                    catPercent = (catEarned / catTotalPoints);//percentage of points the student earned in that category
                    catScore = catPercent * cat.weight;
                    numericGradeTotal += catScore;
                    allCats += cat.weight;
            }

            numericGradeTotal *= (100 / allCats); //rescale category weight into a percentage
            return null;
        }
    }
    */

        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. False if the student is already enrolled in the class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            var query =
                   from cl in db.Class
                   where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                   join co in db.Course
                   on cl.CatId equals co.CatId
                   join e in db.Enrolled
                   on cl.ClassId equals e.ClassId
                   into roll
                   from student in roll
                   where student.Equals(uid) && co.Department.Equals(subject)

                   select new
                   {
                       Name = student.StudentId,
                       Class = cl.ClassId
                   };

            if (query.ToArray().Count() != 0)
            {
                return Json(new { success = false });
            }
            else
            {
                // Create a new AssignmentCat object.
                Enrolled e = new Enrolled()
                {
                    StudentId = uid,
                    ClassId = query.ToArray()[0].Class,
                };

                return Json(query.ToArray());
            }
        }

    }
}