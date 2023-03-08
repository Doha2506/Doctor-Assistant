using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Assistant.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

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

        private IEnumerable<PatientJoinStrokeDisease> FillPatientsList(DBContext dbContext, int doctorId)
        {
            List<PatientJoinStrokeDisease> list = new List<PatientJoinStrokeDisease>();

            var StrokeTests = new StrokeDisease().GetStrokeTests(dbContext, doctorId);


            foreach (var StrokeTest in StrokeTests)
            {
                PatientJoinStrokeDisease StrokePatient = SetPatientDetails(dbContext, StrokeTest);

                list.Add(StrokePatient);
            }

            return list;
        }

        public PatientJoinStrokeDisease SetPatientDetails(DBContext dbContext, StrokeDisease StrokeTest)
        {


            PatientJoinStrokeDisease StrokePatient = new PatientJoinStrokeDisease
            {
                patient = new Patient(),
                stroke = new StrokeDisease(),
            };

            StrokePatient.stroke = StrokeTest;

            StrokePatient.patient.Name = getPatientNameById(dbContext, StrokeTest.PatientId);

            StrokePatient.patient.Email = getPatientEmailById(dbContext, StrokeTest.PatientId);

            return StrokePatient;
        }

        public IEnumerable<PatientJoinStrokeDisease> ShowPatients(DBContext dbContext, int doctorId)
        {
           
            return FillPatientsList(dbContext, doctorId);
        }

        public PatientJoinStrokeDisease GetPatientDetailsById(DBContext dbContext, int id)
        {

            var PatientTest = new StrokeDisease().GetPatientTestById(dbContext, id);

            var PatientDetails = SetPatientDetails(dbContext, PatientTest);

            return PatientDetails;
        }

    }
}
