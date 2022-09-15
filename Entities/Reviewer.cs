namespace Entities
{
    public class Reviewer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public List<Review> Reviews { get; set; }
    }
}