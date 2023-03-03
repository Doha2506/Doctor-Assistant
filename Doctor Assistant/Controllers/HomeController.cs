using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using static Humanizer.On;

namespace Doctor_Assistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DBContext dbContext;
        const string DoctorID = "_DoctorID";

        public HomeController(ILogger<HomeController> logger,DBContext dBContext)
        {
            _logger = logger;
            dbContext = dBContext;

        }

        public bool setTempVariables()
        {
            int? id = @HttpContext.Session.GetInt32("_DoctorID");
            if (id != null && id != -1)
            {
                TempData["DoctorName"] = new Doctor().GetDoctorNameById(dbContext, id);
                TempData["DoctorDept"] = new Doctor().GetDoctorDeptById(dbContext, id);
                TempData["DoctorId"] = id.ToString();

                return true;
            }
            else
            {
                TempData["DoctorDept"] = "";
                return false;
            }
                
        }

        public IActionResult Index()
        {
           
           setTempVariables();
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Services()
        {
            setTempVariables();
            return View();
        }
        public IActionResult Departments()
        {
            setTempVariables();
            return View();
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}