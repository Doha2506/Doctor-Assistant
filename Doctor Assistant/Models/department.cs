namespace Doctor_Assistant.Models
{
    public class department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string getNameById(DBContext dBContext, int Id)
        {
            var department = dBContext.departments.Where(x => x.Id.Equals(Id)).First();
            return department.Name;
        }
    }
}
