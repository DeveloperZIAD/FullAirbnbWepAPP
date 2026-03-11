using System.ComponentModel.DataAnnotations;

namespace Data_Access_Layer.Entities
{
    public abstract class BaseEntity
    {
      
        public Guid Id { get; set; } 

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}