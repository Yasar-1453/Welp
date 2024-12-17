using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Helpers;
using Pronia.Models;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        AppDbContext _context;
        private readonly IWebHostEnvironment env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.ToListAsync();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            slider.ImgUrl = slider.File.Upload(env.WebRootPath, "Upload/Slider");

            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var slider = _context.Sliders.FirstOrDefault(p => p.Id == id);
            if (slider == null) return NotFound();
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int? id)
        {
            if (id == null) return NotFound();
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category newSlider)
        {


            var oldSlider = _context.Categories.FirstOrDefault(x => x.Id == newSlider.Id);
            if (oldSlider == null) return NotFound();

            oldSlider.Name = newSlider.Name;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
