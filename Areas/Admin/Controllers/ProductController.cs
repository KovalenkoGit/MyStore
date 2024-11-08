using MyStore.Models;
using MyStore.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using MyStore.Data;
using Microsoft.AspNetCore.Authorization;

namespace MyStore.Areas.Admin.Controllers
{
	[Area("admin")]
	[Route("admin/[controller]/[action]")]
	[Authorize(Roles = "Admin")]
	public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository = null;
        private readonly ICategoryRepository _categoryRepository = null;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepository,
                                 ICategoryRepository categoryRepository,
                                 IWebHostEnvironment webHostEnvironment) 
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("~/all-product")]
        public async Task<ViewResult> GetAllProduct(bool isSuccess = false, Guid? productId = null, string actionType = null)
		{
			if (isSuccess)
			{
				ViewBag.IsSuccess = true;
				ViewBag.ProductId = productId;
				ViewBag.ActionType = actionType;
			}
			var data = await _productRepository.GetAllProduct();
            return View(data);
        }
        [Route("product-details/{productId}", Name ="ProductDetailsRoute")]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
			var product = await _productRepository.GetProductById(productId);
			if (product == null)
			{
				return NotFound();
			}
			return View("~/Views/Product/GetProduct.cshtml", product); // Вказуємо шлях до представлення
		}
		[Route("~/category")]
        public async Task<ViewResult> GetProductByCategory(Guid categoryId)
        {
            var data = await _productRepository.GetProductByCategoryId(categoryId);
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> SearchProduct(string nameProduct)
        {
            var products = await _productRepository.SearchProducts(nameProduct);
            return View("SearchProduct", products);
        }
        [Authorize]
        public async Task<ViewResult> AddNewProduct(bool isSuccess = false, Guid? productId = null)
        {
            var model = new ProductModel();
            ViewBag.Category = new SelectList(await _categoryRepository.GetCategory(), "CategoryId", "Name");
            ViewBag.IsSuccess = isSuccess;
            ViewBag.ProductId = productId;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                // Додавання обкладинки, якщо вона вибрана
                if (productModel.CoverPhoto != null)
                {
                    string folder = "product/cover/";
                    productModel.CoverImageUrl = await UploadImage(folder, productModel.CoverPhoto);
                }

                // Додавання галереї, якщо є файли
                if (productModel.GalleryFiles != null && productModel.GalleryFiles.Any())
                {
                    string folder = "product/gallery/";
                    productModel.Gallery = new List<GalleryModel>();

                    foreach (var file in productModel.GalleryFiles)
                    {
                        var gallery = new GalleryModel
                        {
                            Name = file.FileName,
                            ImageURL = await UploadImage(folder, file)
                        };
                        productModel.Gallery.Add(gallery);
                    }
                }

                Guid id = await _productRepository.AddNewProduct(productModel);
                if (id != Guid.Empty)
                {
                    return RedirectToAction("GetAllProduct", new { isSuccess = true, productId = id, actionType = "add" });
                }
            }

            // Повертаємо форму з попередніми даними, якщо модель не валідна
            ViewBag.Category = new SelectList(await _categoryRepository.GetCategory(), "CategoryId", "Name");
            return View(productModel);
        }

        private async Task<string> UploadImage (string folderPath, IFormFile file)
        { 
            folderPath += Guid.NewGuid().ToString() + "-" + file.FileName;
            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            return "/" + folderPath;
        }
        // Метод для відображення форми редагування
        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid productId)
        {
			var product = await _productRepository.GetProductById(productId);
            if (product == null) return NotFound();

            ViewBag.Category = new SelectList(await _categoryRepository.GetCategory(), "CategoryId", "Name");
            return View("EditProduct", product);
        }
        // Метод для обробки запиту на оновлення
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                // Перевірка та завантаження обкладинки, тільки якщо вибрано нове фото
                if (productModel.CoverPhoto != null)
                {
                    string folder = "product/cover/";
                    productModel.CoverImageUrl = await UploadImage(folder, productModel.CoverPhoto);
                }

                // Перевірка та завантаження галереї тільки якщо є нові файли
                if (productModel.GalleryFiles != null && productModel.GalleryFiles.Any())
                {
                    string folder = "product/gallery/";
                    productModel.Gallery ??= new List<GalleryModel>(); // Ініціалізація, якщо галерея відсутня

                    foreach (var file in productModel.GalleryFiles)
                    {
                        var gallery = new GalleryModel
                        {
                            Name = file.FileName,
                            ImageURL = await UploadImage(folder, file)
                        };
                        productModel.Gallery.Add(gallery);
                    }
                }
                else
                {
                    // Завантаження існуючих зображень галереї з бази даних
                    productModel.Gallery = await _productRepository.GetExistingGallery(productModel.ProductId);
                }

                bool isUpdated = await _productRepository.UpdateProduct(productModel);
                if (isUpdated)
                {
                    return RedirectToAction("GetAllProduct", new { isSuccess = true, productId = productModel.ProductId, actionType = "update" });
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося оновити продукт.");
                }
            }

            ViewBag.Category = new SelectList(await _categoryRepository.GetCategory(), "CategoryId", "Name");
            return View("EditProduct", productModel);
        }
		[HttpGet]
		public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var product = await _productRepository.GetProductById(productId); // метод для отримання продукту
            if (product == null)
            {
                return NotFound();
            }

            bool isDeleted = await _productRepository.DeleteProduct(productId); // видалення товару
            if (isDeleted)
            {
                return RedirectToAction("GetAllProduct", new { isSuccess = true, productId, actionType = "delete" });
            }

            return RedirectToAction("GetAllProduct", new { isSuccess = false });
        }




    }
}
