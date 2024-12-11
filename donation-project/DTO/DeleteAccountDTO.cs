using System.ComponentModel.DataAnnotations;

namespace donation_project.DTO
{
    public class DeleteAccountDTO
    {
        [Required]
        public string Id { get; set; }
    }
}
