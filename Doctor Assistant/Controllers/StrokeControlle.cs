using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Doctor_Assistant.Controllers
{
    public class StrokeController : Controller
    {
        private readonly ILogger<StrokeController> _logger;
        private DBContext dbContext;
        //private DoctorController doctorController = new DoctorController();  

        public StrokeController(ILogger<StrokeController> logger, DBContext dBContext )
        {
            _logger = logger;
            dbContext = dBContext;

        }

        public void setTempVariables()
        {
            int? id = @HttpContext.Session.GetInt32("_DoctorID");
            TempData["DoctorName"] = new Doctor().getDoctorNameById(dbContext, id);
            TempData["DoctorDept"] = new Doctor().getDoctorDeptById(dbContext, id);
        }

        // ------------- Add Stroke --------------------

        public IActionResult AddStrokePatient()
        {
            setTempVariables();
            return View();
        }

        [HttpPost]
        public IActionResult AddStrokePatient(PatientJoinStrokeDisease PatientJoinStroke)
        {
            new Patient().AddPatient(dbContext, PatientJoinStroke.patient);

            PatientJoinStroke.stroke = setStrokeInfo(PatientJoinStroke.stroke, PatientJoinStroke.patient);

            new StrokeDisease().AddNewStroke(dbContext, PatientJoinStroke.stroke);

            return RedirectToAction("Index","Home");
        }

        private StrokeDisease setStrokeInfo(StrokeDisease stroke, Patient patient)
        {
            stroke.PatientId = new Patient().getPatientIdByEmail(dbContext, patient.Email);

            int? id = @HttpContext.Session.GetInt32("_DoctorID");

            stroke.DoctorId = (int)id;

            stroke.Result = "";

            return stroke;

        }

        //--------------- Show Stroke Patient --------------

        public IActionResult ShowStrokePatients()
        {
            setTempVariables();
            return View(new Patient().ShowPatients(dbContext));
        }

        // -------------- Details Of Stroke Patient -----------------
        public IActionResult DetailsOfStrokePatient(int id)
        {
            setTempVariables();
            return View(new Patient().GetPatientDetailsById(dbContext, id));
        }


        // ---------------- Delete Stroke Patient ----------------------
        public IActionResult DeleteStrokePatient(int id)
        {
            StrokeDisease test = new StrokeDisease().GetPatientTestById(dbContext, id);
            new Patient().DeletePatient(dbContext, test.PatientId);
            new StrokeDisease().DeleteStrokeTest(dbContext, test);
            //setTempVariables();
            return RedirectToAction("ShowStrokePatients");
        }

        // ------------------ Edit Stroke Patient ---------------------
        public IActionResult EditStrokePatient(int id)
        {
            StrokeDisease StrokeTest = new StrokeDisease().GetPatientTestById(dbContext, id);
            setTempVariables();
            return View(new Patient().SetPatientDetails(dbContext, StrokeTest));
        }

        [HttpPost]
        public IActionResult EditStrokePatient(PatientJoinStrokeDisease PatientJoinStroke)
        {
            PatientJoinStroke = setEmptyFields(PatientJoinStroke);

            new Patient().UpdatePatient(dbContext, PatientJoinStroke.patient);

            PatientJoinStroke.stroke = ChangeDoctor(PatientJoinStroke);

            new StrokeDisease().UpdateStrokeTest(dbContext, PatientJoinStroke.stroke);

            return RedirectToAction("ShowStrokePatients");
        }
        private StrokeDisease ChangeDoctor(PatientJoinStrokeDisease PatientJoinStroke)
        {
            string doctorEmail = PatientJoinStroke.doctor.Email;

            int doctorId = new Doctor().getDoctorIdByEmail(dbContext, doctorEmail);

            PatientJoinStroke.stroke.DoctorId = doctorId;

            return PatientJoinStroke.stroke;
        }
        

        private PatientJoinStrokeDisease setEmptyFields(PatientJoinStrokeDisease PatientJoinStroke)
        {
            PatientJoinStroke.stroke.Id = (int)TempData["strokeId"];
            PatientJoinStroke.stroke.PatientId = (int)TempData["patientId"];
            PatientJoinStroke.stroke.DoctorId = (int)TempData["docId"];
            PatientJoinStroke.patient.Id = (int)TempData["patientId"];

            return PatientJoinStroke;
        }

        



    }
}