using Doctor_Assistant.Interfaces;
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

        public object ScriptManager { get; private set; }

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
                new Doctor().AddDoctor(dbContext, doctor);
                //_DoctorServices.AddDoctor(dbContext, doctor);
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
        
        public bool setTempVariables()
        {
            int? id = @HttpContext.Session.GetInt32("_DoctorID");
            if (id != null)
            {
                TempData["DoctorName"] = new Doctor().GetDoctorNameById(dbContext, id);
                TempData["DoctorDept"] = new Doctor().GetDoctorDeptById(dbContext, id);
                TempData["DoctorId"] = id.ToString();
                TempData["DoctorId"] = id.ToString();

                return true;
            }
            else
                return false;
        }

        public void setDoctorId(Doctor doctor)
        {
            var id = new Doctor().GetDoctorIdByEmail(dbContext, doctor.Email);

            HttpContext.Session.SetInt32(DoctorID, id);
        }

        [HttpPost]
        public IActionResult login(Doctor doctor)
        {
            if (IsEmailExists(doctor))
            {
                setDoctorId(doctor);
                setTempVariables();
                return RedirectToAction("Services", "Home");
            }
            else
            {
                TempData["loginError"] = "Email or Password is wrong :(";
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
     
        // ------------ Logout -----------------

        public IActionResult Logout()
        {
            TempData["DoctorName"] = null;
            TempData["DoctorDept"] = null;
            TempData["DoctorId"] = null;
            HttpContext.Session.SetInt32(DoctorID, -1);

            return RedirectToAction("Index", "Home");
        }

        //--------------- Show All Doctors ---------------

        public IActionResult ShowAllDoctors()
        {
            if (setTempVariables())
                return View(new Doctor().ShowDoctors(dbContext));
            else
                return RedirectToAction("login", "Doctor"); 

        }

        //------------- Edit Doctor --------------

        public IActionResult EditDoctor(int id)
        {
            setTempVariables();

            if(id == @HttpContext.Session.GetInt32("_DoctorID"))
            {
                return View(new Doctor().GetDoctorById(dbContext, id));

            }
            else
            {
                TempData["VaildateDoctor"] = "You can not edit any doctor except yourself :(";
                return RedirectToAction("ShowAllDoctors");
            }
        }

        [HttpPost]
        public IActionResult EditDoctor(Doctor doctor)
        {
            new Doctor().UpdateDoctor(dbContext, doctor);

            return RedirectToAction("ShowAllDoctors");
        }

        // ---------------- Delete Doctor ----------------------
        public IActionResult DeleteDoctor(int id)
        { 
            new Doctor().DeleteDoctor(dbContext, id);

            if (id == @HttpContext.Session.GetInt32("_DoctorID"))
            {
                Logout();
                return RedirectToAction("Index","Home");
            }
            else
            {
                return RedirectToAction("ShowAllDoctors");
            }

        }

    }
}