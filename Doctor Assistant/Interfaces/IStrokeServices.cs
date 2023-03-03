using Doctor_Assistant.Models;

namespace Doctor_Assistant.Interfaces
{
    public interface IStrokeServices
    {
        public void AddNewStroke(DBContext dbContext, StrokeDisease stroke);
        public void UpdateStrokeTest(DBContext dbContext, StrokeDisease test);
        public void DeleteStrokeTest(DBContext dbContext, StrokeDisease stroke);
        public IEnumerable<StrokeDisease> GetStrokeTests(DBContext dbContext);
        public StrokeDisease GetPatientTestById(DBContext dbContext , int id);
    }
}
