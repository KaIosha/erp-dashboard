namespace backend.models
{
    public class Departments
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public ICollection<Employees> Employees { get; set; }= new List<Employees>();
    }
}
