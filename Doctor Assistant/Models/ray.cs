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
            //dbContext.SaveChanges();
        }
        public Ray GetRayById(DBContext dbContext, int id)
        {
            return dbContext.rays.Where(x => x.Id.Equals(id)).First();
        }
        public Ray GetRayByPatientId(DBContext dbContext, int patientId)
        {
            return dbContext.rays.Where(x => x.patientId.Equals(patientId)).First();
        }

        public List<RayJoinPatientJoinDoctor> ShowRayPatientsByDieaseId(DBContext dbContext, int diseaseId)
        {
            return FillRaysDetails(dbContext, GetAllRaysByDiseaseId(dbContext, diseaseId));
        }

        private IEnumerable<Ray> GetAllRaysByDiseaseId(DBContext dbContext, int diseaseId)
        {
            return dbContext.rays.Where(x => x.DiseaseId.Equals(diseaseId)).ToList();
        }

        private List<RayJoinPatientJoinDoctor> FillRaysDetails(DBContext dbContext, IEnumerable<Ray> allPatients)
        {
            List<RayJoinPatientJoinDoctor> list = new List<RayJoinPatientJoinDoctor>();

            foreach (var ray in allPatients)
            {
                RayJoinPatientJoinDoctor value = new RayJoinPatientJoinDoctor
                {
                    patient = new Patient(),
                    ray = new Ray(),
                    doctor = new Doctor()
                };

                value.ray = ray;

                value.patient = new Patient().GetPatientById(dbContext, ray.patientId);

                value.doctor = new Doctor().GetDoctorById(dbContext, ray.doctorId);

                list.Add(value);
            }

            return list;
        }
    }
}
