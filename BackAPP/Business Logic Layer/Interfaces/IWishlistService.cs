using Business_Logic_Layer.DTOs;

namespace Business_Logic_Layer.Interfaces
{
    public interface IWishlistService
    {
        string ToggleWishlist(WishlistDto wishlistDto); // إضافة أو إزالة
        IEnumerable<Guid> GetUserWishlist(Guid userId); // جلب قائمة معرفات العقارات المحفوظة
    }
}