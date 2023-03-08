namespace Doctor_Assistant.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string getNameById(DBContext dBContext, int Id)
        {
            var department = dBContext.departments.Where(x => x.Id.Equals(Id)).First();
            return department.Name;
        }
        public int getIdByName(DBContext dBContext, string name)
        {
            var department = dBContext.departments.Where(x => x.Name.Equals(name)).First();
            return department.Id;
        }
    }
}
