using Doctor_Assistant.Interfaces;
using Doctor_Assistant.Models;

namespace Doctor_Assistant.Implementation
{
    public class StrokeServices
    {
        public void AddNewStroke(DBContext dbContext, StrokeDisease stroke)
        {
            dbContext.strokeDisease.Add(stroke);
            update(dbContext);
        }
        public void UpdateStrokeTest(DBContext dbContext, StrokeDisease test)
        {
            dbContext.strokeDisease.Update(test);
            dbContext.SaveChanges();
        }

        public void DeleteStrokeTest(DBContext dbContext, StrokeDisease stroke)
        {
            dbContext.Remove(stroke);
            update(dbContext);
        }
        private void update(DBContext dbContext)
        {
            dbContext.SaveChanges();
        }

        public IEnumerable<StrokeDisease> GetStrokeTests(DBContext dbContext)
        {
            return dbContext.strokeDisease.ToList();
        }

        public StrokeDisease GetPatientTestById(DBContext dbContext , int id)
        {
            
            return dbContext.strokeDisease.Where(x => x.Id.Equals(id)).First();
        }
    }
}
