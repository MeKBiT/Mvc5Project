using Mvc5Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc5Project.Controllers
{
    public class FormExampleController : Controller
    {
        FormExampleDbContext context = new FormExampleDbContext();

        public static List<FormCreateViewModel> usrList = new List<FormCreateViewModel>();
        public static List<Course> courses = new List<Course>();


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            usrList.Clear();
            IList<User> users = context.Users.ToList();

            foreach (var user in users)
            {
                var countryId = context.UserCountries.Where(u => u.UserID == user.ID).Select(c => c.CountryID).FirstOrDefault();
                var country = context.Countries.Where(c => c.ID == countryId).Select(n => n.Name).FirstOrDefault();
                var description = context.UserDescriptions.Where(i => i.UserID == user.ID).Select(d => d.Description).FirstOrDefault();
                var courseIds = context.UserCourses.Where(u => u.UserID == user.ID).Select(x => x.CourseID).ToList();

                List<Course> courseNames = new List<Course>();
                foreach (var crsId in courseIds)
                {
                    courseNames.Add(new Course { Name = context.Courses.Where(x => x.ID == crsId).Select(x => x.Name).FirstOrDefault() });
                }

                usrList.Add(new FormCreateViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Gender = user.Gender,
                    Country = country,
                    UserID = user.ID,
                    Description = description,
                    Courses = courseNames
                });
            }
            return View(usrList);
        }

        public void CreateCourseList(FormCreateViewModel model)
        {
            courses.Clear();
            List<Course> usrCourses = context.Courses.ToList();
            foreach (var crs in usrCourses)
            {
                courses.Add(new Course { Name = crs.Name, ID = crs.ID, Checked = crs.Checked });
            }
            model.Courses = courses;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            var model = new FormCreateViewModel();
            CreateCourseList(model);
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCreateViewModel model)
        {
            User user = new User();
            user.ID = model.FirstName + new Random().Next(999999999).ToString() + model.LastName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Gender = model.Gender;
            context.Users.Add(user);

            var countryId = context.Countries.Where(m => m.Name == model.Country).Select(m => m.ID).FirstOrDefault();
            UserCountry userCountry = new UserCountry();
            userCountry.UserID = user.ID;
            userCountry.CountryID = countryId;
            context.UserCountries.Add(userCountry);

            UserDescription userDescription = new UserDescription();
            userDescription.ID = user.ID + countryId;
            userDescription.UserID = user.ID;
            userDescription.Description = model.Description;
            context.UserDescriptions.Add(userDescription);

            foreach (var course in model.Courses)
            {
                UserCourse userCourse = new UserCourse();
                if (course.Checked == true)
                {
                    userCourse.UserID = user.ID;
                    userCourse.CourseID = course.ID;
                    userCourse.Checked = true;
                    context.UserCourses.Add(userCourse);
                }
            }

            await context.SaveChangesAsync();
            return RedirectToAction("Index", "FormExample");

        }

        public string GetUserProperty(string id, System.Linq.Expressions.Expression<Func<User, string>> selector)
        {
            return context.Users.Where(x => x.ID == id).Select(selector).FirstOrDefault();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Edit(string id, FormCreateViewModel model)
        {
            model.FirstName = GetUserProperty(id, x => x.FirstName);
            model.LastName = GetUserProperty(id, x => x.LastName);
            model.Email = GetUserProperty(id, x => x.Email);
            model.Gender = GetUserProperty(id, x => x.Gender);
            model.Description = context.UserDescriptions.Where(x => x.UserID == id).Select(x => x.Description).FirstOrDefault();
            var countryId = context.UserCountries.Where(x => x.UserID == id).Select(x => x.CountryID).FirstOrDefault();
            model.Country = context.Countries.Where(x => x.ID == countryId).Select(x => x.Name).FirstOrDefault();
            model.UserID = id;

            CreateCourseList(model);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCreateViewModel model)
        {
            User user = context.Users.Where(u => u.ID == model.UserID).FirstOrDefault();
            TryUpdateModel(user, "", new string[] { "FirstName", "LastName", "Email", "Gender" });

            UserDescription userDescription = context.UserDescriptions.Where(u => u.UserID == model.UserID).FirstOrDefault();
            TryUpdateModel(userDescription, "", new string[] { "Description" });

            UserCountry userCountry = context.UserCountries.Where(u => u.UserID == model.UserID).FirstOrDefault();
            var countryId = context.Countries.Where(n => n.Name == model.Country).Select(i => i.ID).FirstOrDefault();
            if (userCountry.CountryID != countryId)
            {
                if (userCountry != null)
                {
                    context.UserCountries.Remove(userCountry);
                    context.SaveChanges();
                }
                UserCountry uCountry = new UserCountry();
                uCountry.UserID = model.UserID;
                uCountry.CountryID = countryId;
                context.UserCountries.Add(uCountry);
            }

            var userCourses = context.UserCourses.Where(u => u.UserID == model.UserID).ToList();
            List<string> uCourseIds = new List<string>();
            foreach (var crId in userCourses)
            {
                uCourseIds.Add(crId.CourseID);
            }
            var newCourses = model.Courses.Where(x => x.Checked == true).ToList();
            List<string> nCourseIds = new List<string>();
            foreach (var crId in newCourses)
            {
                nCourseIds.Add(crId.ID);
            }
            if (!uCourseIds.SequenceEqual(nCourseIds))
            {
                foreach (var crId in userCourses)
                {
                    UserCourse userCourse = context.UserCourses.Where(x => x.UserID == model.UserID && x.CourseID == crId.CourseID).FirstOrDefault();
                    context.UserCourses.Remove(userCourse);
                    context.SaveChanges();
                }
                foreach (var course in model.Courses)
                {
                    UserCourse userCourse = new UserCourse();
                    if (course.Checked == true)
                    {
                        userCourse.UserID = user.ID;
                        userCourse.CourseID = course.ID;
                        userCourse.Checked = true;
                        context.UserCourses.Add(userCourse);
                    }
                }

            }
            context.SaveChanges();

            return RedirectToAction("Index", "FormExample");

        }


    }
}