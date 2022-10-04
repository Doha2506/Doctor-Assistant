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
        public string getDoctorNameByEmail(DBContext dbContext, string email, string password)
        {
            var doctor = dbContext.doctors.Where(x => x.Email.Equals(email) && x.password.Equals(password)).First();
            return doctor.Name;
        }

          
    }
}
