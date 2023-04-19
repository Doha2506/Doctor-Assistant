using Doctor_Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using static Humanizer.On;

namespace Doctor_Assistant.Controllers
{
    
    public class RayController : Controller
    {
        private readonly ILogger<RayController> _logger;
        private DBContext dbContext;
        private IWebHostEnvironment _webHostEnvironment;
        const string DoctorID = "_DoctorID";
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _appEnvironment;

        public RayController(ILogger<RayController> logger, DBContext dBContext,
            IWebHostEnvironment webHostEnvironment, Microsoft.AspNetCore.Hosting.IHostingEnvironment appEnvironment)
        {
            _logger = logger;
            dbContext = dBContext;
            _webHostEnvironment = webHostEnvironment;
            _appEnvironment = appEnvironment;
            

        }
        public bool setTempVariables()
        {
            int? id = @HttpContext.Session.GetInt32("_DoctorID");
            if (id != null)
            {
                TempData["DoctorName"] = new Doctor().GetDoctorNameById(dbContext, id);
                TempData["DoctorDept"] = new Doctor().GetDoctorDeptById(dbContext, id);
                TempData["DoctorId"] = id.ToString();
                return true;
            }
            else
                return false;
        }
        // ---------------- Add Ray ----------------------------
        public IActionResult AddNewBrainTumorPatient()
        {
            if(setTempVariables())
                return View();
            else
                return RedirectToAction("login", "Doctor");
        }
        public IActionResult AddNewAlzehimerPatient()
        {
            if (setTempVariables())
                return View();
            else
                return RedirectToAction("login", "Doctor");
        }
        public IActionResult AddNewDiabeticPatient()
        {
            if (setTempVariables())
                return View();
            else
                return RedirectToAction("login", "Doctor");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(Ray ray, List<IFormFile> imageDate)
        {
            
            setTempVariables();

            Patient patient = new Patient();
            patient.Email = ray.patientEmail;
            patient.Name = ray.patientName;
            new Patient().AddPatient(dbContext, patient);

           int  patientId = new Patient().getPatientIdByEmail(dbContext, patient.Email);

            ray = setPateintRay(ray);

            foreach (var item in imageDate)
            {
                if (item.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await item.CopyToAsync(stream);
                        ray.imageDate = stream.ToArray();
                    }
                }
            }

            var output = "";
            bool result = false;

            if (ray.DiseaseId == 4) // DR disease
            {
                output = await sendDRImage(ray);
                if (output == "Mild" || output == "Moderate" || output == "NO_DR"
                    || output == "Proliferate_DR" || output == "Severe")
                {
                    result = true;
                }
            }
            else if (ray.DiseaseId == 1) // Brain Tumor disease
            {
                output = await sendBrainTumorImage(ray);

                if (output == "Glioma_Tumor" || output == "No_Tumor" || output == "Meningioma_Tumor"
                    || output == "Pituitary_Tumor")
                {
                    result = true;
                }
            }
            else if (ray.DiseaseId == 3) // Alzehimer Tumor disease
            {
                output = await sendAlzehimerImage(ray);

                if (output == "Non_Demented" || output == "Moderate_Demented" || output == "Very_Mild_Demented"
                    || output == "Mild_Demented")
                {
                    result = true;
                }
            }

            if (result == true)
            {
                new Ray().AddRay(dbContext, ray);
                await dbContext.SaveChangesAsync();

                int rayId = new Ray().GetRayByPatientId(dbContext, patientId).Id;

                TempData["patientImage"] = "https://localhost:7298/Ray/ShowPatientRay/"+ rayId;
                TempData["rayPatientName"] = patient.Name;
                TempData["rayPatientEmail"] = patient.Email;
                TempData["rayPatientResult"] = output;

                return RedirectToAction("ShowResult");
            }
            else
            {
                TempData["rayError"] = "Please Enter Clear Image";
                return RedirectToAction("Services", "Home");
            }
        }
       
