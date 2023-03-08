using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class StrokeDisease
    {
        public int Id { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public bool IsMale { get; set; }
        public int Age { get; set; }
        public bool Hypertension { get; set; }
        public bool HeartDisease { get; set; }
        public bool IsMarried { get; set; }
        public string WorkType { get; set; }
        public bool ResidenceType { get; set; }
        public float AvrGlucoseLevel { get; set; }
        public float BMI { get; set; }
        public string SmokingStatus { get; set; }
        public bool Stroke { get; set; }
        public string Result { get; set; }

        public void AddNewStroke(DBContext dbContext, StrokeDisease stroke)
        {
            dbContext.strokeDisease.Add(stroke);
            Update(dbContext);
        }
        public void UpdateStrokeTest(DBContext dbContext, StrokeDisease test)
        {
            dbContext.strokeDisease.Update(test);
            dbContext.SaveChanges();
        }

        public void DeleteStrokeTest(DBContext dbContext, StrokeDisease stroke)
        {
            dbContext.Remove(stroke);
            Update(dbContext);
        }

        private void Update(DBContext dbContext)
        {
            dbContext.SaveChanges();
        }

        public IEnumerable<StrokeDisease> GetStrokeTests(DBContext dbContext, int doctorId)
        {
            return dbContext.strokeDisease.Where(x => x.DoctorId.Equals(doctorId)).ToList();
        }

        public StrokeDisease GetPatientTestById(DBContext dbContext, int id)
        {

            return dbContext.strokeDisease.Where(x => x.Id.Equals(id)).First();
        }
    }
}
