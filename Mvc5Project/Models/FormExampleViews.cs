using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5Project.Models
{
    public class User
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
    }

    public class Country
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class UserCountry
    {

        [Key]
        [Column(Order = 0)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CountryID { get; set; }

        public User User { get; set; }
        public Country Country { get; set; }
    }

    public class Course
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }

    public class UserCourse
    {
        [Key]
        [Column(Order = 0)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CourseID { get; set; }
        public bool Checked { get; set; }

        public User User { get; set; }
        public IList<Course> Courses { get; set; }
    }

    public class UserDescription
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string Description { get; set; }

        public User User { get; set; }
    }

    public class FormCreateViewModel
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public List<Course> Courses { get; set; }
        public string Description { get; set; }
    }

}
