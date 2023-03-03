using Doctor_Assistant.Interfaces;
using Doctor_Assistant.Models;

namespace Doctor_Assistant.Implementation
{
    public class PatientServices : IPatientServices
    {
        private IDoctorServices _DoctorServices { get; set; }
        private IStrokeServices _strokeServices { get; set; }

        public void AddPatient(DBContext dbContext, Patient patient)
        {
            dbContext.patients.Add(patient);
            dbContext.SaveChanges();
        }

        public void DeletePatient(DBContext dbContext, int id)
        {
            dbContext.Remove(GetPatientById(dbContext, id));
            dbContext.SaveChanges();
        }

        public void UpdatePatient(DBContext dbContext, Patient patient)
        {
            dbContext.patients.Update(patient);
            dbContext.SaveChanges();
        }


        public Patient GetPatientById(DBContext dbContext, int id)
        {
            return dbContext.patients.Where(x => x.Id.Equals(id)).First();
        }

        public int getPatientIdByEmail(DBContext dbContext, string email)
        {
            var patient = dbContext.patients.Where(x => x.Email.Equals(email)).First();
            return patient.Id;

        }

        private string getPatientNameById(DBContext dbContext, int id)
        {
            return dbContext.patients.Where(x => x.Id.Equals(id)).First().Name;
        }

        private string getPatientEmailById(DBContext dbContext, int id)
        {
            return dbContext.patients.Where(x => x.Id.Equals(id)).First().Email;

        }

        public IEnumerable<Patient> GetPatients(DBContext dbContext)
        {
            return dbContext.patients.ToList();
        }


        private IEnumerable<PatientJoinStrokeDisease> FillPatientsList(DBContext dbContext )
        {
            List<PatientJoinStrokeDisease> list = new List<PatientJoinStrokeDisease>();

            var StrokeTests = _strokeServices.GetStrokeTests(dbContext);


            foreach (var StrokeTest in StrokeTests)
            {
                PatientJoinStrokeDisease StrokePatient = SetPatientDetails(dbContext, _DoctorServices, StrokeTest);

                list.Add(StrokePatient);
            }

            return list;
        }

        public PatientJoinStrokeDisease SetPatientDetails(DBContext dbContext,IDoctorServices doctorServices, StrokeDisease StrokeTest)
        {

            _DoctorServices = doctorServices;

            PatientJoinStrokeDisease StrokePatient = new PatientJoinStrokeDisease
            {
                patient = new Patient(),
                stroke = new StrokeDisease(),
                doctor = new Doctor()
            };
            StrokePatient.stroke = StrokeTest;

            StrokePatient.patient.Name = getPatientNameById(dbContext, StrokeTest.PatientId);

            StrokePatient.patient.Email = getPatientEmailById(dbContext, StrokeTest.PatientId);

            StrokePatient.doctor.Id = StrokeTest.DoctorId;

            StrokePatient.doctor.Name = _DoctorServices.GetDoctorNameById(dbContext, StrokeTest.DoctorId);

            StrokePatient.doctor.Email = _DoctorServices.GetDoctorEmailById(dbContext, StrokeTest.DoctorId);

            return StrokePatient;
        }


        public IEnumerable<PatientJoinStrokeDisease> ShowPatients(DBContext dbContext, 
            IDoctorServices doctorServices, IStrokeServices strokeServices)
        {
            _DoctorServices = doctorServices;
            _strokeServices = strokeServices;
            
            return FillPatientsList(dbContext);
        }

        public PatientJoinStrokeDisease GetPatientDetailsById(DBContext dbContext, 
            IDoctorServices doctorServices , IStrokeServices strokeServices,int id)
        {
            
            var PatientTest = strokeServices.GetPatientTestById(dbContext, id);

            var PatientDetails = SetPatientDetails(dbContext, doctorServices, PatientTest);

            return PatientDetails;
        }

    }
}
