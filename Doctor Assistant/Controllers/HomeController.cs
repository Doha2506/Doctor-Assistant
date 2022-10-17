using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Doctor_Assistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DBContext dbContext;
        private IWebHostEnvironment _webHostEnvironment;
        const string DoctorID = "_DoctorID";


        public HomeController(ILogger<HomeController> logger,DBContext dBContext,IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            dbContext = dBContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

     


        // ----------------------------------


        /*

        public IActionResult AddNewDiagnosisUsingMRI()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewDiagnosisUsingMRI([Bind("Id,ImageFile")] Ray ray)
        {

            if (ModelState.IsValid)
            {
                string wwwRoot = _webHostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(ray.ImageFile.FileName);
                string extension = Path.GetExtension(ray.ImageFile.FileName);
                ray.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRoot + "/Image", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await ray.ImageFile.CopyToAsync(fileStream);
                }

                dbContext.rays.Add(ray);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            
            }

            return View(ray);
        }

        */





    }
}