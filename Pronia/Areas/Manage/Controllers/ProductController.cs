using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels.Product;
using Pronia.DAL;
using Pronia.Helpers;
using Pronia.Models;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this._env = env;
        }

        public async Task<IActionResult> Index()
        {
            var product = await _context.Products.Include(c=>c.Category)
                .Include(t=>t.TagProducts)
                .ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductImages).ToListAsync();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm vm)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
            {

                return View();
            }
            if(vm.CategoryId != null)
            {
                if(!await _context.Categories.AnyAsync(c=>c.Id == vm.CategoryId))
                {
                    ModelState.AddModelError("CategoryId",$"{vm.CategoryId} -id li category yoxdur");
                    return View(); 
                }
            }
           
            Product product = new Product()
            {
                Name = vm.Name,
                Price = vm.Price,
                Description = vm.Description,
                SKU = vm.SKU,
                CategoryId = vm.CategoryId,
                ProductImages= new List<ProductImage>()
            };
            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"{tagId} id li tag yoxdur");
                        return View();
                    }
                    TagProduct tagProduct = new TagProduct()
                    {
                        TagId = tagId,
                        Product = product
                    };
                    _context.TagProducts.Add(tagProduct);
                }
            }


            List<string> error= new List<string>();


            if (!vm.MainPhoto.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainPhoto", "Duzgun format daxil edilmeyib");
                return View();
            }
            if(vm.MainPhoto.Length > 3000000)
            {
                ModelState.AddModelError("MainPhoto", "max 3mb yukleye bilersiz");
                return View();

            }

            product.ProductImages.Add(new()
            {
                Primary=true,
                ImgUrl=vm.MainPhoto.Upload(_env.WebRootPath,"Upload/Product")
            });

            foreach (var item in vm.Images)
            {
                if (!item.ContentType.Contains("image/"))
                {
                    error.Add($"{item.Name} image formatinda deyil");
                    continue;
                }
                if (item.Length > 3000000)
                {
                    error.Add($"{item.Name} olcu max 3mb olabiler");

                    continue;
                }

                product.ProductImages.Add(new()
                {
                    Primary = false,
                    ImgUrl = item.Upload(_env.WebRootPath, "Upload/Product")
                });
            }

            TempData["error"] = error;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || !(_context.Products.Any(p => p.Id == id)))
            {
                return NotFound();
            }

            var product = await _context.Products.Include(c => c.Category)
                .Include(t => t.TagProducts)
                .ThenInclude(p => p.Tag).FirstOrDefaultAsync(x=>x.Id==id);
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            UpdateProductVm updateProductVm = new UpdateProductVm()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId= product.CategoryId,
                TagIds= new List<int>()
            };

            foreach(var item in product.TagProducts)
            {
                updateProductVm.TagIds.Add(item.Id);
            }

            return View(updateProductVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVm vm)
        {
            if (vm.Id == null || !(_context.Products.Any(p=>p.Id==vm.Id)))
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            Product oldProduct = _context.Products.Include(p=>p.TagProducts).FirstOrDefault(x=>x.Id == vm.Id);
            if (oldProduct == null)
            {
                return NotFound();
            }
            if (vm.CategoryId != null)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", $"{vm.CategoryId} -id li category yoxdur");
                    return View();
                }
            }

            _context.TagProducts.RemoveRange(oldProduct.TagProducts);

            if(vm.TagIds.Count() > 0)
            {
                
                foreach(var item in vm.TagIds)
                {
                    await _context.TagProducts.AddAsync(new TagProduct()
                    {
                        ProductId = oldProduct.Id,
                        TagId = item
                    });
                    
                }
            }



            oldProduct.Name = vm.Name;
            oldProduct.Price = vm.Price;
            oldProduct.Description = vm.Description;
            oldProduct.CategoryId = vm.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }
    }
}
