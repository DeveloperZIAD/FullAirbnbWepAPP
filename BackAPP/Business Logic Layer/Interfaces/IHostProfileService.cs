using Business_Logic_Layer.DTOs;

namespace Business_Logic_Layer.Interfaces
{
    public interface IHostProfileService
    {
        string CreateOrUpdateProfile(HostProfileDto profileDto);
        HostProfileDto GetProfileByUserId(Guid userId);
        bool VerifyHost(Guid userId); // للأدمن فقط
        Task<IEnumerable<ListingDto>> GetHostListings(Guid hostId);
        Task<object> GetHostDashboardStats(Guid hostId);
    }
}