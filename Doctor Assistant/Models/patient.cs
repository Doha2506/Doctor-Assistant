namespace Doctor_Assistant.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public void addPatient(DBContext dbContext, Patient patient)
        {
            dbContext.patients.Add(patient);
            dbContext.SaveChanges();
        }

        public int getPatientIdByEmail(DBContext dbContext, string email)
        {
            var patient = dbContext.patients.Where(x => x.Email.Equals(email)).First();
            return patient.Id;

        }

    }
}
