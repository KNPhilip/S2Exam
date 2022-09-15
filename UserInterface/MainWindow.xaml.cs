using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Entities;
using DataAccess;
using System.Net.Mail;
using System.Linq;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Main
        Repository r;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                r = new();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Fejl under tilgang til databasen {e.Message}", "Opstartsfejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateMediaProducts();
            UpdateReviewers();
            UpdateReviews();
        }
        #endregion

        #region Method: IsEmailValid
        private bool IsEmailValid(string email)
        {
            var valid = true;
            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }
            return valid;
        }
        #endregion

        #region Media Methods

        #region Method: IsMediaInputsValid
        private bool IsMediaInputsValid()
        {
            if (!Medias_Name.Text.All(Char.IsDigit) && Medias_Price.Text.Length > 0)
            {
                string mediaType = Medias_Type.Text;
                if (mediaType == "Streaming" || mediaType == "Spil" || mediaType == "Film")
                {
                    string c = Medias_Price.Text;
                    if (double.TryParse(c, out double value))
                    {
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("Vælg en type");
                }
            }
            return false;
        } 
        #endregion

        #region Method: UpdateMediaProducts
        private void UpdateMediaProducts()
        {
            List<DigitalMediaProduct> digitalMediaProducts = r.GetAllDigitalMediaProducts();
            MediaList.ItemsSource = null;
            MediaList.ItemsSource = digitalMediaProducts;
        }
        #endregion

        #region Method: MediaList_SelectionChanged
        private void MediaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MediaList.SelectedItem != null)
            {
                DigitalMediaProduct selectedDigitalMedia = (DigitalMediaProduct)MediaList.SelectedItem;
                Medias_Name.Text = selectedDigitalMedia.Name;
                Medias_NewName.Text = "";
                Medias_Price.Text = selectedDigitalMedia.Price.ToString();
            }
        }
        #endregion

        #region Method: Medias_Edit_Click
        private void Medias_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (IsMediaInputsValid())
            {
                List<DigitalMediaProduct> digitalMediaProducts = r.GetAllDigitalMediaProducts();
                foreach (DigitalMediaProduct digitalMediaProduct in digitalMediaProducts)
                {
                    if (digitalMediaProduct.Name == Medias_Name.Text.ToString())
                    {
                        if (Medias_NewName.Text.ToString().Length > 0)
                        {
                            digitalMediaProduct.Name = Medias_NewName.Text;
                        }
                        digitalMediaProduct.Price = Convert.ToDouble(Medias_Price.Text);
                        string mediaType = Medias_Type.Text;
                        if (mediaType == "Spil")
                        {
                            digitalMediaProduct.Type = "Game";
                        }
                        else if (mediaType == "Film")
                        {
                            digitalMediaProduct.Type = "Movie";
                        }
                        else
                        {
                            digitalMediaProduct.Type = "Streaming";
                        }
                        r.EditDigitalMediaProduct(digitalMediaProduct);
                        UpdateMediaProducts();
                    }
                }
            }
        } 
        #endregion

        #region Method: Medias_Create_Click
        private void Medias_Create_Click(object sender, RoutedEventArgs e)
        {
            if (IsMediaInputsValid())
            {
                if (Medias_NewName.Text.ToString().Length == 0)
                {
                    bool newName = true;
                    List<DigitalMediaProduct> digitalMediaProducts = r.GetAllDigitalMediaProducts();
                    foreach (DigitalMediaProduct digitalMediaProduct in digitalMediaProducts)
                    {
                        if (digitalMediaProduct.Name == Medias_Name.Text.ToString())
                        {
                            newName = false;
                        }
                    }
                    if (newName == true)
                    {
                        DigitalMediaProduct newDigitalMediaProduct = new();
                        newDigitalMediaProduct.Name = Medias_Name.Text;
                        newDigitalMediaProduct.Price = Convert.ToDouble(Medias_Price.Text);

                        string mediaType = Medias_Type.Text;
                        if (mediaType == "Spil")
                        {
                            newDigitalMediaProduct.Type = "Game";
                        }
                        else if (mediaType == "Film")
                        {
                            newDigitalMediaProduct.Type = "Movie";
                        }
                        else
                        {
                            newDigitalMediaProduct.Type = "Streaming";
                        }

                        r.CreateDigitalMediaProduct(newDigitalMediaProduct);
                        UpdateMediaProducts();
                    }
                }
                else
                {
                    MessageBox.Show("Du kan ikke lave et nyt navn til et ikke-oprettet media project");
                }
            }
        }
        #endregion

        #endregion

        #region Reviewer Methods

        #region Method: IsReviewerInputsValid
        private bool IsReviewerInputsValid()
        {
            if (!Reviewer_Name.Text.All(Char.IsDigit) && IsEmailValid(Reviewer_Mail.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Forkert Navn eller Mail");
            }
            return false;
        }
        #endregion

        #region Method: UpdateReviewers
        private void UpdateReviewers()
        {
            List<Reviewer> reviewers = r.GetAllReviewers(true);
            ReviewerList.ItemsSource = null;
            ReviewerList.ItemsSource = reviewers;
        }
        #endregion

        #region Method: ReviewerList_SelectionChanged

        private void ReviewerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewerList.SelectedItem != null)
            {
                Reviewer selectedReviewer = (Reviewer)ReviewerList.SelectedItem;
                Reviewer_Name.Text = selectedReviewer.Name;
                Reviewer_Mail.Text = selectedReviewer.Mail;
                Reviewer_NewName.Text = "";
            }
        }
        #endregion

        #region Method: Reviewer_Edit_Click
        private void Reviewer_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (IsReviewerInputsValid())
            {
                List<Reviewer> reviewers = r.GetAllReviewers(false);
                foreach (Reviewer reviewer in reviewers)
                {
                    if (reviewer.Name == Reviewer_Name.Text.ToString())
                    {
                        if (Reviewer_NewName.Text.ToString().Length > 0)
                        {
                            reviewer.Name = Reviewer_NewName.Text;
                        }
                        reviewer.Mail = Reviewer_Mail.Text;
                        r.EditReviewer(reviewer);
                        UpdateReviewers();
                    }
                }
            }
        }
        #endregion

        #region Method: Reviewer_Create_Click
        private void Reviewer_Create_Click(object sender, RoutedEventArgs e)
        {
            if (IsReviewerInputsValid())
            {
                if (Reviewer_NewName.Text.ToString().Length == 0)
                {
                    bool newName = true;
                    List<Reviewer> reviewers = r.GetAllReviewers(false);
                    foreach (Reviewer reviewer in reviewers)
                    {
                        if (reviewer.Name == Reviewer_Name.Text.ToString())
                        {
                            newName = false;
                        }
                    }
                    if (newName == true)
                    {
                        Reviewer reviewer = new();
                        reviewer.Name = Reviewer_Name.Text;
                        reviewer.Mail = Reviewer_Mail.Text;

                        r.CreateReviewer(reviewer);
                        UpdateReviewers();
                    }
                }
                else
                {
                    MessageBox.Show("Du kan ikke lave et nyt navn til en ikke-oprettet anmelder");
                }
            }
        }
        #endregion

        #endregion

        #region Review Methods

        #region Method: IsReviewInputsValid
        private bool IsReviewInputsValid()
        {
            bool answer = false;
            List<Reviewer> reviewers = r.GetAllReviewers(true);
            List<DigitalMediaProduct> digitalMediaProducts = r.GetAllDigitalMediaProducts();

            foreach (DigitalMediaProduct digitalMediaProduct in digitalMediaProducts)
            {
                foreach (Reviewer reviewer in reviewers)
                {
                    if (digitalMediaProduct.Id == Convert.ToInt32(Review_DigitalMediaProduct.Text) && reviewer.Id == Convert.ToInt32(Review_Reviewer.Text))
                    {
                        if (Convert.ToInt32(Review_Rate.Text) >= 1 && Convert.ToInt32(Review_Rate.Text) <= 5)
                        {
                            if (Review_Text.Text.Length > 2)
                            {
                                answer = true;
                                goto BREAKLOOPS;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Rate intervallet er 1 - 5");
                        }
                    }
                }
            }
        BREAKLOOPS:
        return answer;
        }
        #endregion

        #region Method: UpdateReviews
        private void UpdateReviews()
        {
            List<Review> reviews = r.GetAllReviews();
            ReviewList.ItemsSource = null;
            ReviewList.ItemsSource = reviews;
        }
        #endregion

        #region Method: ReviewList_SelectionChanged

        private void ReviewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewList.SelectedItem != null)
            {
                Review selectedReview = (Review)ReviewList.SelectedItem;
                Review_Reviewer.Text = selectedReview.Reviewer.Id.ToString();
                Review_DigitalMediaProduct.Text = selectedReview.DigitalMediaProduct.Id.ToString();
                Review_Rate.Text = selectedReview.Rating.ToString();
                Review_Text.Text = selectedReview.Text;
                Review_Date.SelectedDate = selectedReview.Date;
            }
        }
        #endregion

        #region Method: Review_Edit_Click
        private void Review_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (IsReviewInputsValid())
            {
                List<Reviewer> reviewers = r.GetAllReviewers(false);
                List<DigitalMediaProduct> digitalMediaProducts = r.GetAllDigitalMediaProducts();
                List<Review> reviews = r.GetAllReviews();
                foreach (DigitalMediaProduct digitalMediaProduct in digitalMediaProducts)
                {
                    foreach (Reviewer reviewer in reviewers)
                    {
                        if (Convert.ToInt32(Review_DigitalMediaProduct.Text) == digitalMediaProduct.Id && Convert.ToInt32(Review_Reviewer.Text) == reviewer.Id)
                        {
                            foreach (Review review in reviews)
                            {
                                if (review.DigitalMediaProduct.Id == digitalMediaProduct.Id && review.Reviewer.Id == reviewer.Id)
                                {

                                    DateTime earliestDate = new DateTime(1900, 01, 01);
                                    if (Review_Date.SelectedDate > earliestDate)
                                    {
                                        Review editedReview = new();

                                        editedReview.Id = review.Id;
                                        editedReview.Date = Review_Date.SelectedDate.Value;
                                        editedReview.Rating = Convert.ToInt32(Review_Rate.Text);
                                        editedReview.Text = Review_Text.Text;
                                        r.EditReview(editedReview);
                                        UpdateReviews();
                                        goto BREAKLOOPS;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Skift dato");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        BREAKLOOPS:
            return;
        }
        #endregion

        #region Method: Review_Create_Click
        private void Review_Create_Click(object sender, RoutedEventArgs e)
        {
            bool createReview = false;
            if (IsReviewInputsValid())
            {
                if (int.TryParse(Review_DigitalMediaProduct.Text, out int dmp_Id))
                {
                    if (int.TryParse(Review_Reviewer.Text, out int reviewer_Id))
                    {
                        createReview = true;
                        List<Review> reviews = r.GetAllReviews();
                        foreach (Review review in reviews)
                        {
                            if (dmp_Id == review.DigitalMediaProduct.Id && reviewer_Id == review.Reviewer.Id)
                            {
                                createReview = false;
                                goto BREAKLOOP;
                            }
                        }
                    }
                }
                BREAKLOOP:
                if (createReview == true)
                {
                    Review newReview = new();

                    DateTime Date = DateTime.Now;
                    newReview.Date = Date;
                    newReview.Text = Review_Text.Text;
                    newReview.Rating = Convert.ToInt32(Review_Rate.Text);
                    int reviewer_FK = Convert.ToInt32(Review_Reviewer.Text);
                    int media_FK = Convert.ToInt32(Review_DigitalMediaProduct.Text);

                    r.CreateReview(newReview, reviewer_FK, media_FK);
                    UpdateReviews();
                    UpdateReviewers();
                }
                else
                {
                    MessageBox.Show("Den anmelder har alleredet anmeldt mediet");
                }
            }
        }
        #endregion

        #endregion
    }
}
