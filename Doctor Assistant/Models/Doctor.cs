using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string  Email { get; set; }

        public string password { get; set; }

        [ForeignKey("department")]
        public int deptID { get; set; }


        public void addDoctor(DBContext dbContext, Doctor doctor)
        {
            dbContext.doctors.Add(doctor);
            dbContext.SaveChanges();
        }

        public void UpdateDoctor(DBContext dbContext, Doctor doctor)
        {
            dbContext.doctors.Update(doctor);
            dbContext.SaveChanges();
        }

        public int getDoctorIdByEmail(DBContext dbContext, string email)
        {
            var doctor = dbContext.doctors.Where(x => x.Email.Equals(email)).First();
            return doctor.Id;
        }

        public string getDoctorNameById(DBContext dbContext, int? Id)
        {
            var doctor = GetDoctorById(dbContext, Id);
            return doctor.Name;
        }

        public string getDoctorDeptById(DBContext dbContext, int? Id)
        {
            var doctor = GetDoctorById(dbContext, Id);
            return new department().getNameById(dbContext, doctor.deptID);
             
        }
        private Doctor GetDoctorById(DBContext dbContext, int? Id)
        {
            return dbContext.doctors.Where(x => x.Id.Equals(Id)).First(); 
        }

        public string getDoctorEmailById(DBContext dbContext, int id)
        {
            return dbContext.doctors.Where(x => x.Id.Equals(id)).First().Email;
        }
     

    }
}
