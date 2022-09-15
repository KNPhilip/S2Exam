using Microsoft.Data.SqlClient;
using Entities;
using Services;

namespace DataAccess
{
    public class Repository
    {
        private const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Mediareviews;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        #region Constructors
        public Repository()
        {
            SqlConnection connection = new(connectionString);
            connection.Open();
            connection.Close();
        }
        #endregion

        #region Method: GetSQLDateTime
        public string GetSQLDateTime(DateTime date)
        {
            string sqlDate = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return sqlDate;
        }
        #endregion

        #region Method: GetAllDigitalMediaProducts
        public List<DigitalMediaProduct> GetAllDigitalMediaProducts()
        {
            List<DigitalMediaProduct> digitalMediaProducts = new();

            SqlConnection connection = new(connectionString);
            connection.Open();

            string sql = "SELECT * FROM DigitalMediaProducts";
            SqlCommand command = new(sql, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader[0];
                string name = (string)reader[1];
                double price = (double)reader[2];
                string type = (string)reader[3];

                ExchangeRatesWebService rates = new();
                double USDInDKK = rates.GetUSDInDKK();
                double totalprice = Math.Round(price * USDInDKK, 2);

                DigitalMediaProduct dmp = new()
                {
                    Id = id,
                    Name = name,
                    Price = totalprice,
                    Type = type
                };
                digitalMediaProducts.Add(dmp);
            }
            connection.Close();
            return digitalMediaProducts;
        }
        #endregion

        #region Method: GetAllReviewers
        public List<Reviewer> GetAllReviewers(bool GetReview_FK)
        {
            List<Reviewer> reviewers = new();
            SqlConnection connection = new(connectionString);
            connection.Open();

            string sql = "SELECT * FROM Reviewers";
            SqlCommand command = new(sql, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader[0];
                string name = (string)reader[1];
                string mail = (string)reader[2];

                Reviewer reviewer = new()
                {
                    Id = id,
                    Name = name,
                    Mail = mail,
                    Reviews = new()
                };
                reviewers.Add(reviewer);
            }
            if (GetReview_FK == true)
            {
                List<Review> reviews = GetAllReviews();
                AggregateReviews(reviewers, reviews);
            }

            connection.Close();
            return reviewers;
        }
        #endregion

        #region Method: GetAllReviews
        public List<Review> GetAllReviews()
        {
            List<Review> reviews = new();

            SqlConnection connection = new(connectionString);
            connection.Open();
            string sql = "SELECT * FROM Reviews";
            SqlCommand command = new(sql, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader[0];
                int rating = (int)reader[1];
                string text = (string)reader[2];
                DateTime date = (DateTime)reader[3];
                int reviewer_FK = (int)reader[4];
                int digitalMediaProduct_FK = (int)reader[5];

                List<DigitalMediaProduct> digitalMediaProducts = GetAllDigitalMediaProducts();
                DigitalMediaProduct digitalMediaProduct = AggregateDigitalMediaProducts(digitalMediaProducts, digitalMediaProduct_FK);

                List<Reviewer> reviewers = GetAllReviewers(false);
                Reviewer reviewer = AggregateReviewers(reviewers, reviewer_FK);

                Review review = new()
                {
                    Id = id,
                    Rating = rating,
                    Text = text,
                    Date = date,
                    Reviewer = reviewer,
                    DigitalMediaProduct = digitalMediaProduct
                };
                reviews.Add(review);
            }

            connection.Close();
            return reviews;
        }
        #endregion

        #region Aggregate Methods
        private void AggregateReviews(List<Reviewer> reviewers, List<Review> reviews)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            string sql = "SELECT Id,ReviewerId FROM Reviews";
            SqlCommand command = new(sql, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader["Id"];
                int reviewer_FK = (int)reader["ReviewerID"];
                foreach (Reviewer reviewer in reviewers)
                {
                    foreach (Review review in reviews)
                    {
                        if (reviewer.Id == reviewer_FK && id == review.Id)
                        {
                            reviewer.Reviews.Add(review);
                        }
                    }
                }
            }
            connection.Close();
        }
        private DigitalMediaProduct AggregateDigitalMediaProducts(List<DigitalMediaProduct> digitalMediaProducts, int digitalMediaProduct_FK)
        {
            foreach (DigitalMediaProduct digitalMediaProduct in digitalMediaProducts)
            {
                if (digitalMediaProduct.Id == digitalMediaProduct_FK)
                {
                    return digitalMediaProduct;
                }
            }
            return new();
        }

