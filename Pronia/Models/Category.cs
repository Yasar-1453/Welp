using Pronia.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Category : BaseEntity
    {
        [Required,StringLength(30,ErrorMessage ="max 30 herif yazabilersen"),MinLength(3,ErrorMessage ="minimum 3 herf olabiler")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
