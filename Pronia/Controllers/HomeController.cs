using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Product> products = _db.Products.Include(p=>p.ProductImages).ToList();
            return View(products);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product = await _db.Products.Include(x=>x.Category).
                Include(x=>x.ProductImages)
                .Include(x=>x.TagProducts)
                .ThenInclude(tp=>tp.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            ViewBag.ReProduct =await _db.Products.Include(p=>p.ProductImages).Where(x=>x.CategoryId==product.CategoryId).ToListAsync();
            return View(product);
        }
    }
}
