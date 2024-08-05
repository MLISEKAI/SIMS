using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS
{
    public class Users
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string UserName { get; set; } 
        [Required]

        public string Pass { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserRole { get; set; }

    }
}
