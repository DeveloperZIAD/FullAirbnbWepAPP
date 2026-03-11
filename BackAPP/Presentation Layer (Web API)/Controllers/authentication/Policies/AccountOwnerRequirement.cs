using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace AirbnbClone.Api.Controllers.authentication.Policies
{
    public class AccountOwnerRequirement : IAuthorizationRequirement { }
}