using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Numerics;


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
            if(IsEmailExists(doctor))
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

        // ------------- Stroke --------------------

        public IActionResult AddStrokePatient()
        {
            setTempVariables();
            return View();
        }

        [HttpPost]
        public IActionResult AddStrokePatient(PatientJoinStrokeDisease PatientJoinStroke)
        {
            new Patient().addPatient(dbContext, PatientJoinStroke.patient);

            PatientJoinStroke.stroke = setStrokeInfo(PatientJoinStroke.stroke, PatientJoinStroke.patient);

            new StrokeDisease().addNewStroke(dbContext, PatientJoinStroke.stroke);

            return RedirectToAction("Index");
        }

        private StrokeDisease setStrokeInfo(StrokeDisease stroke, Patient patient)
        {
            stroke.PatientId = new Patient().getPatientIdByEmail(dbContext, patient.Email);

            int? id = @HttpContext.Session.GetInt32("_DoctorID");

            stroke.DoctorId = (int)id;

            stroke.Result = "";

            return stroke;

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