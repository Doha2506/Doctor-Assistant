using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Doctor_Assistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DBContext dbContext;

        public HomeController(ILogger<HomeController> logger,DBContext dBContext)
        {
            _logger = logger;
            dbContext = dBContext;
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
        public IActionResult ShowDiseases()
        {
            return View();
        }

        public bool IsEmailExists(Doctor doctor)
        {
            if (dbContext.doctors.Any(x => x.Email.Equals(doctor.Email) && x.password.Equals(doctor.password)))
                return true;
            else
                return false;
        }
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult login(Doctor doctor)
        {
            if(IsEmailExists(doctor))
            {
                TempData["DoctorName"] = new Doctor().getDoctorNameByEmail(dbContext, doctor.Email, doctor.password);
                return RedirectToAction("ShowDiseases");
            }
            else
            {
                TempData["loginError"] = "Email or Password is wrong !!";
                return RedirectToAction("login");
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Doctor doctor)
        {
            if (!IsEmailExists(doctor))
            {
                new Doctor().addDoctor(dbContext, doctor);
                return RedirectToAction("login");
            }
            else
            {
                TempData["RegisterError"] = "Email is aleardy Exsists !!";
                return RedirectToAction("Register");
            }
            
        }
    }
}