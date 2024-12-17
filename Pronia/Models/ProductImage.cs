using Pronia.Models.Base;

namespace Pronia.Models
{
    public class ProductImage : BaseEntity
    {
        public string ImgUrl { get; set; }
        public bool Primary { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
