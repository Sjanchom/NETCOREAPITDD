using System.ComponentModel.DataAnnotations;

namespace HappyKids.Models.DataTranferObjects
{
    public class CredentialModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
