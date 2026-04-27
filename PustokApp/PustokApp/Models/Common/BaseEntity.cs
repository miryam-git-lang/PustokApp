namespace PustokApp.Models.Common;

public abstract class BaseEntity
{
        public Guid Id { get; init; }

        protected BaseEntity()
        {
             Id = Guid.NewGuid();   
        }
}

// public class AuditEntity : BaseEntity
// {
//     public DateTime CreatedAt { get; set; }
//     public DateTime? UpdatedAt { get; set; }
// }