        private Reviewer AggregateReviewers(List<Reviewer> reviewers, int reviewer_FK)
        {
            foreach (Reviewer reviewer in reviewers)
            {
                if (reviewer.Id == reviewer_FK)
                {
                    return reviewer;
                }
            }
            return new();
        }
        #endregion

        #region Method: DeleteReview
        public void DeleteReview(Review review)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();
            string sql = $"DELETE FROM Reviews WHERE Text = '{review.Text}';";

            SqlCommand command = new(sql, connection);
            command.ExecuteReader();
            connection.Close();
        }
        #endregion

        #region Method: EditDigitalMediaProduct
        public void EditDigitalMediaProduct(DigitalMediaProduct digitalMediaProduct)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            ExchangeRatesWebService rates = new();
            double USDInDKK = rates.GetUSDInDKK();
            double totalprice = Math.Round(digitalMediaProduct.Price / USDInDKK, 2);
            string price = string.Empty;
            foreach (char item in totalprice.ToString())
            {
                if (item == ',')
                {
                    price = price + '.';
                }
                else
                {
                    price = price + item;
                }
            }

            string sql = $"UPDATE DigitalMediaProducts SET Name = '{digitalMediaProduct.Name}', Price = {price}, Type = '{digitalMediaProduct.Type}' WHERE Id={digitalMediaProduct.Id};";

            SqlCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        } 
        #endregion

        #region Method: CreateDigitalMediaProduct
        public void CreateDigitalMediaProduct(DigitalMediaProduct digitalMediaProduct)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            ExchangeRatesWebService rates = new();
            double USDInDKK = rates.GetUSDInDKK();
            double totalprice = Math.Round(digitalMediaProduct.Price / USDInDKK, 2);

            string price = string.Empty;
            foreach (char item in totalprice.ToString())
            {
                if (item == ',')
                {
                    price = price + '.';
                }
                else
                {
                    price = price + item;
                }
            }

            string sql = $"INSERT INTO DigitalMediaProducts (Name, Price, Type) VALUES('{digitalMediaProduct.Name}', {price}, '{digitalMediaProduct.Type}');";

            SqlCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        #endregion

        #region Method: EditReviewer
        public void EditReviewer(Reviewer reviewer)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            string sql = $"UPDATE Reviewers SET Name = '{reviewer.Name}', Mail = '{reviewer.Mail}' WHERE Id={reviewer.Id};";

            SqlCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        #endregion

        #region Method: CreateReviewer
        public void CreateReviewer(Reviewer reviewer)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            string sql = $"INSERT INTO Reviewers (Name, Mail) VALUES('{reviewer.Name}', '{reviewer.Mail}');";

            SqlCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        #endregion

        #region Method: EditReview
        public void EditReview(Review review)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            string date = GetSQLDateTime(review.Date);
            string sql = $"UPDATE Reviews SET Rating = {review.Rating}, Text = '{review.Text}', Date = '{date}' WHERE Id={review.Id};";

            SqlCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        #endregion

        #region Method: CreateReview
        public void CreateReview(Review review, int reviewer_FK, int media_FK)
        {
            SqlConnection connection = new(connectionString);
            connection.Open();

            string date = GetSQLDateTime(review.Date);
            string sql = $"INSERT INTO Reviews (Rating, Text, Date, ReviewerId, DigitalMediaProductId) VALUES({review.Rating}, '{review.Text}', '{date}', {reviewer_FK}, {media_FK});";

            SqlCommand command = new(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        #endregion
    }
}