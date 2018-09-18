using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
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
                    join u in db.Users
                    on student.StudentId equals u.Uid

                    select new
                    {
                        fname = u.First,
                        lname = u.Last,
                        uid = u.Uid,
                        dob = u.Dob.ToString(),
                        grade = student.Grade.ToString()
                    };

                return Json(query.ToArray());

    }

    /// <summary>
    /// Assume that a specific class can not have two categories with the same name.
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
         if (category is null)
            {
                var query =
                    from cl in db.Class
                    where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                    join co in db.Course
                    on cl.CatId equals co.CatId
                    into classes
                    from _cl in classes
                    where _cl.Department.Equals(subject) && _cl.Number == num
                    join ass_cat in db.AssignmentCat
                    on cl.CatId equals ass_cat.Class
                    into catset
                    from cats in catset
                    join asss in db.Assignment
                    on cats.AcatId equals asss.AssignmentCat
    
                select new
                {
                    aname = asss.Name,
                    cname = cats.Name,
                    due = asss.Due
                };

                    return Json(query.ToArray());
            }

            else
            {
                var query =
                    from cl in db.Class
                    where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                    join co in db.Course
                    on cl.CatId equals co.CatId
                    into classes
                    from _cl in classes
                    where _cl.Department.Equals(subject) && _cl.Number == num
                    join ass_cat in db.AssignmentCat
                    on cl.CatId equals ass_cat.Class
                    into catset
                    from cats in catset
                    where cats.Name.Equals(category)
                    join asss in db.Assignment
                    on cats.AcatId equals asss.AssignmentCat

                    select new
                    {
                        aname = asss.Name,
                        cname = cats.Name,
                        due = asss.Due
                    };

                    return Json(query.ToArray());
            
        }
    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the following fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {
        
            var query =
            from cl in db.Class
            where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
            join co in db.Course
            on cl.CatId equals co.CatId
            into classes
            from _cl in classes
            where _cl.Department.Equals(subject) && _cl.Number == num
            join ass_cat in db.AssignmentCat
            on cl.CatId equals ass_cat.Class

            select new
            {
                name = ass_cat.Name,
                weight = ass_cat.Weight
            };

                return Json(query.ToArray());
        
    }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// A class can not have two categories with the same name.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
                var query =
                    from cl in db.Class
                    where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                    join co in db.Course
                    on cl.CatId equals co.CatId
                    into classes
                    from _cl in classes
                    where _cl.Department.Equals(subject) && _cl.Number == num
                    join ass_cat in db.AssignmentCat
                    on cl.CatId equals ass_cat.Class

                select new
                {
                    Name = ass_cat.Name
                };

                if (query.ToArray().Count() != 0)
                {
                    return Json(new { success = false });
                }
                else
                {
                    // Create a new AssignmentCat object.
                    AssignmentCat ass_cat = new AssignmentCat()
                    {
                        Name = category,
                        Weight = catweight,
                        Class = (from cl in db.Class
                                 where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
                                 join co in db.Course
                                 on cl.CatId equals co.CatId
                                 select

                    };

                    // Add the new object to the AssignmentCat collection.
                    db.AssignmentCat.Add(ass_cat);

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
            
        }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// An assignment category (which belongs to a class) can not have two assignments with 
    /// the same name.
    /// If an assignment of the given category with the given name already exists, return success = false. 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {
       
            var query =
            from cl in db.Class
            where cl.Semester.Equals(season) && cl.Year == year && cl.CatId == num
            join co in db.Course
            on cl.CatId equals co.CatId
            into classes
            from _cl in classes
            where _cl.Department.Equals(subject) && _cl.Number == num
            join _ass in db.Assignment
            on cl.CatId equals _ass.Class

            select new
            {
                Category = _ass.AssignmentCat,
                Name = _ass.Name,
                ClassID = _ass.Class
            };

            if (query.ToArray().Count() != 0)
            {
                return Json(new { success = false });
            }
            else
            {
                // Create a new Assignment object.
                Assignment ass = new Assignment()
                {
                    Name = asgname,
                    AssignmentCat = query.ToArray()[0].Category,
                    Points = asgpoints,
                    Contents = asgcontents,
                    Handin = 1,
                    Due = asgdue,
                    Class = query.ToArray()[0].ClassID,
                };

                // Add the new object to the AssignmentCat collection.
                db.Assignment.Add(ass);

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
    }

    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
    {
        
            var assignment_query =
                from cl in db.Class
                where cl.Semester.Equals(season) && cl.Year == year
                join co in db.Course
                on cl.CatId equals co.CatId
                into classes
                from _cl in classes
                where _cl.Department.Equals(subject) && _cl.Number == num
                join _ass in db.Assignment
                on cl.CatId equals _ass.Class
                into assigns
                from assign in assigns
                join sub in db.Submissions
                on assign.AssId equals sub.Assignment
                into submissns
                from subs in submissns
                join user in db.Users
                on subs.Student equals user.Uid

                select new
                {
                    fname = user.First,
                    lname = user.Last,
                    uid = user.Uid,
                    time = subs.Time,
                    score = subs.Score
                };
            return Json(assignment_query.ToArray());
        
    }

    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns> 
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {
        
            var query =
                from cl in db.Class
                where cl.Semester.Equals(season) && cl.Year == year
                join co in db.Course
                on cl.CatId equals co.CatId
                into classes
                from _cl in classes
                where _cl.Department.Equals(subject) && _cl.Number == num
                join _ass in db.Assignment
                on cl.CatId equals _ass.Class
                into assigns
                from assign in assigns
                join sub in db.Submissions
                on assign.AssId equals sub.Assignment
                into submissns
                from subs in submissns
                where subs.Student.Equals(uid)
                join user in db.Users
                on subs.Student equals user.Uid
                into subm
                select subm;

            foreach (Submissions sub in query)
            {
                sub.Score = score;   
            }

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


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 6016)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
           
                var query =
                    from cl in db.Class
                    where cl.Teacher.Equals(uid)
                    join co in db.Course
                    on cl.CatId equals co.CatId


                select new
                {
                    subject = co.Department,
                    number = co.Number,
                    name = co.Name,
                    season = cl.Semester,
                    year = cl.Year
                };

                return Json(query.ToArray());
            
        }
  }
}