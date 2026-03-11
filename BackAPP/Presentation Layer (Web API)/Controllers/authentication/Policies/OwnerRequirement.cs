using Microsoft.AspNetCore.Authorization;

namespace Presentation_Layer__Web_API_.Controllers.authentication.Policies
{
    public class OwnerRequirement : IAuthorizationRequirement
    {
        // ممكن تضيف هنا خصائص لو محتاج، بس حالياً هي مجرد Marker
    }
}