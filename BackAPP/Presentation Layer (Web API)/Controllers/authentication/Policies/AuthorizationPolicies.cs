using AirbnbClone.Api.Controllers.Authentication;
using Business_Logic_Layer;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer__Web_API_.Controllers.authentication.Policies;
using AirbnbClone.Api.Controllers.authentication.Policies;

namespace AirbnbClone.Api.Controllers.authentication
{
    // this file only to add all the custom policies in one place and use it in the IdentityServiceExtensions.cs 
    // here we define the names of the policies and the logic for each policy if it's more complex than just checking a role or claim 
    // we make sure of use the right services and handlers to implement the logic of the policies
    public static class AuthorizationPolicies
    {
        // أسماء البولسي عشان نستخدمها في الـ Controllers
        public const string AdminOnly = "AdminOnly";
        public const string HostOnly = "HostOnly";
        public const string ActiveGuest = "ActiveGuest";
        public const string ListingOwner = "ListingOwner";
        public const string AccountOwner = "AccountOwner";

        public static void AddCustomPolicies(AuthorizationOptions options)
        {
     
           
            options.AddPolicy(ActiveGuest, policy =>
                policy.RequireRole(UserRoles.Guest)
                      .RequireClaim("EmailVerified", "true"));

            options.AddPolicy(AccountOwner, policy =>
                        policy.RequireAuthenticatedUser()
                              .Requirements.Add(new AccountOwnerRequirement()));

            // سياسة حماية العقارات (التي أنشأناها سابقاً)
            options.AddPolicy(ListingOwner, policy =>
                policy.Requirements.Add(new OwnerRequirement()));
        }
    }
}