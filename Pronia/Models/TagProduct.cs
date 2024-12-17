using Pronia.Models.Base;

namespace Pronia.Models
{
    public class TagProduct : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
