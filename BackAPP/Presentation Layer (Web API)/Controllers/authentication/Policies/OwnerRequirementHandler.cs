using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer__Web_API_.Controllers.authentication.Policies;
using System.Security.Claims;

public class OwnerRequirementHandler : AuthorizationHandler<OwnerRequirement, Guid>
{
    private readonly IServiceProvider _serviceProvider;

    public OwnerRequirementHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerRequirement requirement, Guid listingId)
    {
        // بنفتح Scope جديد جوه الميثود عشان نجيب السيرفس بأمان
        using (var scope = _serviceProvider.CreateScope())
        {
            var listingService = scope.ServiceProvider.GetRequiredService<ListingService>();

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return;

            var userId = Guid.Parse(userIdClaim.Value);
            var isOwner = listingService.IsUserOwnerOfListing(userId, listingId);

            if (isOwner)
            {
                context.Succeed(requirement);
            }
        }
    }
}