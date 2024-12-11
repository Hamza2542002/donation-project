using donation_project.DTO;

namespace donation_project.Services
{
    public interface IAccountService
    {
        Task<bool> DeleteAccountAsync(DeleteAccountDTO model);
    }
}
