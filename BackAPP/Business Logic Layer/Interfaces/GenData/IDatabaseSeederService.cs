using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Interfaces.GenData
{
    public interface IDatabaseSeederService
    {
        Task<string> SeedSystemDataAsync(int userCount);
        Task<string> SeedDataByCategoryAsync(int userCount, string categoryName);
        Task<string> SeedReviewsAsync(Guid listingId, int count);
        Task<string> SeedListingsByCountryAsync(int count, string countryCode);
    }
}
