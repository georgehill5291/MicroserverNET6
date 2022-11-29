using System.ComponentModel.DataAnnotations;
using UserAPI.Filter;
using UserAPI.Models.Entities;

namespace UserAPI.Models
{
    public class User : IEntity
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public Role Role { get; set; }
    }
}
