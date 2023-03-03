using Doctor_Assistant.Models;

namespace Doctor_Assistant.Interfaces
{
    public interface IDoctorServices
    {
        public void AddDoctor(DBContext dbContext, Doctor doctor);
        public void UpdateDoctor(DBContext dbContext, Doctor doctor);
        public void DeleteDoctor(DBContext dbContext, int id);
        public int GetDoctorIdByEmail(DBContext dbContext, string email);
        public string GetDoctorNameById(DBContext dbContext, int? Id);
        public string GetDoctorDeptById(DBContext dbContext, int? Id);
        public Doctor GetDoctorById(DBContext dbContext, int? Id);
        public string GetDoctorEmailById(DBContext dbContext, int id);
        public IEnumerable<Doctor> GetDoctors(DBContext dbContext);
        public IEnumerable<DoctorJoinDepartment> ShowDoctors(DBContext dbContext);
        public DoctorJoinDepartment SetDoctorDetails(DBContext dbContext, Doctor doctor);

    }
}
