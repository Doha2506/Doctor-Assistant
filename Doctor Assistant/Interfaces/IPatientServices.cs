using Doctor_Assistant.Models;

namespace Doctor_Assistant.Interfaces
{
    public interface IPatientServices
    {
        public void AddPatient(DBContext dbContext, Patient patient);
        public void DeletePatient(DBContext dbContext, int id);
        public void UpdatePatient(DBContext dbContext, Patient patient);
        public Patient GetPatientById(DBContext dbContext, int id);
        public int getPatientIdByEmail(DBContext dbContext, string email);
        public IEnumerable<Patient> GetPatients(DBContext dbContext);
        public PatientJoinStrokeDisease SetPatientDetails(DBContext dbContext, IDoctorServices doctorServices ,StrokeDisease StrokeTest);
        public IEnumerable<PatientJoinStrokeDisease> ShowPatients(DBContext dbContext,
            IDoctorServices doctorServices, IStrokeServices strokeServices);
        public PatientJoinStrokeDisease GetPatientDetailsById(DBContext dbContext,
                IDoctorServices doctorServices, IStrokeServices strokeServices, int id);

    }
}
