using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Services;
using System.Collections.Generic;

namespace Business_Logic_Layer.Interfaces
{
    public interface IUserService
    {
        RegistrationResult RegisterUser(UserRegisterDto dto); 
        IEnumerable<UserRegisterDto> GetAllUsers();
        UserProfileDto GetUserById(Guid id);
        //void MigratePasswords();

    }
}