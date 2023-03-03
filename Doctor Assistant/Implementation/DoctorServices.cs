using Doctor_Assistant.Interfaces;
using Doctor_Assistant.Models;

namespace Doctor_Assistant.Implementation
{
    public class DoctorServices : IDoctorServices
    {
        public void AddDoctor(DBContext dbContext, Doctor doctor)
        {
            dbContext.doctors.Add(doctor);
            dbContext.SaveChanges();
        }

        public void UpdateDoctor(DBContext dbContext, Doctor doctor)
        {
            dbContext.doctors.Update(doctor);
            dbContext.SaveChanges();
        }

        public void DeleteDoctor(DBContext dbContext, int id)
        {
            dbContext.Remove(GetDoctorById(dbContext, id));
            dbContext.SaveChanges();
        }

        public int GetDoctorIdByEmail(DBContext dbContext, string email)
        {
            var doctor = dbContext.doctors.Where(x => x.Email.Equals(email)).First();
            return doctor.Id;
        }

        public string GetDoctorNameById(DBContext dbContext, int? Id)
        {
            var doctor = GetDoctorById(dbContext, Id);
            return doctor.Name;
        }

        public string GetDoctorDeptById(DBContext dbContext, int? Id)
        {
            var doctor = GetDoctorById(dbContext, Id);
            return new Department().getNameById(dbContext, doctor.deptID);

        }
        public Doctor GetDoctorById(DBContext dbContext, int? Id)
        {
            return dbContext.doctors.Where(x => x.Id.Equals(Id)).First();
        }

        public string GetDoctorEmailById(DBContext dbContext, int id)
        {
            return dbContext.doctors.Where(x => x.Id.Equals(id)).First().Email;
        }

        public IEnumerable<Doctor> GetDoctors(DBContext dbContext)
        {
            return dbContext.doctors.ToList();
        }

        public IEnumerable<DoctorJoinDepartment> ShowDoctors(DBContext dbContext)
        {
            return FillDoctorsList(dbContext);
        }

        private IEnumerable<DoctorJoinDepartment> FillDoctorsList(DBContext dbContext)
        {
            List<DoctorJoinDepartment> list = new List<DoctorJoinDepartment>();

            var AllDoctors = GetDoctors(dbContext);

            foreach (var doctor in AllDoctors)
            {
                DoctorJoinDepartment DoctorDepartment = SetDoctorDetails(dbContext, doctor);

                list.Add(DoctorDepartment);
            }

            return list;
        }
        public DoctorJoinDepartment SetDoctorDetails(DBContext dbContext, Doctor doctor)
        {
            DoctorJoinDepartment DoctorDepartment = new DoctorJoinDepartment
            {
                doctor = new Doctor(),
                department = new Department()
            };

            DoctorDepartment.doctor = doctor;

            DoctorDepartment.department.Name = new Department().getNameById(dbContext, doctor.deptID);

            DoctorDepartment.department.Id = doctor.deptID;

            return DoctorDepartment;
        }

    
}
}
