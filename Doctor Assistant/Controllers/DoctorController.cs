using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;


namespace Doctor_Assistant.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ILogger<DoctorController> _logger;
        private DBContext dbContext;
        const string DoctorID = "_DoctorID";

        public DoctorController()
        {

        }

        public DoctorController(ILogger<DoctorController> logger, DBContext dBContext)
        {
            _logger = logger;
            dbContext = dBContext;
        }

        // ------------ Register -----------------

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

        public bool IsEmailExists(Doctor doctor)
        {
            if (dbContext.doctors.Any(x => x.Email.Equals(doctor.Email) && x.password.Equals(doctor.password)))
                return true;
            else
                return false;
        }

        // ------------ Login -----------------

        public IActionResult login()
        {
            return View();
        }

        public void setTempVariables()
        {
            int? id = @HttpContext.Session.GetInt32("_DoctorID");
            TempData["DoctorName"] = new Doctor().getDoctorNameById(dbContext, id);
            TempData["DoctorDept"] = new Doctor().getDoctorDeptById(dbContext, id);

        }
        public void setDoctorId(Doctor doctor)
        {
            var id = new Doctor().getDoctorIdByEmail(dbContext, doctor.Email);
            HttpContext.Session.SetInt32(DoctorID, id);
        }

        [HttpPost]
        public IActionResult login(Doctor doctor)
        {
            if (IsEmailExists(doctor))
            {
                setDoctorId(doctor);
                setTempVariables();
                return RedirectToAction("ViewDepartmentDiseases");
            }
            else
            {
                TempData["loginError"] = "Email or Password is wrong !!";
                return RedirectToAction("login");
            }
        }

        public IActionResult ViewDepartmentDiseases()
        {
            if (TempData["DoctorDept"].Equals("Brain"))
            {
                return RedirectToAction("ShowBrainDiseases");
            }
            else if (TempData["DoctorDept"].Equals("Eye"))
            {
                return RedirectToAction("ShowEyeDiseases");
            }
            else
            {
                return RedirectToAction("login");
            }
        }
        public IActionResult ShowBrainDiseases()
        {
            return View();
        }
        public IActionResult ShowEyeDiseases()
        {
            return View();
        }

        // ------------ Logout -----------------

        public IActionResult Logout()
        {
            TempData["DoctorName"] = null;
            TempData["DoctorDept"] = null;

            return RedirectToAction("Index");
        }


    }
}