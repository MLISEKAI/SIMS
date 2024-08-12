using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS
{
    public class Users
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Pass { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserRole { get; set; }
    }

    public class Students
    {
        [Key]
        [ForeignKey("Users")]
        public int Student_ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Student_Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        [MaxLength(11)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public virtual ICollection<Students_Courses>? Students_Courses { get; set; }
        public virtual ICollection<Scores>? Scores { get; set; }

    }

    public class AdminSystem
    {
        [Key]
        public int Admin_ID { get; set; }

        [ForeignKey("Admin_ID")]
        public Users User { get; set; }
    }

    public class Teachers
    {
        [Key]
        [ForeignKey("Users")]
        public int Teacher_ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Teacher_Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        [MaxLength(11)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public virtual ICollection<Teachers_Courses>? Teachers_Courses { get; set; }
        public virtual ICollection<Scores>? Scores { get; set; }

    }

    public class Courses
    {
        [Key]
        public int Course_ID { get; set; }

        [Required(ErrorMessage = "Course Name is required")]
        [StringLength(100, ErrorMessage = "Course Name cannot be longer than 100 characters")]
        public string Course_Name { get; set; }

        [Required(ErrorMessage = "Course Code is required")]
        [StringLength(20, ErrorMessage = "Course Code cannot be longer than 20 characters")]
        public string Course_code { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        public virtual ICollection<Students_Courses>? Students_Courses { get; set; }
        public virtual ICollection<Teachers_Courses>? Teachers_Courses { get; set; }
        public virtual ICollection<Scores>? Scores { get; set; }

    }

    public class Students_Courses
    {
        public int Student_ID { get; set; }
        public Students Student { get; set; }

        public int Course_ID { get; set; }
        public Courses Course { get; set; }
    }

    public class Teachers_Courses
    {
        public int Teacher_ID { get; set; }
        public Teachers Teacher { get; set; }

        public int Course_ID { get; set; }
        public Courses Course { get; set; }
    }

 
    public class Scores
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Score_ID { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        public int Student_ID { get; set; }
        public virtual Students Student { get; set; }

        [Required(ErrorMessage = "Teacher ID is required.")]
        public int Teacher_ID { get; set; }
        public virtual Teachers Teacher { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public int Course_ID { get; set; }
        public virtual Courses Course { get; set; }

        [Required(ErrorMessage = "Score is required.")]
        public string Score { get; set; }

        [Required(ErrorMessage = "Grade is required.")]
        public string? Grade { get; set; }
    }

}

