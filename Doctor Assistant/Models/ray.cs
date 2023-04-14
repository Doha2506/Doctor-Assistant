using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class Ray
    {
        public int Id { get; set; }

        [ForeignKey("patient")]
        public int patientId { get; set; }

        [ForeignKey("Doctor")]
        public int doctorId { get; set; }

        public string result { get; set; } = string.Empty;

        public string patientEmail { get; set; } = string.Empty;

        public string patientName { get; set; } = string.Empty;

        public byte[] imageDate { get; set; }

        [ForeignKey("Disease")]
        public int DiseaseId { get; set; }

        public IEnumerable<Ray> GetRays(DBContext dbContext)
        {
            return dbContext.rays.ToList();
        }

        public void AddRay(DBContext dbContext, Ray ray)
        {
            dbContext.rays.Add(ray);
        }

        public void DeleteRay(DBContext dbContext, int id)
        {
            dbContext.Remove(GetRayById(dbContext, id));
            dbContext.SaveChanges();
        }

        public void UpdateRay(DBContext dbContext, Ray ray)
        {
            dbContext.rays.Update(ray);
        }

        public Ray GetRayById(DBContext dbContext, int id)
        {
            return dbContext.rays.Where(x => x.Id.Equals(id)).First();
        }

        public List<Ray> GetAllRaysByDiseaseId(DBContext dbContext, int diseaseId, int doctorId)
        {
            return dbContext.rays.Where(x => x.DiseaseId.Equals(diseaseId) && x.doctorId.Equals(doctorId)).ToList();
        }
        public Ray GetRayByPatientId(DBContext dbContext, int patientId)
        {
            return dbContext.rays.Where(x => x.patientId.Equals(patientId)).First();
        }

    }
}
