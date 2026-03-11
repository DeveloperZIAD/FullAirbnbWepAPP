using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AirbnbClone.Api.Controllers.authentication.Policies
{
    public class AccountOwnerHandler : AuthorizationHandler<AccountOwnerRequirement, Guid>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccountOwnerRequirement requirement,
            Guid targetUserId)
        {
            // 1. استخراج الـ User ID من التوكن الحالي
            var currentUserIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                                     ?? context.User.FindFirst("sub");

            if (currentUserIdClaim == null) return Task.CompletedTask;

            // 2. تحويل القيمة لـ Guid للمقارنة
            if (Guid.TryParse(currentUserIdClaim.Value, out Guid currentUserId))
            {
                // 3. المقارنة الذهبية: هل أنت صاحب هذا الـ ID؟
                // نتحقق أيضاً إذا كان المستخدم Admin (اختياري: لو تريد للأدمن صلاحية مطلقة)
                if (currentUserId == targetUserId || context.User.IsInRole("Admin"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}