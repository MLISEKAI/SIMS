using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS
{
    public class Student
    {
        [Required]
        public int Student_ID { get; set; }
        [Required]

        [StringLength(100)]
        public string Student_Name { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]

        [StringLength(100)]
        public string Address { get; set; }
        [Required]
        public int Phone { get; set; }
    }
}
