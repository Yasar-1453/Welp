using Pronia.Models.Base;

namespace Pronia.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public List<TagProduct> TagProducts { get; set; }
    }
}
