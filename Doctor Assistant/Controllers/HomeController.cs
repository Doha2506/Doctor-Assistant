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

       
        
        public IActionResult GetPatientReport(string patientEmail)
        {
            int patientId = new Patient().getPatientIdByEmail(dbContext, patientEmail);

            StrokeDisease  test = HasStroke(patientId);
            List<Ray> rays = HasAnyRayDisease(patientEmail);

            if (test == null)
               test = new StrokeDisease();

            Report report = new Report
            {
                Stroke = test,
                DR = new Ray(),
                BrainTumor = new Ray(),
                Alzehimer = new Ray()

            };

            for (int i=0;i< rays.Count; i++)
            {
                if (rays[i].DiseaseId == 4) // DR
                {
                    report.DR = rays[i];
                }else if(rays[i].DiseaseId == 1) // Brain Tumor
                {
                    report.BrainTumor = rays[i];
                }
                else if(rays[i].DiseaseId == 3) // Alzehimer
                {
                    report.Alzehimer = rays[i];
                }
            }

            return View(report);
        }

      
        private StrokeDisease HasStroke(int patientId)
        {
            return dbContext.strokeDisease.FirstOrDefault(x => x.PatientId.Equals(patientId));
        }
        private List<Ray> HasAnyRayDisease(string patientEmail)
        {
            List<Ray> rays = dbContext.rays
             .Where(x => x.patientEmail == patientEmail)
             .Select(x => new Ray { Id = x.Id, doctorId = x.doctorId, result = x.result,
                 imageDate  = x.imageDate,patientEmail = x.patientEmail, 
                 patientName = x.patientName, DiseaseId = x.DiseaseId })
             .ToList();

            return rays;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}