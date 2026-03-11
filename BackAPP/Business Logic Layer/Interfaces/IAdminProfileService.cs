using Business_Logic_Layer.DTOs.Admin;

namespace Business_Logic_Layer.Interfaces
{
    public interface IAdminProfileService
    {
        string CreateAdminProfile(AdminProfileDto adminDto);
        bool UpdatePermissions(Guid userId, string newPermissions);
        AdminProfileDto GetAdminDetails(Guid userId);
        bool DeactivateAdmin(Guid userId);
    }
}