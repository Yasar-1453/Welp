using Pronia.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models
{
    public class Slider : BaseEntity
    {
        public string Title { get; set; }
        public string SubzTitle { get; set; }
        public string Description { get; set; }
        public string? ImgUrl { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
