using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using static Humanizer.On;

namespace Doctor_Assistant.Controllers
{
    public class StrokeController : Controller
    {
        private readonly ILogger<StrokeController> _logger;
        private DBContext dbContext;
       
        public StrokeController(ILogger<StrokeController> logger, DBContext dBContext )
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
                return false;
        }

        // ------------- Add Stroke --------------------

        public IActionResult AddStrokePatient()
        {
            if (setTempVariables())
                return View();
            else
                return RedirectToAction("login", "Doctor");
        }

        [HttpPost]
        public async Task<IActionResult> AddStrokePatient(PatientJoinStrokeDisease PatientJoinStroke)
        {
            new Patient().AddPatient(dbContext, PatientJoinStroke.patient);

            PatientJoinStroke.stroke = setStrokeInfo(PatientJoinStroke.stroke, PatientJoinStroke.patient);
            
            var output = await SendData(PatientJoinStroke.stroke);

            if(output == "Stroke" || output == "No Stroke")
            {
                if (output == "Stroke")
                    PatientJoinStroke.stroke.Stroke = true;
                else
                    PatientJoinStroke.stroke.Stroke = false;

                new StrokeDisease().AddNewStroke(dbContext, PatientJoinStroke.stroke);
                TempData["strokePatientName"] = PatientJoinStroke.patient.Name;
                TempData["strokePatientEmail"] = PatientJoinStroke.patient.Email;
                TempData["strokePatientResult"] = output;

                return RedirectToAction("ShowResult");
            }
            else
            {
                TempData["strokeError"] = "Please Enter Correct Data";
                return RedirectToAction("AddStrokePatient");
            }
           
        }
        
        public async Task<string> SendData(StrokeDisease patient)
        {

            var data = setData(patient);

            var client = new HttpClient();

            var content = new StringContent(JsonConvert.SerializeObject(data));

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(@"http://127.0.0.1:5000/BrainStroke/", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(jsonString);
                var output = result["data"].ToString();
                return output;
                
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(jsonString);
                var output = result["message"].ToString();
                return output;
            }
        }
        private object setData(StrokeDisease patient)
        {
            string gender_Male;
            if (patient.IsMale == true)
            {
                gender_Male = "Male";
            }
            else
            {
                gender_Male = "Female";
            }
            int hypertension;
            if (patient.Hypertension == true)
            {
                hypertension = 1;
            }
            else
            {
                hypertension = 0;
            }
            int heart_disease;
            if (patient.HeartDisease == true)
            {
                heart_disease = 1;
            }
            else
            {
                heart_disease = 0;
            }
            string ever_married = string.Empty;
            if (patient.IsMarried == true)
            {
                ever_married = "Yes";
            }
            else
            {
                ever_married = "No";
            }
            string Residence_type = string.Empty;
            if (patient.ResidenceType == true)
            {
                Residence_type = "Urban";
            }
            else
            {
                Residence_type = "Rural";
            }

            var data = new
            {
                gender = gender_Male,
                age = patient.Age,
                hypertension = hypertension,
                heart_disease = heart_disease,
                ever_married = ever_married,
                work_type = patient.WorkType,
                Residence_type = Residence_type,
                avg_glucose_level = patient.AvrGlucoseLevel,
                bmi = patient.BMI,
                smoking_status = patient.SmokingStatus,
            };
            return data;
        }

        private StrokeDisease setStrokeInfo(StrokeDisease stroke, Patient patient)
        {
            stroke.PatientId = new Patient().getPatientIdByEmail(dbContext, patient.Email);

            int? id = @HttpContext.Session.GetInt32("_DoctorID");

            stroke.DoctorId = (int)id;

            stroke.Result = "";

            return stroke;

        }
        // -------------------------- Show Result ----------------------------

        public IActionResult ShowResult()
        {
            if (setTempVariables())
                return View();
            else
                return RedirectToAction("login", "Doctor");
        }

        //--------------- Show Stroke Patient --------------

        public IActionResult ShowStrokePatients()
        {
            if (setTempVariables())
                return View(new Patient().ShowPatients(dbContext, (int)@HttpContext.Session.GetInt32("_DoctorID")));
            else
                return RedirectToAction("login", "Doctor");
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

            if (PatientJoinStroke.stroke.Result == null)
            {
                PatientJoinStroke.stroke.Result = "";
            }

            new StrokeDisease().UpdateStrokeTest(dbContext, PatientJoinStroke.stroke);

            return RedirectToAction("ShowStrokePatients");
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