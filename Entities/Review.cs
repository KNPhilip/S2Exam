namespace Entities
{
    public class Review
    {
        private int rating;

        public int Id { get; set; }
        public int Rating
        {
            get
            {
                return rating;
            }
            set 
            {
                if (rating < 0 || 5 < rating)
                {
                    throw new ArgumentOutOfRangeException("Rate uden for intervallet");
                }
                rating = value;
            }
        }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public Reviewer Reviewer { get; set; }
        public DigitalMediaProduct DigitalMediaProduct { get; set; }
    }
}