        private MultipartFormDataContent MakeFormData(Ray ray)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(ray.imageDate), "image", "image.jpg");
            return formData;
        }


        private async Task<string> sendDRImage(Ray ray)
        {
            using var httpClient = new HttpClient();

            var formData = MakeFormData(ray);

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/DiabeticRetinopathy", formData);

            return await GetResponse(response);
        }

        private async Task<string> sendBrainTumorImage(Ray ray)
        {
            using var httpClient = new HttpClient();

            var formData = MakeFormData(ray);

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/BrainTumor", formData);

            return await GetResponse(response);
        }

        private async Task<string> sendAlzehimerImage(Ray ray)
        {
            using var httpClient = new HttpClient();

            var formData = MakeFormData(ray);

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/BrainAlzhemir", formData);

            return await GetResponse(response);
        }

        private async Task<string> GetResponse(HttpResponseMessage response)
        {
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

        private Ray setPateintRay(Ray ray)
        {

            ray.patientId = new Patient().getPatientIdByEmail(dbContext, ray.patientEmail);
            Patient patient = new Patient().GetPatientById(dbContext, ray.patientId);
            ray.doctorId = (int)@HttpContext.Session.GetInt32("_DoctorID");
            ray.result = "";
            ray.patientName = patient.Name;
            ray.patientEmail = patient.Email;

            return ray;
        }
        // ---------------------- Show Result ----------------------------

        public IActionResult ShowResult()
        {
            if (setTempVariables())
                return View();
            else
                return RedirectToAction("login", "Doctor");
        }

        // -------------- Show -------------------------
        public IActionResult ShowBrainTumorPatients()
        {
            if (setTempVariables())
                return View(new Ray().GetAllRaysByDiseaseId(dbContext, 1, (int)@HttpContext.Session.GetInt32("_DoctorID")));
            else
                return RedirectToAction("login", "Doctor");
        }
        public IActionResult ShowAlzehimerPatients()
        {
            if (setTempVariables())
                return View(new Ray().GetAllRaysByDiseaseId(dbContext, 3, (int)@HttpContext.Session.GetInt32("_DoctorID")));
            else
                return RedirectToAction("login", "Doctor");
        }
        public IActionResult ShowDiabeticRetinopathyPatients()
        {
            if (setTempVariables())
                return View(new Ray().GetAllRaysByDiseaseId(dbContext, 4, (int)@HttpContext.Session.GetInt32("_DoctorID")));
            else
                return RedirectToAction("login", "Doctor");
        }

        public IActionResult ShowPatientRay(int id)
        {
            var image = (from m in dbContext.rays
                         where m.Id == id
                         select m.imageDate).FirstOrDefault();

            var stream = new MemoryStream(image.ToArray());

            Response.ContentType = "image/jpeg";

            return File(stream, "image/jpeg", "image.jpg");
        }

        // ------------------ Edit Ray ---------------------
        public IActionResult EditRayPatient(int id)
        {
            Ray patient = new Ray().GetRayById(dbContext, id);
            
            setTempVariables();

            TempData["Image"] = "https://localhost:7298/Ray/ShowPatientRay/"+id;

            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> EditRayPatient(Ray ray, List<IFormFile> imageDate)
        {
            ray = setEmptyFields(ray);

            Patient patient = new Patient();
            patient.Id = ray.patientId;
            patient.Email = ray.patientEmail;
            patient.Name = ray.patientName;
            new Patient().UpdatePatient(dbContext, patient);

            if (imageDate.Count != 0)
            {
                foreach (var item in imageDate)
                {
                    if (item.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await item.CopyToAsync(stream);
                            ray.imageDate = stream.ToArray();
                        }
                    }
                }
            }
            else
            {
               var image = (from m in dbContext.rays
                             where m.Id == ray.Id
                                     select m.imageDate).FirstOrDefault();
                ray.imageDate = image;
            }

            if (ray.result == null)
            {
                ray.result = "";
            }

            new Ray().UpdateRay(dbContext, ray);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Services", "Home");
        }
        private Ray setEmptyFields(Ray ray)
        {
            ray.Id = (int)TempData["rayId"];
            ray.patientId = (int)TempData["patientId"];
            ray.doctorId = (int)TempData["docId"];
            
            return ray;
        }

        // ------------ Delete Ray -----------------------------

        public IActionResult DeleteBrainTumorPatient(int id)
        {
            
            new Ray().DeleteRay(dbContext, id);

            return RedirectToAction("Index", "Home");
        }




    }
}
