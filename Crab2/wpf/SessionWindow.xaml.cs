using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CRABSTUDENT.wpf
{
    /// <summary>
    /// Interaction logic for Session.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        private Session CurrentSession;

        public SessionWindow()
        {
            InitializeComponent();
            
            CurrentSession = (Session) Registry.getInstance().getReference(typeof(Session).FullName);
        }

        private void setUpCurrentSessionValues()
        {
            if(CurrentSession != null)
            {
                SessionStartDate.SelectedDate = Convert.ToDateTime(CurrentSession.Start);
                SessionEndDate.SelectedDate = Convert.ToDateTime(CurrentSession.End);
                SessionTermCount.SelectedItem = CurrentSession.Term;
                SessionYear.SelectedItem = CurrentSession.Year;
            }
        }


        private bool ValidateFields()
        {
            if (!string.IsNullOrWhiteSpace(Common.GetDate(SessionStartDate)) &&
                !string.IsNullOrWhiteSpace(Common.GetDate(SessionEndDate)) &&
                (int)SessionTermCount.SelectedItem > 0 &&
                Convert.ToDateTime(Common.GetDate(SessionStartDate)) < Convert.ToDateTime(Common.GetDate(SessionEndDate)) &&
                (int)SessionYear.SelectedItem > 0)
            {
                return true;
            }
            return false;
            
        }

        private void SaveSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                var client = new RestClient(Common.getConnectionString());

                Session session = new Session();
                session.Start = Common.GetDate(SessionStartDate);
                session.End = Common.GetDate(SessionEndDate);
                session.Term = (int)SessionTermCount.SelectedItem;
                session.Year = (int)SessionYear.SelectedItem;
                session.Is_active = true;

               if (CurrentSession != null)
                {
                    var request = new RestRequest("sessions/{id}/", Method.PUT);
                    Common.addHeaders(request);
                    request.AddUrlSegment("id", CurrentSession.Id.ToString());
                    request.AddJsonBody(session);
                    var asyncHandle = client.ExecuteAsync<Session>(request, response =>
                    {
                        
                    });
                }
                else
                {
                    var request = new RestRequest("sessions/", Method.GET);
                    Common.addHeaders(request);
                    request.AddParameter("term", session.Term.ToString());
                    request.AddParameter("year", session.Year.ToString());
                    var asyncHandle1 = client.ExecuteAsync<List<Session>>(request, response2 =>
                    {
                        if (response2.Data.Count > 0)
                        {
                            MessageBox.Show("There exists an active session similar to this.\n" +
                                    "This session should have a start date later than " + response2.Data[0].End);
                            
                        }
                        else if (response2.Data.Count == 0)
                        {
                            
                            var request2 = new RestRequest("sessions/", Method.POST);
                            Common.addHeaders(request2);
                            request2.AddJsonBody(session);
                            var asyncHandle = client.ExecuteAsync<Session>(request2, response =>
                            {
                                if (!response2.StatusCode.ToString().Equals("BadRequest"))
                                {
                                    MessageBox.Show("A new session has been created successfully");
                                    MessageBoxResult AskToUpdate = MessageBox.Show("All Active Sessions Have been Closed.\nCrab.Student will now restart....", "CRABSTUDENT", MessageBoxButton.OK);
                                    if (AskToUpdate == MessageBoxResult.OK)
                                    {
                                        Application.Current.Shutdown();
                                        System.Windows.Forms.Application.Restart();

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Provided Session Data is Invalid");
                                }
                                
                            });
                        }
                        else
                        {
                            MessageBox.Show("Provided Session Data is Invalid");
                        }
                    });
                }
            }
            else
            {
                MessageBox.Show("Session Form is Invalid\n" +
                    "Check if end date is later than start date and all fields are filled.");
            }
        }

        private void SessionTermCount_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> terms = new List<int>();
            for (int x = 1; x < 4; x++)
            {
                terms.Add(x);
            }

            foreach (int aTerm in terms)
            {
                SessionTermCount.Items.Add(aTerm);
            }

            SessionTermCount.SelectedIndex = 0;
            setUpCurrentSessionValues();
        }

        private void SessionYear_Loaded(object sender, RoutedEventArgs e)
        {
            int cYear = DateTime.Now.Year;

            List<int> years = new List<int>();
            for (int x = cYear; x > cYear-10; x--)
            {
                years.Add(x);
            }

            foreach (int aYear in years)
            {
                SessionYear.Items.Add(aYear);
            }

            SessionYear.SelectedIndex = 0;
            setUpCurrentSessionValues();
        }
    }
}
