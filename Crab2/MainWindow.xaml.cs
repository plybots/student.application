using Crab.common;
using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using CRABSTUDENT.wpf;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CRABSTUDENT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //action taken by save menu action
        private static string SAVE_KEY;
        //currently handled student id
        private static int SELECTED_ITEM;
        //selected recipt
        private static Receipt SELECTED_RECEIPT;

        private ObservableCollection<Student> DisplayedStudentsList { get; set; }
        private ObservableCollection<Receipt> StudentReceiptList { get; set; }
        private ObservableCollection<Sponsor> Sponsors { get; set; }

        private ObservableCollection<Sponsor> _Sponsors { get; set; }
        private ObservableCollection<string> subjects { get; set; }

        private ObservableCollection<Receipt> SponsorReceipts { get; set; }

        private ObservableCollection<Student> RecentList { get; set; }

        private List<Student> SelectedStudentList = new List<Student>();

        private TermRegistration mTermRegistration = null;

        private GUISwitch GUISwitch;
        private Sponsor SELECTED_ITEM_SPONSOR;
        private Student SELECTED_ITEM_STUDENT;
        private TermRegistration SELECTED_ITEM_OFFERING;
        private int SELECTED_ITEM_INDEX;
        private Session ACTIVE_SESSION;

        public MainWindow()
        {
            InitializeComponent();

            //create a config file
            //stores the connection url
            Common.createConfigFile();
            //Common.startMySQLServer();
            Common.startWebServer();
            GUISwitch = new GUISwitch(
                StudentScrollViewer,
                RegistrationGrid,
                PaymentsGrid,
                ProgressDialog,
                PayableFeesGrid,
                SponsorsGrid
                );

            new GUISwitch().InitializeGUI(MainContent, LoginGrid);

        }

        //menu session login action
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            
            if (!string.IsNullOrWhiteSpace(username.Text) || !string.IsNullOrWhiteSpace(password.Password))
            {
                //create user object
                User user = new User();
                user.Username = username.Text;
                user.Password = password.Password;

                //get connection
                var client = new RestClient(Common.getConnectionString());

                var request = new RestRequest("auth", Method.POST);
                request.AddJsonBody(user);
                request.AddHeader("Accept", "application/json");
                login.Content = "Loading....";
                var asyncHandle = client.ExecuteAsync<User>(request, response =>
                {
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                try {
                                 
                                    if (response.Data.Status)
                                    {
                                        login.Content = "Login";
                                        LoginGrid.Visibility = Visibility.Collapsed;
                                        MainContent.Visibility = Visibility.Visible;
                                        //add user object to registry
                                        Registry.getInstance().register(response.Data);
                                        try
                                        {
                                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                                            ACTIVE_SESSION = json_serializer.Deserialize<Session>(response.Data.SessionJson);
                                            response.Data.Session = ACTIVE_SESSION;
                                            Registry.getInstance().register(ACTIVE_SESSION, "ACTIVE_SESSION");
                                            //UsernameLabel.Content = response.Data.Username + "\n(" + response.Data.Email + ")";
                                            //CheckForActiveSession();
                                            Title = "CRAB.STUDENT : " + ACTIVE_SESSION.Year + " TERM " + ACTIVE_SESSION.Term + " [ " + ACTIVE_SESSION.Start + " - " + ACTIVE_SESSION.End + "]";
                                            SearchSponsor_Loaded();
                                            StudentsList_Loaded();
#pragma warning disable CS0168 // Variable is declared but never used
                                        }
                                        catch (InvalidOperationException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                        {
                                            MessageBox.Show("No Sessions Loaded");
                                        }
                                    }
                                    else
                                    {
                                        login.Content = "Login";
                                        MessageBox.Show("Invalid username/password");
                                    }
                                #pragma warning disable CS0168 // Variable is declared but never used
                                }
                                catch(NullReferenceException error)
                                #pragma warning restore CS0168 // Variable is declared but never used
                                {
                                    login.Content = "Login";
                                    MessageBox.Show("Connection Error!");
                                }
                        }));
                });
            }
            else
            {
                MessageBox.Show("Invalid username/password");
            }

            
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            //remove user object to registry
            //display login interface
            if (Registry.getInstance().remove(TypeDescriptor.GetClassName(new User())))
            {
                LoginGrid.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                username.Text = "Username";
                password.Password = "12345678";
            }

            
        }

        private void StudentsList_Loaded()
        {
            GUISwitch.ShowProgressDialog();
            var client = new RestClient(Common.getConnectionString());

            var request = new RestRequest("student/", Method.GET);

            Common.addHeaders(request);
            var asyncHandle = client.ExecuteAsync<List<Student>>(request, response =>
            {
                
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                try
                                {
                                    try
                                    {
                                        GUISwitch.ShowStudentsList();
                                        DisplayedStudentsList = new ObservableCollection<Student>();
                                        StudentsList.ItemsSource = DisplayedStudentsList;
                                        int count = 1;
                                        foreach (Student student in response.Data)
                                        {
                                            student.Count = count;
                                            var TermRegRequest = new RestRequest("terms/", Method.GET);
                                            Common.addHeaders(TermRegRequest);
                                            TermRegRequest.AddParameter("student", student.Id.ToString());
                                            client.ExecuteAsync<List<TermRegistration>>(TermRegRequest, TermRegResponse =>
                                            {
                                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                 new ThreadStart(delegate
                                                 {
                                                     try
                                                     {
                                                         TermRegistration termReg = TermRegResponse.Data[0];
                                                         student.Student_class = termReg.Student_class;
            #pragma warning disable CS0168 // Variable is declared but never used
                                                     }
                                                     catch (Exception er)
            #pragma warning restore CS0168 // Variable is declared but never used
                                                     {

                                                     }
                                                     DisplayedStudentsList.Add(student);
                                                 }));
                                         
                                            });
                                            count++;
                                            if (count == 20)
                                                break;

                                        }

                                    }
#pragma warning disable CS0168 // Variable is declared but never used
                                    catch (ArgumentOutOfRangeException exc)
#pragma warning restore CS0168 // Variable is declared but never used
                                    {
                                        MessageBox.Show("No currently registered Students");
                                    }
                                    
                                }
                                catch (Exception ex) when (ex is ArgumentNullException || ex is NullReferenceException)
                                {
                                    ErrorNote.Visibility = Visibility.Visible;
                                    ErrorNote.Content = "Ooops!!! An error ocuured.\n\nThis is possible if sessions information is not available.\nClose application and start it again.";
                                    MenuPanel.IsEnabled = false;
                                    StudentsList.IsEnabled = false;
                                }    
                            
                            }));
            });

        }

        private void NewStudentToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            GUISwitch.ShowRegistrationGrid();
            RegistrationGrid_Loaded();
            generateStudentNumber();
            
        }

        private void RegistrationGrid_Loaded()
        {
            SAVE_KEY = "new student";

            //setup combo box content

            subjects = new ObservableCollection<string>();
            subjects.Add("ALL");
            SubjectsComboBox.ItemsSource = subjects;
            SubjectsComboBox.SelectedIndex = 0;

            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sponsor/", Method.GET);
            Common.addHeaders(request);
            var asyncHandle = client.ExecuteAsync<List<Sponsor>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                try {
                                    Sponsors = new ObservableCollection<Sponsor>();
                                    Sponsors.Clear();
                                    response.Data.ToList().ForEach(Sponsors.Add);
                                    SponsorComboBox.ItemsSource = Sponsors;
                                    SponsorComboBox.SelectedIndex = 0;
#pragma warning disable CS0168 // Variable is declared but never used
                                }
                                catch (ArgumentNullException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                            }));
            });
            

            List<int> classes = new List<int>();
            for(int x = 1; x < 7; x++)
            {
                classes.Add(x);
            }

            StudentClassComboBox.Items.Clear();
            foreach (int aClass in classes)
            {
                StudentClassComboBox.Items.Add(aClass);
            }

            StudentClassComboBox.SelectedIndex = 0;

            
        }

        private void MenuSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(Common.getConnectionString());

            switch (SAVE_KEY)
            {
                case "new student":
                   if (!string.IsNullOrWhiteSpace(StudentNameTextBox.Text) && 
                        !string.IsNullOrWhiteSpace(StudentNoTextBox.Text) &&
                        !string.IsNullOrWhiteSpace(StudentFeesOffer.Text)
                        )
                    {
                        try
                        {
                            Student student = new Student();
                            student.Student_no = Convert.ToInt32(StudentNoTextBox.Text);
                            student.Student_name = StudentNameTextBox.Text;
                            student.Student_class = (int)StudentClassComboBox.SelectedItem;
                            student.Subjects = (string)SubjectsComboBox.SelectedItem;
                            student.Sponsor = ((Sponsor)SponsorComboBox.SelectedItem).Id;
                            student.Fees_offer = Convert.ToInt32(StudentFeesOffer.Text);

                            //MessageBox.Show(student.Student_name.ToString());
                            var request = new RestRequest("student/", Method.POST);
                            request.AddJsonBody(student);

                            Common.addHeaders(request);
                            //MessageBox.Show(new JavaScriptSerializer().Serialize(student));

                            var asyncHandle = client.ExecuteAsync<Student>(request, response =>
                            {
                                Application.Current.Dispatcher.Invoke(
                                        DispatcherPriority.Background,
                                        new ThreadStart(
                                            delegate
                                            {

                                                try
                                                {
                                                    student = response.Data;
                                                    StudentNoReg.Content = student.Student_no.ToString();
                                                    StudentNameReg.Content = student.Student_name;
                                                    StudentClassReg.Content = student.Student_class.ToString();
                                                    var SponsorRequest = new RestRequest("sponsor/{id}/", Method.GET);
                                                    Common.addHeaders(SponsorRequest);
                                                    SponsorRequest.AddUrlSegment("id", student.Sponsor.ToString());
                                                    client.ExecuteAsync<Sponsor>(SponsorRequest, SponsorResponse =>
                                                    {
                                                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                            new ThreadStart(delegate
                                                            {
                                                                StudentSponsorReg.Content = SponsorResponse.Data.Name;
                                                                student.SponsorName = SponsorResponse.Data.Name;
                                                                SelectedStudentList.Clear();
                                                                SelectedStudentList.Add(student);

                                                                if (SelectedStudentList.Count > 0)
                                                                {
                                                                    SELECTED_ITEM = SelectedStudentList[0].Id;
                                                                    SELECTED_ITEM_INDEX = DisplayedStudentsList.IndexOf(SelectedStudentList[0]);
                                                                }
                                                                    

                                                            }));
                                                    });
                                                    SubjectsReg.Content = student.Subjects;
                                                    StudentFeesOfferReg.Content = student.Fees_offer.ToString();
                                                    generateStudentNumber();
                                                    
                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                catch (NullReferenceException ex)
                                                {
#pragma warning restore CS0168 // Variable is declared but never used

                                                    MessageBox.Show("Invalid Student Data");
                                                }
                                            }));
                            });
                        }
#pragma warning disable CS0168 // Variable is declared but never used
                        catch (FormatException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                        {
                            MessageBox.Show("Invalid Student Data");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Student Data");
                    }
                    break;
            }
        }

        private void StudentClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            generateStudentNumber();
            try {
                if (((int)StudentClassComboBox.SelectedItem) < 5)
                {
                    subjects.Clear();
                    subjects.Add("ALL");
                    SubjectsComboBox.SelectedIndex = 0;
                } else if (((int)StudentClassComboBox.SelectedItem) > 4)
                {
                    subjects.Clear();
                    subjects.Add("ARTS");
                    subjects.Add("SCIENCES");
                    SubjectsComboBox.SelectedIndex = 0;
                }
#pragma warning disable CS0168 // Variable is declared but never used
            }
            catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used

        }

        private void generateStudentNumber()
        {
            if (RegistrationGrid.IsVisible)
            {
                try
                {

                    //generate student number;
                    var client = new RestClient(Common.getConnectionString());

                    var request = new RestRequest("student_number/", Method.GET);
                    Common.addHeaders(request);
                    client.ExecuteAsync<int>(request, response =>
                    {
                        Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Background,
                                new ThreadStart(
                                    delegate
                                    {
                                        StudentNoTextBox.Text = response.Data.ToString();
                                    }));
                    });
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (NullReferenceException es)
#pragma warning restore CS0168 // Variable is declared but never used
                {

                }

            }

        }

        private void SearchClass_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> classes = new List<int>();

            for (int x = 1; x < 7; x++)
            {
                classes.Add(x);
            }
            SearchClass.Items.Clear();
            SearchClass.Items.Add("CLASS");

            foreach (int aClass in classes)
            {
                SearchClass.Items.Add(aClass);
            }

            SearchClass.SelectedIndex = 0;
        }
        
        private void SearchOffering_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> offerings = new List<string>();
            offerings.Add("OFFERING");
            offerings.Add("DAY");
            offerings.Add("BOARDING");

            SearchOffering.Items.Clear();
            foreach (string offer in offerings)
            {
                SearchOffering.Items.Add(offer);
            }

            SearchOffering.SelectedIndex = 0;
        }

        private void SearchSubjects_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> subjects = new List<string>();
            subjects.Add("SUBJECTS");
            subjects.Add("ALL");
            subjects.Add("SCIENCES");
            subjects.Add("ARTS");

            SearchSubjects.Items.Clear();

            foreach (string subject in subjects)
            {
                SearchSubjects.Items.Add(subject);
            }

            SearchSubjects.SelectedIndex = 0;
        }

        private void SearchSponsor_Loaded()
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sponsor/", Method.GET);
            Common.addHeaders(request);
            var asyncHandle = client.ExecuteAsync<List<Sponsor>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                try {
                                    Sponsors = new ObservableCollection<Sponsor>();
                                    Sponsors.Clear();
                                    Sponsor dummy = new Sponsor();
                                    dummy.Name = "SPONSOR";
                                    Sponsors.Add(dummy);
                                    response.Data.ToList().ForEach(Sponsors.Add);
                                    SearchSponsor.ItemsSource = Sponsors;
                                    SearchSponsor.SelectedIndex = 0;
#pragma warning disable CS0168 // Variable is declared but never used
                                }
                                catch (ArgumentNullException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                            }));
            });
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSearchVisible())
            {
                var client = new RestClient(Common.getConnectionString());
                Student student = new Student();

                if (!string.IsNullOrWhiteSpace(SearchName.Text) && !SearchName.Text.Equals("Student Name"))
                {
                    student.Student_name = SearchName.Text;
                }
                if (!string.IsNullOrWhiteSpace(SearchNo.Text) && !SearchNo.Text.Equals("Student Number"))
                {
                    try
                    {
                        student.Student_no = Convert.ToInt32(SearchNo.Text);
                    }

#pragma warning disable CS0168 // Variable is declared but never used
                    catch (FormatException exp)
#pragma warning restore CS0168 // Variable is declared but never used
                    {

                    }

                }

                if (SearchClass.SelectedIndex != 0)
                    student.Student_class = (int)SearchClass.SelectedItem;
                if (SearchSubjects.SelectedIndex != 0)
                    student.Subjects = (string)SearchSubjects.SelectedItem;
                if (SearchSponsor.SelectedIndex != 0)
                    student.Sponsor = ((Sponsor)SearchSponsor.SelectedItem).Id;
                if (SearchOffering.SelectedIndex != 0)
                    student.Offering = (string)SearchOffering.SelectedItem;

                var SearchRequest = new RestRequest("search", Method.POST);
                Common.addHeaders(SearchRequest);
                SearchRequest.AddJsonBody(student);
                client.ExecuteAsync<List<Student>>(SearchRequest, StudentResponse =>
                {
                    Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new ThreadStart(
                                delegate
                                {
                                    if (StudentResponse.Data.Count > 0)
                                    {
                                        //MessageBox.Show(new JavaScriptSerializer().Serialize(response.Data));
                                        DisplayedStudentsList = new ObservableCollection<Student>();
                                        GUISwitch.ShowStudentsList();
                                        DisplayedStudentsList.Clear();
                                        string search_string = "Listing : ";

                                        if (!string.IsNullOrWhiteSpace(SearchName.Text) && !SearchName.Text.Equals("Student Name"))
                                            search_string += "Name( " + SearchName.Text + " ); ";
                                        else search_string += "Name( * ); ";

                                        if (!string.IsNullOrWhiteSpace(SearchNo.Text) && !SearchNo.Text.Equals("Student Number"))
                                            search_string += "Number: ( " + SearchNo.Text + " ); ";
                                        else search_string += "Number: ( * ); ";

                                        if (SearchClass.SelectedIndex != 0)
                                            search_string += "Class ( " + student.Student_class + " ); ";
                                        else search_string += "Class ( * ); ";

                                        if (SearchSubjects.SelectedIndex != 0)
                                            search_string += "Subjects ( "+(string)SearchSubjects.SelectedItem+" ); ";
                                        else search_string += "Subjects ( * ); ";

                                        if (SearchSponsor.SelectedIndex != 0)
                                            search_string += "Sponsor ( "+((Sponsor)SearchSponsor.SelectedItem).Name+" ); ";
                                        else search_string += "Sponsor ( * ); ";

                                        if (SearchOffering.SelectedIndex != 0)
                                            search_string += "Offering ( "+(string)SearchOffering.SelectedItem+" ) ";
                                        else search_string += "Offering ( * ) ";

                                        if (SearchWithBalance.IsChecked.Value)
                                            search_string += "With Any Tuition Balance";

                                        if (SearchNoBalance.IsChecked.Value)
                                            search_string += "With No Tuition Balance";

                                        SearchString.Content = search_string;
                                        ToggleSearchView();
                                        SearchTuitionBalance.Visibility = Visibility.Collapsed;

                                        
                                        int count = 1;
                                        foreach (Student _student in StudentResponse.Data)
                                        {
                                            if (SearchWithBalance.IsChecked.Value || SearchNoBalance.IsChecked.Value)
                                            {
                                                var BalanceRequest = new RestRequest("get_tuition_balance/{id}/", Method.GET);
                                                Common.addHeaders(BalanceRequest);
                                                BalanceRequest.AddUrlSegment("id", _student.Id.ToString());
                                                client.ExecuteAsync<int>(BalanceRequest, BalanceResponse =>
                                                {
                                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(
                                                        delegate
                                                        {
                                                              if (BalanceResponse.Data < 0)
                                                              {
                                                                  if (SearchWithBalance.IsChecked.Value)
                                                                  {
                                                                      _student.Count = count;
                                                                      DisplayedStudentsList.Add(_student);
                                                                      count++;
                                                                  }
                                                              }
                                                              if (BalanceResponse.Data >= 0)
                                                              {
                                                                  if (SearchNoBalance.IsChecked.Value)
                                                                  {
                                                                      _student.Count = count;
                                                                      DisplayedStudentsList.Add(_student);
                                                                      count++;
                                                                  }
                                                              }
                                                        }));
                                                });
                                            }
                                            else
                                            {
                                                _student.Count = count;
                                                DisplayedStudentsList.Add(_student);
                                                count++;
                                            }
                                        }
                                        StudentsList.ItemsSource = DisplayedStudentsList;
                                    }
                                    else
                                    {
                                        MessageBox.Show("No Matches for Specified Query");
                                    }
                                }));
                });
            }
            else
            {
                ToggleSearchView();
            }
        }
        
        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStudentList.Clear();

            foreach (Student student in StudentsList.SelectedItems)
            {
                SelectedStudentList.Add(student);
            }

            if (SelectedStudentList.Count > 0)
                SELECTED_ITEM = SelectedStudentList[0].Id;
        }

        private void MenuNewPaymentOption_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudentList.Count == 1)
            {
                var client = new RestClient(Common.getConnectionString());
                var request = new RestRequest("terms/", Method.GET);
                Common.addHeaders(request);
                request.AddParameter("student", SELECTED_ITEM.ToString());
                var asyncHandle = client.ExecuteAsync<List<TermRegistration>>(request, response =>
                {
                    Application.Current.Dispatcher.Invoke(
                           DispatcherPriority.Background,
                           new ThreadStart(
                               delegate
                               {
                                   if(response.Data.Count > 0)
                                   {
                                       var request2 = new RestRequest("sessions/", Method.GET);
                                       Common.addHeaders(request2);
                                       request2.AddParameter("now", "true");
                                       client.ExecuteAsync<List<Session>>(request2, response2 =>
                                       {
                                           var RecentSessionRequest = new RestRequest("recent_session/", Method.GET);
                                           Common.addHeaders(RecentSessionRequest);
                                           client.ExecuteAsync<Session>(RecentSessionRequest, RecentsessionResponse =>
                                           {
                                               Application.Current.Dispatcher.Invoke(
                                                  DispatcherPriority.Background,
                                                  new ThreadStart(
                                                      delegate
                                                      {
                                                          Registry.getInstance().register(response2.Data[0]);
                                                          Registry.getInstance().register(RecentsessionResponse.Data, "RECENT_SESSION");
                                                          object ST = SELECTED_ITEM;
                                                          Registry.getInstance().register(ST, "STUDENT");

                                                          ReceiptWindow window = new ReceiptWindow();
                                                          window.Owner = this;
                                                          window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                                          window.Closed += new EventHandler(ReceiptWindowClosed);
                                                          window.ShowDialog();


                                                      }));
                                           });
                                       });
                                       
                                   }
                                   else
                                   {
                                       MessageBoxResult AcceptEdit = MessageBox.Show("Student term registration not completed,\n" +
                                       "Do you wish to complete the registration now?", "Term Registration", MessageBoxButton.YesNo);
                                       if (AcceptEdit == MessageBoxResult.Yes)
                                       {
                                           StudentTermReg_Click(sender, e);
                                       }
                                       
                                   }
                               }));
                });

                /**/
            }
                
            else
                MessageBox.Show("Exactly One(1) student should be selected to use this function");
        }

        private void SessionWindowClosed(object sender, EventArgs e)
        {
            SearchSponsor_Loaded();
            StudentsList_Loaded();
        }
        
        private void ReceiptWindowClosed(object sender, EventArgs e)
        {
            ReadPayments();
        }

        private void MenuReadPaymentOption_Click(object sender, RoutedEventArgs e)
        {
            ReadPayments();
        }

        public void ReadPayments()
        {
            if (SelectedStudentList.Count == 1)
            {
                try
                {
                    StudentReceiptList.Clear();
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used


                
                var client = new RestClient(Common.getConnectionString());
                GUISwitch.ShowProgressDialog();
                var TermRegRequest0 = new RestRequest("terms/", Method.GET);
                Common.addHeaders(TermRegRequest0);
                TermRegRequest0.AddParameter("student", SELECTED_ITEM.ToString());
                client.ExecuteAsync<List<TermRegistration>>(TermRegRequest0, TermRegResponse0 =>
                {
                    if (TermRegResponse0.Data.Count <= 0)
                    {
                        var TermRegRequest2 = new RestRequest("forward_reg/{student_id}/", Method.GET);
                        Common.addHeaders(TermRegRequest2);
                        TermRegRequest2.AddUrlSegment("student_id", SELECTED_ITEM.ToString());
                        client.ExecuteAsync<TermRegistration>(TermRegRequest2, TermRegResponse2 =>
                        {
                            try
                            {
                                int tId = TermRegResponse2.Data.Id;
                                readAllPayments();
#pragma warning disable CS0168 // Variable is declared but never used
                            }
                            catch (NullReferenceException e)
#pragma warning restore CS0168 // Variable is declared but never used
                            {
                                try
                                {
                                    if ((int)((object)TermRegResponse2.Data) == 1)
                                    {
                                        MessageBox.Show("Student has reached maximum possible registrations.\n" +
                                            "Student could have just graduated from S.6.");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Automatic Student Registration Failed,\nTry Manual Registration");
                                    }
#pragma warning disable CS0168 // Variable is declared but never used
                                }
                                catch (Exception ep)
#pragma warning restore CS0168 // Variable is declared but never used
                                {
                                    MessageBox.Show("Automatic Student Registration Failed,\nTry Manual Registration");
                                }
                                
                            }

                        });
                    }
                    else
                    {
                        readAllPayments();
                    }

                });

                
                
            }
            else
            {
                MessageBox.Show("Exactly One(1) student should be selected to use this function");
            }
        }

        private void readAllPayments()
        {
            
            var client = new RestClient(Common.getConnectionString());
            var StudentRequest = new RestRequest("student/{id}/", Method.GET);
            Common.addHeaders(StudentRequest);
            StudentRequest.AddUrlSegment("id", SELECTED_ITEM.ToString());
            client.ExecuteAsync<Student>(StudentRequest, StudentResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                Student student = StudentResponse.Data;
                                SELECTED_ITEM_STUDENT = StudentResponse.Data;
                                StudentNoPay.Content = student.Student_no.ToString();
                                StudentNamePay.Content = student.Student_name;

                                SubjectsPay.Content = student.Subjects;
                                Sponsor sponsor = new Sponsor();
                                TermPayableFees termFees = new TermPayableFees();
                                TermClassAdditionalFees termAdditionalFees = new TermClassAdditionalFees();
                                TermClassAdditionalFees BAdditionalFees = new TermClassAdditionalFees();
                                ComputerPayableFees computerFees = new ComputerPayableFees();
                                TermRegistration termReg = new TermRegistration();

                                var TermPayableFeesRequest = new RestRequest("term_payable_fees/", Method.GET);
                                TermPayableFeesRequest.AddQueryParameter("session", ACTIVE_SESSION.Id.ToString());
                                Common.addHeaders(TermPayableFeesRequest);
                                client.ExecuteAsync<List<TermPayableFees>>(TermPayableFeesRequest, TermPayableFeesResponse =>
                                {
                                    if (TermPayableFeesResponse.Data.Count > 0)
                                    {
                                        Application.Current.Dispatcher.Invoke(
                                            DispatcherPriority.Background,
                                            new ThreadStart(
                                                delegate
                                                {
                                                    termFees = TermPayableFeesResponse.Data[0];

                                                    var TermAdditionalFeesRequest = new RestRequest("term_class_additional_fees/", Method.GET);
                                                    TermAdditionalFeesRequest.AddQueryParameter("offering", false.ToString());
                                                    TermAdditionalFeesRequest.AddQueryParameter("term_fees", termFees.Id.ToString());
                                                    Common.addHeaders(TermAdditionalFeesRequest);
                                                    client.ExecuteAsync<List<TermClassAdditionalFees>>(TermAdditionalFeesRequest, TermAdditionalFeesResponse =>
                                                    {
                                                        Application.Current.Dispatcher.Invoke(
                                                                DispatcherPriority.Background,
                                                                new ThreadStart(
                                                                    delegate
                                                                    {
                                                                        try
                                                                        {
                                                                            termAdditionalFees = TermAdditionalFeesResponse.Data[0];
                                                                        }
                                                                        catch (IndexOutOfRangeException ex)
                                                                        {
                                                                            MessageBox.Show("Incomplete payable fees records");
                                                                        }

                                                                        var BAdditionalFeesRequest = new RestRequest("term_class_additional_fees/", Method.GET);
                                                                        BAdditionalFeesRequest.AddQueryParameter("offering", true.ToString());
                                                                        BAdditionalFeesRequest.AddQueryParameter("term_fees", termFees.Id.ToString());
                                                                        Common.addHeaders(BAdditionalFeesRequest);
                                                                        client.ExecuteAsync<List<TermClassAdditionalFees>>(BAdditionalFeesRequest, BAdditionalFeesResponse =>
                                                                        {
                                                                            Application.Current.Dispatcher.Invoke(
                                                                                    DispatcherPriority.Background,
                                                                                    new ThreadStart(
                                                                                        delegate
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                BAdditionalFees = BAdditionalFeesResponse.Data[0];
                                                                                            }
                                                                                            catch (IndexOutOfRangeException ex)
                                                                                            {
                                                                                                MessageBox.Show("Incomplete payable fees records");
                                                                                            }

                                                                                            var ComputerFeesRequest = new RestRequest("computer/{id}/", Method.GET);
                                                                                            Common.addHeaders(ComputerFeesRequest);
                                                                                            ComputerFeesRequest.AddUrlSegment("id", termFees.ComputerPayableFees.ToString());
                                                                                            client.ExecuteAsync<ComputerPayableFees>(ComputerFeesRequest, ComputerFeesResponse =>
                                                                                            {
                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                        DispatcherPriority.Background,
                                                                                                        new ThreadStart(
                                                                                                            delegate
                                                                                                            {
                                                                                                                computerFees = ComputerFeesResponse.Data;

                                                                                                                var TermRegRequest = new RestRequest("terms/", Method.GET);
                                                                                                                Common.addHeaders(TermRegRequest);
                                                                                                                TermRegRequest.AddParameter("student", student.Id.ToString());
                                                                                                                client.ExecuteAsync<List<TermRegistration>>(TermRegRequest, TermRegResponse =>
                                                                                                                {
                                                                                                                    try
                                                                                                                    {

                                                                                                                        Application.Current.Dispatcher.Invoke(
                                                                                                                            DispatcherPriority.Background,
                                                                                                                            new ThreadStart(
                                                                                                                                delegate
                                                                                                                                {
                                                                                                                                    
                                                                                                                                    if (TermRegResponse.Data.Count > 0)
                                                                                                                                    {
                                                                                                                                        termReg = TermRegResponse.Data[0];

                                                                                                                                        StudentClassPay.Content = termReg.Student_class.ToString();
                                                                                                                                        StudentFeesOfferPay.Content = termReg.Fees_offer.ToString();

                                                                                                                                        var SponsorRequest = new RestRequest("sponsor/{id}/", Method.GET);
                                                                                                                                        Common.addHeaders(SponsorRequest);
                                                                                                                                        SponsorRequest.AddUrlSegment("id", termReg.Sponsor.ToString());
                                                                                                                                        client.ExecuteAsync<Sponsor>(SponsorRequest, SponsorResponse =>
                                                                                                                                        {
                                                                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                                                                    DispatcherPriority.Background,
                                                                                                                                                    new ThreadStart(
                                                                                                                                                        delegate
                                                                                                                                                        {
                                                                                                                                                            sponsor = SponsorResponse.Data;
                                                                                                                                                            SELECTED_ITEM_SPONSOR = SponsorResponse.Data;
                                                                                                                                                            StudentSponsorPay.Content = SponsorResponse.Data.Name;

                                                                                                                                                            StudentBalanceCalculator balance_calc = new StudentBalanceCalculator(
                                                                                                                                                            student, sponsor, termAdditionalFees, BAdditionalFees, termFees, computerFees, termReg);

                                                                                                                                                            SELECTED_ITEM_OFFERING = TermRegResponse.Data[0];
                                                                                                                                                            offeringTypePay.Content = TermRegResponse.Data[0].Offering;
                                                                                                                                                            mTermRegistration = TermRegResponse.Data[0];

                                                                                                                                                            var StudentTuitionRequest = new RestRequest("tuition/", Method.GET);
                                                                                                                                                            StudentTuitionRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                            Common.addHeaders(StudentTuitionRequest);
                                                                                                                                                            client.ExecuteAsync<List<TuitionFees>>(StudentTuitionRequest, StudentTuitionResponse =>
                                                                                                                                                            {
                                                                                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                                                                                            DispatcherPriority.Background,
                                                                                                                                                                            new ThreadStart(
                                                                                                                                                                                delegate
                                                                                                                                                                                {
                                                                                                                                                                                    TuitionFees tuition = new TuitionFees();

                                                                                                                                                                                    foreach (TuitionFees atuition in StudentTuitionResponse.Data)
                                                                                                                                                                                    {
                                                                                                                                                                                        tuition.Tuition += atuition.Tuition;
                                                                                                                                                                                        tuition.Admission += atuition.Admission;
                                                                                                                                                                                        tuition.Computer += atuition.Computer;
                                                                                                                                                                                        tuition.Development_fee += atuition.Development_fee;
                                                                                                                                                                                    }
                                                                                                                                                                                    StudentTuition.Text = tuition.Tuition.ToString();
                                                                                                                                                                                    StudentAdmission.Text = tuition.Admission.ToString();
                                                                                                                                                                                    StudentComputer.Text = tuition.Computer.ToString();
                                                                                                                                                                                    StudentDevelopment.Text = tuition.Development_fee.ToString();


                                                                                                                                                                                    var StudentTuitionBFRequest = new RestRequest("student_bf/{student_id}/", Method.GET);
                                                                                                                                                                                    StudentTuitionBFRequest.AddUrlSegment("student_id", student.Id.ToString());
                                                                                                                                                                                    Common.addHeaders(StudentTuitionBFRequest);
                                                                                                                                                                                    client.ExecuteAsync<BalanceFowarded>(StudentTuitionBFRequest, StudentTuitionBFResponse =>
                                                                                                                                                                                    {
                                                                                                                                                                                        Application.Current.Dispatcher.Invoke(
                                                                                                                                                                                                DispatcherPriority.Background,
                                                                                                                                                                                                new ThreadStart(
                                                                                                                                                                                                    delegate
                                                                                                                                                                                                    {
                                                                                                                                                                                                        BalanceFowarded bf = StudentTuitionBFResponse.Data;
                                                                                                                                                                                                        try
                                                                                                                                                                                                        {
                                                                                                                                                                                                            StudentTuitionBalanceBF.Text = "[ " + bf.Tuition + " ]";
                                                                                                                                                                                                            StudentDevelomentBalanceBF.Text = "[ " + bf.Development_fee + " ]";
                                                                                                                                                                                                            // StudentAdmissionBalanceBF.Text = "[ " + bf.Admission + " ]";
                                                                                                                                                                                                            StudentAdmissionBalanceBF.Visibility = Visibility.Collapsed;
                                                                                                                                                                                                   
                                                                                                                                                                                                            StudentComputerBalanceBF.Text = "[ " + bf.Computer + " ]";
                                                                                                                                                                                                            StudentUCEBalanceBF.Text = "[ " + bf.Uce + " ]";
                                                                                                                                                                                                            StudentUACEBalanceBF.Text = "[ " + bf.Uace + " ]";
                                                                                                                                                                                                            StudentMockBalanceBF.Text = "[ " + bf.Mock + " ]";

                                                                                                                                                                                                            StudentTuitionBalance.Text = "[ " + (balance_calc.TuitionBalance(tuition.Tuition) - bf.Tuition) + " ]";
                                                                                                                                                                                                            StudentDevelomentBalance.Text = "[ " + (balance_calc.DevelopmentBalance(tuition.Development_fee) - bf.Development_fee) + " ]";
                                                                                                                                                                                                            StudentComputerBalance.Text = "[ " + (balance_calc.ComputerBalance(tuition.Computer) - bf.Computer) + " ]";
                                                                                                                                                                                                            StudentAdmissionBalance.Text = "[ " + bf.Admission + " ]";
                                                                                                                                                                                                        }
                                                                                                                                                                                                        catch(NullReferenceException es)
                                                                                                                                                                                                        {

                                                                                                                                                                                                        }
                                                                                                                                                                                                        /*
                                                                                                                                                                                                        var StudentAdmissionRequest = new RestRequest("get_admission/{student}/", Method.GET);
                                                                                                                                                                                                        StudentAdmissionRequest.AddUrlSegment("student", SELECTED_ITEM.ToString());
                                                                                                                                                                                                        Common.addHeaders(StudentAdmissionRequest);
                                                                                                                                                                                                        client.ExecuteAsync<int>(StudentAdmissionRequest, StudentAdmissionResponse =>
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                                                                                                                                    DispatcherPriority.Background,
                                                                                                                                                                                                                    new ThreadStart(
                                                                                                                                                                                                                        delegate
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            StudentAdmissionBalance.Text = "[ " + (balance_calc.AdmissionBalance(StudentAdmissionResponse.Data) - bf.Admission) + " ]";
                                                                                                                                                                                                                        }));
                                                                                                                                                                                                        });
                                                                                                                                                                                                        */
                                                                                                                                                                                                        var StudentExamRequest = new RestRequest("exams/", Method.GET);
                                                                                                                                                                                                        StudentExamRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                                                                        Common.addHeaders(StudentExamRequest);
                                                                                                                                                                                                        var StudentExamAsyncHandle = client.ExecuteAsync<List<Exam>>(StudentExamRequest, StudentExamResponse =>
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                                                                                                                                    DispatcherPriority.Background,
                                                                                                                                                                                                                    new ThreadStart(
                                                                                                                                                                                                                        delegate
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            Exam exam = new Exam();

                                                                                                                                                                                                                            foreach (Exam aExam in StudentExamResponse.Data)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                exam.Uace += aExam.Uace;
                                                                                                                                                                                                                                exam.Uce += aExam.Uce;
                                                                                                                                                                                                                                exam.Mock += aExam.Mock;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            StudentExamsUACE.Text = exam.Uace.ToString();
                                                                                                                                                                                                                            StudentExamsUCE.Text = exam.Uce.ToString();
                                                                                                                                                                                                                            StudentExamsMock.Text = exam.Mock.ToString();
                                                                                                                                                                                                                            try
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                StudentUCEBalance.Text = "[ " + (balance_calc.UCEBalance(exam.Uce) - bf.Uce) + " ]";
                                                                                                                                                                                                                                StudentUACEBalance.Text = "[ " + (balance_calc.UACEBalance(exam.Uace) - bf.Uace) + " ]";
                                                                                                                                                                                                                                StudentMockBalance.Text = "[ " + (balance_calc.MockBalance(exam.Mock) - bf.Mock) + " ]";
                                                                                                                                                                                                                            }catch(NullReferenceException es)
                                                                                                                                                                                                                            {

                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }));
                                                                                                                                                                                                        });
                                                                                                                                                                                                    }));
                                                                                                                                                                                    });
                                                                                                                                                                                }));
                                                                                                                                                            });

                                                                                                                                                            var StudentTextBookRequest = new RestRequest("textbooks/", Method.GET);
                                                                                                                                                            StudentTextBookRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                            Common.addHeaders(StudentTextBookRequest);
                                                                                                                                                            var StudentTextBookAsyncHandle = client.ExecuteAsync<List<TextBooks>>(StudentTextBookRequest, StudentTextBookResponse =>
                                                                                                                                                            {
                                                                                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                                                                                            DispatcherPriority.Background,
                                                                                                                                                                            new ThreadStart(
                                                                                                                                                                                delegate
                                                                                                                                                                                {
                                                                                                                                                                                    TextBooks textbook = new TextBooks();

                                                                                                                                                                                    foreach (TextBooks aTextBook in StudentTextBookResponse.Data)
                                                                                                                                                                                    {
                                                                                                                                                                                        textbook.Cost += aTextBook.Cost;
                                                                                                                                                                                        textbook.Name += aTextBook.Name;
                                                                                                                                                                                        textbook.Role += aTextBook.Role;
                                                                                                                                                                                    }

                                                                                                                                                                                    StudentTextBookName.Text = textbook.Name;
                                                                                                                                                                                    StudentTextBookRole.Text = textbook.Role;
                                                                                                                                                                                    StudentTextBookCost.Text = textbook.Cost.ToString();
                                                                                                                                                                                }));
                                                                                                                                                            });

                                                                                                                                                            var StudentUniformRequest = new RestRequest("uniforms/", Method.GET);
                                                                                                                                                            StudentUniformRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                            Common.addHeaders(StudentUniformRequest);
                                                                                                                                                            var StudentUniformAsyncHandle = client.ExecuteAsync<List<Uniform>>(StudentUniformRequest, StudentUniformResponse =>
                                                                                                                                                            {
                                                                                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                                                                                            DispatcherPriority.Background,
                                                                                                                                                                            new ThreadStart(
                                                                                                                                                                                delegate
                                                                                                                                                                                {
                                                                                                                                                                                    Uniform uniform = new Uniform();

                                                                                                                                                                                    foreach (Uniform aUniform in StudentUniformResponse.Data)
                                                                                                                                                                                    {
                                                                                                                                                                                        uniform.Jogging += aUniform.Jogging;
                                                                                                                                                                                        uniform.School_t_shirt += aUniform.School_t_shirt;
                                                                                                                                                                                        uniform.Sports_wear += aUniform.Sports_wear;
                                                                                                                                                                                        uniform.Sweater += aUniform.Sweater;
                                                                                                                                                                                        uniform.Casual_wear += aUniform.Casual_wear;
                                                                                                                                                                                        uniform.Class_wear += aUniform.Class_wear;
                                                                                                                                                                                        uniform.Badge += aUniform.Badge;
                                                                                                                                                                                    }

                                                                                                                                                                                    StudentClassUniform.Text = uniform.Class_wear.ToString();
                                                                                                                                                                                    StudentJoggingWear.Text = uniform.Jogging.ToString();
                                                                                                                                                                                    StudentTShirt.Text = uniform.School_t_shirt.ToString();
                                                                                                                                                                                    StudentSportsWear.Text = uniform.Sports_wear.ToString();
                                                                                                                                                                                    StudentSweater.Text = uniform.Sweater.ToString();
                                                                                                                                                                                    StudentCasualWear.Text = uniform.Casual_wear.ToString();
                                                                                                                                                                                    StudentBadge.Text = uniform.Badge.ToString();
                                                                                                                                                                                }));
                                                                                                                                                            });

                                                                                                                                                            var StudentOthersRequest = new RestRequest("others/", Method.GET);
                                                                                                                                                            StudentOthersRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                            Common.addHeaders(StudentOthersRequest);
                                                                                                                                                            var StudentOthersAsyncHandle = client.ExecuteAsync<List<Others>>(StudentOthersRequest, StudentOthersResponse =>
                                                                                                                                                            {
                                                                                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                                                                                            DispatcherPriority.Background,
                                                                                                                                                                            new ThreadStart(
                                                                                                                                                                                delegate
                                                                                                                                                                                {
                                                                                                                                                                                    Others Others = new Others();

                                                                                                                                                                                    foreach (Others aOthers in StudentOthersResponse.Data)
                                                                                                                                                                                    {
                                                                                                                                                                                        Others.Identity_card += aOthers.Identity_card;
                                                                                                                                                                                        Others.Passport_photos += aOthers.Passport_photos;
                                                                                                                                                                                        Others.Hair_cutting += aOthers.Hair_cutting;
                                                                                                                                                                                        Others.Library += aOthers.Library;
                                                                                                                                                                                        Others.Medical += aOthers.Medical;
                                                                                                                                                                                        Others.Scouts += aOthers.Scouts;
                                                                                                                                                                                    }

                                                                                                                                                                                    StudentIDCard.Text = Others.Identity_card.ToString();
                                                                                                                                                                                    StudentPassportPhoto.Text = Others.Passport_photos.ToString();
                                                                                                                                                                                    StudentHairCutting.Text = Others.Hair_cutting.ToString();
                                                                                                                                                                                    StudentLibrary.Text = Others.Library.ToString();
                                                                                                                                                                                    StudentMedical.Text = Others.Medical.ToString();
                                                                                                                                                                                    StudentScouts.Text = Others.Scouts.ToString();
                                                                                                                                                                                }));
                                                                                                                                                            });

                                                                                                                                                            var StudentMiscRequest = new RestRequest("misc/", Method.GET);
                                                                                                                                                            StudentMiscRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                            Common.addHeaders(StudentMiscRequest);
                                                                                                                                                            var StudentMiscAsyncHandle = client.ExecuteAsync<List<Misc>>(StudentMiscRequest, StudentMiscResponse =>
                                                                                                                                                            {
                                                                                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                                                                                            DispatcherPriority.Background,
                                                                                                                                                                            new ThreadStart(
                                                                                                                                                                                delegate
                                                                                                                                                                                {
                                                                                                                                                                                    Misc misc = new Misc();

                                                                                                                                                                                    foreach (Misc aMisc in StudentMiscResponse.Data)
                                                                                                                                                                                    {
                                                                                                                                                                                        misc.Role += aMisc.Role;
                                                                                                                                                                                        misc.Cost += aMisc.Cost;
                                                                                                                                                                                    }

                                                                                                                                                                                    StudentMiscRole.Text = misc.Role;
                                                                                                                                                                                    StudentMiscCost.Text = misc.Cost.ToString();
                                                                                                                                                                                }));
                                                                                                                                                            });

                                                                                                                                                            var StudentReceiptRequest = new RestRequest("receipts/", Method.GET);
                                                                                                                                                            StudentReceiptRequest.AddParameter("student", SELECTED_ITEM.ToString());
                                                                                                                                                            Common.addHeaders(StudentReceiptRequest);
                                                                                                                                                            var StudentReceiptAsyncHandle = client.ExecuteAsync<List<Receipt>>(StudentReceiptRequest, StudentReceiptResponse =>
                                                                                                                                                            {
                                                                                                                                                                Application.Current.Dispatcher.Invoke(
                                                                                                                                                                            DispatcherPriority.Background,
                                                                                                                                                                            new ThreadStart(
                                                                                                                                                                                delegate
                                                                                                                                                                                {
                                                                                                                                                                                    
                                                                                                                                                                                    if (StudentReceiptResponse.Data.Count != 0)
                                                                                                                                                                                    {
                                                                                                                                                                                        StudentReceiptList = new ObservableCollection<Receipt>();
                                                                                                                                                                                        StudentReceiptList.Clear();
                                                                                                                                                                                        StudentReceiptResponse.Data.ToList().ForEach(StudentReceiptList.Add);
                                                                                                                                                                                        ReceiptsList.ItemsSource = StudentReceiptList;
                                                                                                                                                                                        
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {

                                                                                                                                                                                    }
                                                                                                                                                                                }));
                                                                                                                                                            });
                                                                                                                                                        }));
                                                                                                                                        });

                                                                                                                                        GUISwitch.ShowPaymentsGrid();

                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        student.Offering = "NOT REGISTERED";
                                                                                                                                    }
                                                                                                                                }));
                                                                                                                    }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                                                                        catch (ArgumentOutOfRangeException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                                                                        {

                                                                                                                    }

                                                                                                                });


                                                                                                            }));
                                                                                            });
                                                                                        }));
                                                                        });

                                                                    }));
                                                    });

                                                }));
                                    }
                                    else
                                    {
                                        MessageBoxResult termlimits = MessageBox.Show("Term Fees Limits are not set, Set them now", "Term Limits", MessageBoxButton.OK);
                                        if (termlimits == MessageBoxResult.OK)
                                        {
                                            Application.Current.Dispatcher.Invoke(
                                            DispatcherPriority.Background,
                                            new ThreadStart(
                                                delegate
                                                {
                                                    PayableFeesMenuItem_Click();

                                                }));
                                        }
                                    }

                                });



                            }));
            });
        }

        private void NewSession_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sessions/", Method.GET);
            Common.addHeaders(request);
            request.AddParameter("now", "true");
            var asyncHandle = client.ExecuteAsync<List<Session>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Background,
                       new ThreadStart(
                           delegate
                           {
                               if (response.Data.Count == 0)
                               {
                                   SessionWindow window = new SessionWindow();
                                   window.Owner = this;
                                   window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                   window.ShowDialog();
                               }
                               else
                               {
                                   MessageBoxResult AcceptEdit = MessageBox.Show("There is an existing active session,\n" +
                                       "Do you wish to update it?\nTo start a new session close existing session via 'Menu-Sessions-Close'", "Edit Confirmation", MessageBoxButton.YesNo);
                                   if (AcceptEdit == MessageBoxResult.Yes)
                                   {
                                       
                                       Registry.getInstance().register(response.Data[0]);
                                       SessionWindow window = new SessionWindow();
                                       window.Owner = this;
                                       window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                       window.ShowDialog();
                                   }

                               }
                           }));
            });
        }


        private void CheckForActiveSession()
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sessions/", Method.GET);
            Common.addHeaders(request);
            request.AddParameter("now", "true");
            var asyncHandle = client.ExecuteAsync<List<Session>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Background,
                       new ThreadStart(
                           delegate
                           {
                               if (response.Data.Count == 0)
                               {

                                   SessionWindow window = new SessionWindow();
                                   window.Owner = this;
                                   window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                   window.Closed += new EventHandler(SessionWindowClosed);
                                   window.ShowDialog();
                                   

                               }
                               else
                               {
                                   Session session = response.Data[0];
                                   Title = "CRAB.STUDENT : " + session.Year + " TERM " + session.Term + " [ " + session.Start + " to " + session.End + "]";
                                   SearchSponsor_Loaded();
                                   StudentsList_Loaded();

                               }
                               
                           }));
            });
        }

        private void CloseSession_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messagebox = MessageBox.Show("You are about to close the current session,\nThis can't be undone.\nDo you wish to continue?", "Session Closure", MessageBoxButton.OKCancel);
            if(messagebox == MessageBoxResult.OK)
            {
                var client = new RestClient(Common.getConnectionString());
                var request = new RestRequest("sessions/", Method.GET);
                Common.addHeaders(request);
                request.AddParameter("now", "true");
                var asyncHandle = client.ExecuteAsync<List<Session>>(request, response =>
                {
                    Application.Current.Dispatcher.Invoke(
                           DispatcherPriority.Background,
                           new ThreadStart(
                               delegate
                               {
                                   if (response.Data.Count > 0)
                                   {
                                       if (DateTime.Now >= Convert.ToDateTime(response.Data[0].End))
                                       {
                                           var request2 = new RestRequest("sessions/{id}/", Method.PUT);
                                           Common.addHeaders(request2);
                                           request2.AddUrlSegment("id", response.Data[0].Id.ToString());
                                           response.Data[0].Is_active = false;
                                           request2.AddJsonBody(response.Data[0]);
                                           var asyncHandle2 = client.ExecuteAsync<Session>(request2, response2 =>
                                           {
                                               
                                               Application.Current.Dispatcher.Invoke(
                                                       DispatcherPriority.Background,
                                                       new ThreadStart(
                                                           delegate
                                                           {
                                                               MessageBoxResult AskToUpdate = MessageBox.Show("All Active Sessions Have been Closed.\nSandstorm will now restart....", "CRABSTUDENT", MessageBoxButton.OK);
                                                               if (AskToUpdate == MessageBoxResult.OK)
                                                               {
                                                                   Application.Current.Shutdown();
                                                                   System.Windows.Forms.Application.Restart();

                                                               }
                                                           }));
                                           });
                                       }
                                       else
                                       {
                                           MessageBox.Show("Active Session Expiry Date is on " + response.Data[0].End +
                                               ";\nUpdate first to today's date or ealier if you wish to close it now.");
                                       }

                                   }
                                   else
                                   {
                                       MessageBox.Show("No Active Session Found");
                                   }

                               }));
                });
            }
        }

        private void StudentTermReg_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudentList.Count == 1)
            {
                var client = new RestClient(Common.getConnectionString());
                var request = new RestRequest("sessions/", Method.GET);
                Common.addHeaders(request);
                request.AddParameter("now", "true");
                var asyncHandle = client.ExecuteAsync<List<Session>>(request, response =>
                {
                    Application.Current.Dispatcher.Invoke(
                           DispatcherPriority.Background,
                           new ThreadStart(
                               delegate
                               {
                                   if (response.Data.Count > 0)
                                   {

                                       Registry.getInstance().register(response.Data[0]);
                                       object ST = SELECTED_ITEM;
                                       Registry.getInstance().register(ST, "STUDENT");
                                       TermReg window = new TermReg();
                                       window.Owner = this;
                                       window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                       window.Closed += new EventHandler(ReceiptWindowClosed);
                                       window.ShowDialog();
                                   }
                                   else
                                   {
                                       MessageBox.Show("No active Sessions Found.\nYou need atleast one active session to continue.");
                                   }
                               }));
                });
            }
            else
            {
                MessageBox.Show("Only One (1) Student should be selected to use this feature");
            }
            
        }
        
        
        private void ReceptsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var client = new RestClient(Common.getConnectionString());

            //initialize fields to zero
            SELECTED_RECEIPT = (Receipt) ReceiptsList.SelectedItem;

            StudentTuition.Text = "0";
            StudentAdmission.Text = "0";
            StudentComputer.Text = "0";
            StudentDevelopment.Text = "0";
            ActivatedReceipts.Content = "[ ]";

            int counter = 0;

            foreach (Receipt receipt in ReceiptsList.SelectedItems)
            {
                counter++;
                if(counter == 10)
                {
                    ActivatedReceipts.Content = "[ " + receipt.Receipt_number + ", \n" + ActivatedReceipts.Content.ToString().Split('[')[1];
                    counter = 0;
                }
                else
                {
                    ActivatedReceipts.Content = "[ " + receipt.Receipt_number + ", " + ActivatedReceipts.Content.ToString().Split('[')[1];
                }
                

                var TuitionRequest = new RestRequest("tuition/", Method.GET);
                Common.addHeaders(TuitionRequest);
                TuitionRequest.AddParameter("receipt", receipt.Id.ToString());
                var asyncHandle = client.ExecuteAsync<List<TuitionFees>>(TuitionRequest, TuitionResponse =>
                {
                    Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new ThreadStart(
                                delegate
                                {
                                    if (TuitionResponse.Data.Count > 0)
                                    {
                                        StudentTuition.Text = (Convert.ToInt32(StudentTuition.Text) + TuitionResponse.Data[0].Tuition).ToString();
                                        StudentAdmission.Text = (Convert.ToInt32(StudentAdmission.Text) + TuitionResponse.Data[0].Admission).ToString();
                                        StudentComputer.Text = (Convert.ToInt32(StudentComputer.Text) + TuitionResponse.Data[0].Computer).ToString();
                                        StudentDevelopment.Text = (Convert.ToInt32(StudentDevelopment.Text) + TuitionResponse.Data[0].Development_fee).ToString();
                                    }

                                }));
                });
            }
            
        }
        

        private void EditSession_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sessions/", Method.GET);
            Common.addHeaders(request);
            request.AddParameter("now", "true");
            var asyncHandle = client.ExecuteAsync<List<Session>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Background,
                       new ThreadStart(
                           delegate
                           {
                               if (response.Data.Count == 0)
                               {
                                   MessageBox.Show("No Active Session Found");
                               }
                               else
                               {
                                   Registry.getInstance().register(response.Data[0]);
                                   SessionWindow window = new SessionWindow();
                                   window.Owner = this;
                                   window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                   window.ShowDialog();
                               }
                           }));
            });
        }

        private void PayableFeesSaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<TextBox> controls = new List<TextBox>();
            controls.Add(GlobalPayableFees);
            controls.Add(Class1AdditionalFees);
            controls.Add(Class2AdditionalFees);
            controls.Add(Class3AdditionalFees);
            controls.Add(Class4AdditionalFees);
            controls.Add(Class5AdditionalFees);
            controls.Add(Class6AdditionalFees);
            controls.Add(BClass1AdditionalFees);
            controls.Add(BClass2AdditionalFees);
            controls.Add(BClass3AdditionalFees);
            controls.Add(BClass4AdditionalFees);
            controls.Add(BClass5AdditionalFees);
            controls.Add(BClass6AdditionalFees);
            controls.Add(ComputerPayableFees);
            controls.Add(UCEPayableFees);
            controls.Add(UACEPayableFees);
            controls.Add(MockPayableFees);
            controls.Add(SciencesPayableFees);
            controls.Add(ArtsPayableFees);
            controls.Add(AdmissionPayableFees);
            controls.Add(DevelopmentPayableFees);
            controls.Add(BoardingPayableFees);

            if (Common.AreNullOrWhiteSpace(controls))
            {
                TermPayableFees fees = new TermPayableFees();
                fees.Session = ACTIVE_SESSION.Id;
                fees.Global = Convert.ToInt32(GlobalPayableFees.Text);
                fees.Boarding = Convert.ToInt32(BoardingPayableFees.Text);
                fees.Uce = Convert.ToInt32(UCEPayableFees.Text);
                fees.Uace = Convert.ToInt32(UACEPayableFees.Text);
                fees.Mock = Convert.ToInt32(MockPayableFees.Text);
                fees.Sciences = Convert.ToInt32(SciencesPayableFees.Text);
                fees.Arts = Convert.ToInt32(ArtsPayableFees.Text);
                fees.Admission = Convert.ToInt32(AdmissionPayableFees.Text);
                fees.Development = Convert.ToInt32(DevelopmentPayableFees.Text);

                ComputerPayableFees conputerFees = new ComputerPayableFees();
                conputerFees.Fees = Convert.ToInt32(ComputerPayableFees.Text);
                conputerFees.IsClass1_applicable = DoesClass1PayComputer.IsChecked.Value;
                conputerFees.IsClass2_applicable = DoesClass2PayComputer.IsChecked.Value;
                conputerFees.IsClass3_applicable = DoesClass3PayComputer.IsChecked.Value;
                conputerFees.IsClass4_applicable = DoesClass4PayComputer.IsChecked.Value;
                conputerFees.IsClass5_applicable = DoesClass5PayComputer.IsChecked.Value;
                conputerFees.IsClass6_applicable = DoesClass6PayComputer.IsChecked.Value;

                TermClassAdditionalFees addFees = new TermClassAdditionalFees();
                addFees.Class1 = Convert.ToInt32(Class1AdditionalFees.Text);
                addFees.Class2 = Convert.ToInt32(Class2AdditionalFees.Text);
                addFees.Class3 = Convert.ToInt32(Class3AdditionalFees.Text);
                addFees.Class4 = Convert.ToInt32(Class4AdditionalFees.Text);
                addFees.Class5 = Convert.ToInt32(Class5AdditionalFees.Text);
                addFees.Class6 = Convert.ToInt32(Class6AdditionalFees.Text);
                addFees.Is_Boarding = false;

                TermClassAdditionalFees boadingAddFees = new TermClassAdditionalFees();
                boadingAddFees.Class1 = Convert.ToInt32(BClass1AdditionalFees.Text);
                boadingAddFees.Class2 = Convert.ToInt32(BClass2AdditionalFees.Text);
                boadingAddFees.Class3 = Convert.ToInt32(BClass3AdditionalFees.Text);
                boadingAddFees.Class4 = Convert.ToInt32(BClass4AdditionalFees.Text);
                boadingAddFees.Class5 = Convert.ToInt32(BClass5AdditionalFees.Text);
                boadingAddFees.Class6 = Convert.ToInt32(BClass6AdditionalFees.Text);
                boadingAddFees.Is_Boarding = true;


                var client = new RestClient(Common.getConnectionString());

                var TermPayableFeesGetRequest = new RestRequest("term_payable_fees/", Method.GET);
                TermPayableFeesGetRequest.AddQueryParameter("session", ACTIVE_SESSION.Id.ToString());
                Common.addHeaders(TermPayableFeesGetRequest);
                client.ExecuteAsync<List<TermPayableFees>>(TermPayableFeesGetRequest, TermPayableFeesGetResponse =>
                {
                    Application.Current.Dispatcher.Invoke(
                           DispatcherPriority.Background,
                           new ThreadStart(
                               delegate
                               {
                                   if (TermPayableFeesGetResponse.Data.Count > 0)
                                   {
                                       List<int> addFeesIds = new List<int>();
                                       addFeesIds.Add(TermPayableFeesGetResponse.Data[0].TermClassAdditionalFees[0]);
                                       addFeesIds.Add(TermPayableFeesGetResponse.Data[0].TermClassAdditionalFees[1]);

                                       fees.TermClassAdditionalFees = addFeesIds;
                                       fees.ComputerPayableFees = TermPayableFeesGetResponse.Data[0].ComputerPayableFees;

                                       var TermPayableFeesPutRequest = new RestRequest("term_payable_fees/{id}/", Method.PUT);
                                       Common.addHeaders(TermPayableFeesPutRequest);
                                       TermPayableFeesPutRequest.AddUrlSegment("id", TermPayableFeesGetResponse.Data[0].Id.ToString());
                                       TermPayableFeesPutRequest.AddJsonBody(fees);
                                       client.ExecuteAsync<TermPayableFees>(TermPayableFeesPutRequest, TermPayableFeesPutResponse =>
                                       { });

                                       var ComputerFeesPutRequest = new RestRequest("computer/{id}/", Method.PUT);
                                       Common.addHeaders(ComputerFeesPutRequest);
                                       ComputerFeesPutRequest.AddJsonBody(conputerFees);
                                       ComputerFeesPutRequest.AddUrlSegment("id", TermPayableFeesGetResponse.Data[0].ComputerPayableFees.ToString());
                                       client.ExecuteAsync<ComputerPayableFees>(ComputerFeesPutRequest, ComputerFeesPutResponse =>
                                       { });

                                       var TermAdditionalFeesPutRequest = new RestRequest("term_class_additional_fees/{id}/", Method.PUT);
                                       Common.addHeaders(TermAdditionalFeesPutRequest);
                                       TermAdditionalFeesPutRequest.AddJsonBody(addFees);
                                       TermAdditionalFeesPutRequest.AddUrlSegment("id", TermPayableFeesGetResponse.Data[0].TermClassAdditionalFees[0].ToString());
                                       client.ExecuteAsync<TermClassAdditionalFees>(TermAdditionalFeesPutRequest, TermAdditionalFeesPutResponse =>
                                       { });

                                       var BAdditionalFeesPutRequest = new RestRequest("term_class_additional_fees/{id}/", Method.PUT);
                                       Common.addHeaders(BAdditionalFeesPutRequest);
                                       BAdditionalFeesPutRequest.AddJsonBody(boadingAddFees);
                                       BAdditionalFeesPutRequest.AddUrlSegment("id", TermPayableFeesGetResponse.Data[0].TermClassAdditionalFees[1].ToString());
                                       client.ExecuteAsync<TermClassAdditionalFees>(BAdditionalFeesPutRequest, BAdditionalFeesPutResponse =>
                                       { });
                                   }
                                   else
                                   {
                                       var TermAdditionalFeesPostRequest = new RestRequest("term_class_additional_fees/", Method.POST);
                                       Common.addHeaders(TermAdditionalFeesPostRequest);
                                       TermAdditionalFeesPostRequest.AddJsonBody(addFees);
                                       client.ExecuteAsync<TermClassAdditionalFees>(TermAdditionalFeesPostRequest, TermAdditionalFeesPostResponse =>
                                       {
                                           var BAdditionalFeesPostRequest = new RestRequest("term_class_additional_fees/", Method.POST);
                                           Common.addHeaders(BAdditionalFeesPostRequest);
                                           BAdditionalFeesPostRequest.AddJsonBody(boadingAddFees);
                                           client.ExecuteAsync<TermClassAdditionalFees>(BAdditionalFeesPostRequest, BAdditionalFeesPostResponse =>
                                           {
                                               var ComputerFeesPostRequest = new RestRequest("computer/", Method.POST);
                                               Common.addHeaders(ComputerFeesPostRequest);
                                               ComputerFeesPostRequest.AddJsonBody(conputerFees);
                                               client.ExecuteAsync<ComputerPayableFees>(ComputerFeesPostRequest, ComputerFeesPostResponse =>
                                               {
                                                   List<int> addFeesIds = new List<int>();
                                                   addFeesIds.Add(TermAdditionalFeesPostResponse.Data.Id);
                                                   addFeesIds.Add(BAdditionalFeesPostResponse.Data.Id);

                                                   fees.TermClassAdditionalFees = addFeesIds;
                                                   fees.ComputerPayableFees = ComputerFeesPostResponse.Data.Id;

                                                   var TermPayableFeesPostRequest = new RestRequest("term_payable_fees/", Method.POST);
                                                   Common.addHeaders(TermPayableFeesPostRequest);
                                                   TermPayableFeesPostRequest.AddJsonBody(fees);
                                                   client.ExecuteAsync<TermPayableFees>(TermPayableFeesPostRequest, TermPayableFeesPostResponse =>
                                                   {

                                                   });
                                               });
                                           });
                                       });
                                   }
                               }));
                });
            }

        }

        private void APIBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.getConnectionString());
        }


        private void PayableFeesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PayableFeesMenuItem_Click();
        }


        private void PayableFeesMenuItem_Click()
        {
            GUISwitch.ShowPayableFees();
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("term_payable_fees/", Method.GET);
            request.AddQueryParameter("session", ACTIVE_SESSION.Id.ToString());
            Common.addHeaders(request);
            var asyncHandle = client.ExecuteAsync<List<TermPayableFees>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Background,
                       new ThreadStart(
                           delegate
                           {
                               if (response.Data.Count != 0)
                               {
                                   TermPayableFees fees = response.Data[0];
                                   GlobalPayableFees.Text = fees.Global.ToString();
                                   UCEPayableFees.Text = fees.Uce.ToString();
                                   UACEPayableFees.Text = fees.Uace.ToString();
                                   MockPayableFees.Text = fees.Mock.ToString();
                                   SciencesPayableFees.Text = fees.Sciences.ToString();
                                   ArtsPayableFees.Text = fees.Arts.ToString();
                                   AdmissionPayableFees.Text = fees.Admission.ToString();
                                   DevelopmentPayableFees.Text = fees.Development.ToString();
                                   BoardingPayableFees.Text = fees.Boarding.ToString();

                                   var ComputerFeesGetRequest = new RestRequest("computer/{id}/", Method.GET);
                                   Common.addHeaders(ComputerFeesGetRequest);
                                   ComputerFeesGetRequest.AddUrlSegment("id", fees.ComputerPayableFees.ToString());
                                   var ComputerFeesGetAsyncHandle = client.ExecuteAsync<ComputerPayableFees>(ComputerFeesGetRequest, ComputerFeesGetResponse =>
                                   {
                                       Application.Current.Dispatcher.Invoke(
                                              DispatcherPriority.Background,
                                              new ThreadStart(
                                                  delegate
                                                  {
                                                      ComputerPayableFees.Text = ComputerFeesGetResponse.Data.Fees.ToString();
                                                      DoesClass1PayComputer.IsChecked = ComputerFeesGetResponse.Data.IsClass1_applicable;
                                                      DoesClass2PayComputer.IsChecked = ComputerFeesGetResponse.Data.IsClass2_applicable;
                                                      DoesClass3PayComputer.IsChecked = ComputerFeesGetResponse.Data.IsClass3_applicable;
                                                      DoesClass4PayComputer.IsChecked = ComputerFeesGetResponse.Data.IsClass4_applicable;
                                                      DoesClass5PayComputer.IsChecked = ComputerFeesGetResponse.Data.IsClass5_applicable;
                                                      DoesClass6PayComputer.IsChecked = ComputerFeesGetResponse.Data.IsClass6_applicable;
                                                  }));
                                   });

                                   var TermAdditionalFeesGetRequest = new RestRequest("term_class_additional_fees/", Method.GET);
                                   TermAdditionalFeesGetRequest.AddQueryParameter("offering", false.ToString());
                                   TermAdditionalFeesGetRequest.AddQueryParameter("term_fees", fees.Id.ToString());
                                   Common.addHeaders(TermAdditionalFeesGetRequest);
                                   client.ExecuteAsync<List<TermClassAdditionalFees>>(TermAdditionalFeesGetRequest, TermAdditionalFeesGetResponse =>
                                   {
                                       Application.Current.Dispatcher.Invoke(
                                              DispatcherPriority.Background,
                                              new ThreadStart(
                                                  delegate
                                                  {
                                                      try
                                                      {
                                                          Class1AdditionalFees.Text = TermAdditionalFeesGetResponse.Data[0].Class1.ToString();
                                                          Class2AdditionalFees.Text = TermAdditionalFeesGetResponse.Data[0].Class2.ToString();
                                                          Class3AdditionalFees.Text = TermAdditionalFeesGetResponse.Data[0].Class3.ToString();
                                                          Class4AdditionalFees.Text = TermAdditionalFeesGetResponse.Data[0].Class4.ToString();
                                                          Class5AdditionalFees.Text = TermAdditionalFeesGetResponse.Data[0].Class5.ToString();
                                                          Class6AdditionalFees.Text = TermAdditionalFeesGetResponse.Data[0].Class6.ToString();
#pragma warning disable CS0168 // Variable is declared but never used
                                                      }
                                                      catch (IndexOutOfRangeException es)
#pragma warning restore CS0168 // Variable is declared but never used
                                                      {
                                                          
                                                      }
                                                  }));
                                   });

                                   var BAdditionalFeesGetRequest = new RestRequest("term_class_additional_fees/", Method.GET);
                                   BAdditionalFeesGetRequest.AddQueryParameter("offering", true.ToString());
                                   BAdditionalFeesGetRequest.AddQueryParameter("term_fees", fees.Id.ToString());
                                   Common.addHeaders(BAdditionalFeesGetRequest);
                                   client.ExecuteAsync<List<TermClassAdditionalFees>>(BAdditionalFeesGetRequest, BAdditionalFeesGetResponse =>
                                   {
                                       Application.Current.Dispatcher.Invoke(
                                              DispatcherPriority.Background,
                                              new ThreadStart(
                                                  delegate
                                                  {
                                                      try
                                                      {
                                                          BClass1AdditionalFees.Text = BAdditionalFeesGetResponse.Data[0].Class1.ToString();
                                                          BClass2AdditionalFees.Text = BAdditionalFeesGetResponse.Data[0].Class2.ToString();
                                                          BClass3AdditionalFees.Text = BAdditionalFeesGetResponse.Data[0].Class3.ToString();
                                                          BClass4AdditionalFees.Text = BAdditionalFeesGetResponse.Data[0].Class4.ToString();
                                                          BClass5AdditionalFees.Text = BAdditionalFeesGetResponse.Data[0].Class5.ToString();
                                                          BClass6AdditionalFees.Text = BAdditionalFeesGetResponse.Data[0].Class6.ToString();
#pragma warning disable CS0168 // Variable is declared but never used
                                                      }
                                                      catch (IndexOutOfRangeException es)
#pragma warning restore CS0168 // Variable is declared but never used
                                                      {

                                                      }
                                                  }));
                                   });

                                   SponsorsList_Loaded();
                               }
                           }));
            });


        }


        private void SponsorsList_Loaded()
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sponsor/", Method.GET);
            Common.addHeaders(request);
            var asyncHandle = client.ExecuteAsync<List<Sponsor>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                try {
                                    Sponsors = new ObservableCollection<Sponsor>();
                                    response.Data.ToList().ForEach(Sponsors.Add);
                                    SponsorsList.ItemsSource = Sponsors;
#pragma warning disable CS0168 // Variable is declared but never used
                                }
                                catch (ArgumentNullException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                            }));
            });
        }

        private void EditReceipt_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sessions/{id}/", Method.GET);
            Common.addHeaders(request);
            request.AddUrlSegment("id", SELECTED_RECEIPT.Session.ToString());
            client.ExecuteAsync<Session>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                         new ThreadStart(delegate
                         {
                             Registry.getInstance().register(SELECTED_RECEIPT, "SELECTED_RECEIPT");
                             Registry.getInstance().register(response.Data, "SELECTED_RECEIPT_SESSION");

                             ReceiptUpdate window = new ReceiptUpdate();
                             window.Owner = this;
                             window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                             window.Closed += new EventHandler(ReceiptWindowClosed);
                             window.ShowDialog();
                         }));

            });

            
        }

        private void StudentInformation_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Registry.getInstance().register(SELECTED_ITEM_STUDENT, "SELECTED_STUDENT");
            Registry.getInstance().register(SELECTED_ITEM_SPONSOR, "SELECTED_SPONSOR");

            StudentUpdate window = new StudentUpdate();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closed += new EventHandler(ReceiptWindowClosed);
            window.ShowDialog();
        }

        private void DatabaseBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.getDatabaseConnectionString());
        }

        private void MenuItem_Click()
        {
            this.Close();
        }

        private void Export2Excel_Click(object sender, RoutedEventArgs e)
        {
            Registry.getInstance().register("PAYMENTS", "EXPORT_DATA_TYPE");
            ExportStudentPaymentRecord();
        }

        private void ExportStudentPaymentRecord()
        {
            List<Student> studentList = new List<Student>();
            studentList.Clear();
            if (StudentsList.SelectedItems.Count > 0)
            {
                foreach (Student student in StudentsList.SelectedItems)
                {
                    studentList.Add(student);
                }

                Registry.getInstance().register(studentList, "STUDENTS_TO_EXPORT");

                ExportWindow window = new ExportWindow();
                window.Owner = this;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Atleast One(1) Student should be selected");
            }
        }

        private void MenuSponsorPaymentOption_Click(object sender, RoutedEventArgs e)
        {
            SponsorReceipt window = new SponsorReceipt();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closed += new EventHandler(NewSponsorAddedCloseEvent);
            window.ShowDialog();
        }

        private void PrintStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Student student = (Student)StudentsList.SelectedItems[0];
                

                if (StudentsList.SelectedItems.Count == 1)
                {
                    var client = new RestClient(Common.getConnectionString());
                    var StudentTuitionRequest = new RestRequest("tuition/", Method.GET);
                    StudentTuitionRequest.AddParameter("student", student.Id.ToString());
                    Common.addHeaders(StudentTuitionRequest);
                    client.ExecuteAsync<List<TuitionFees>>(StudentTuitionRequest, StudentTuitionResponse =>
                    {
                        var PayableTuitionRequest = new RestRequest("payable_tuition/{id}/", Method.GET);
                        PayableTuitionRequest.AddUrlSegment("id", student.Id.ToString());
                        Common.addHeaders(PayableTuitionRequest);
                        client.ExecuteAsync<int>(PayableTuitionRequest, PayableTuitionResponse =>
                        {
                            var StudentSponsorRequest = new RestRequest("sponsor/{id}/", Method.GET);
                            StudentSponsorRequest.AddUrlSegment("id", student.Sponsor.ToString());
                            Common.addHeaders(StudentSponsorRequest);
                            var StudentReceiptAsyncHandle = client.ExecuteAsync<Sponsor>(StudentSponsorRequest, StudentSponsorResponse =>
                            {
                                var StudentRegRequest = new RestRequest("terms/", Method.GET);
                                Common.addHeaders(StudentRegRequest);
                                StudentRegRequest.AddParameter("student", student.Id.ToString());
                                var StudentOfferingAsyncHandle = client.ExecuteAsync<List<TermRegistration>>(StudentRegRequest, StudentRegResponse =>
                                {
                                    if(StudentRegResponse.Data.Count > 0)
                                    {
                                        Application.Current.Dispatcher.Invoke(
                                    DispatcherPriority.Background,
                                    new ThreadStart(
                                        delegate
                                        {
                                            CustomPrinter printer = new CustomPrinter(
                                                       student,
                                                       StudentTuitionResponse.Data,
                                                       PayableTuitionResponse.Data,
                                                       StudentSponsorResponse.Data,
                                                       StudentRegResponse.Data[0]);
                                            printer.printStudent();
                                        }));
                                    }
                                    else
                                    {
                                        MessageBox.Show("Term Registration Not Completed. This can be done the the Sessions Menu");
                                    }
                                    
                                });
                                           
                            });
                            
                        });
                    });
                }
                else
                {
                    MessageBox.Show("Exactly One (1) Student should be selected");
                }
            }
            catch(Exception ex) when (ex is NullReferenceException | ex is IndexOutOfRangeException | ex is ArgumentOutOfRangeException)
            {

                MessageBox.Show("Exactly One (1) Student should be selected");
            }
            

        }

        private void MenuSponsorPaymentReviewOption_Click(object sender, EventArgs e)
        {
            GUISwitch.ShowSponsrsGrid();
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sponsor/", Method.GET);
            Common.addHeaders(request);
            var asyncHandle = client.ExecuteAsync<List<Sponsor>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                try
                                {
                                    _Sponsors = new ObservableCollection<Sponsor>();
                                    response.Data.ToList().ForEach(_Sponsors.Add);
                                    ListOfSponsors.ItemsSource = _Sponsors;
                                    ListOfSponsors.SelectedIndex = 0;
                                    ReviewSponsor((Sponsor)ListOfSponsors.SelectedItem);
                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                catch (ArgumentNullException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                            }));
            });
        }

        private void AddNewSponsor_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.getConnectionString()+"/sponsor");
        }

        private void SponsorListItemSelected_Click(object sender, EventArgs e)
        {
            SELECTED_ITEM_SPONSOR = (Sponsor)ListOfSponsors.SelectedItem;
            Sponsor sponsor = (Sponsor)ListOfSponsors.SelectedItem;
           ReviewSponsor(sponsor);
        }

        private void NewSponsorAddedCloseEvent(object sender, EventArgs e)
        {
            GUISwitch.ShowSponsrsGrid();
            Sponsor sponsor = (Sponsor)Registry.getInstance().getReference("NEW_RECEIPT_SPONSOR");
            ReviewSponsor(sponsor);
            Registry.getInstance().remove("NEW_RECEIPT_SPONSOR");

        }

        

        private void ReviewSponsor(Sponsor sponsor)
        {
            try
            {
                SponsorReceiptTitle.Content = "Sponsor Receipts [" + sponsor.Name + "]";
                SponsorDiscount.Text = sponsor.Discount.ToString();
                SponsorTuition.Text = sponsor.Tuition.ToString();
                SponsorBoarding.Text = sponsor.Boarding.ToString();
                SponsorAdmission.Text = sponsor.Admission.ToString();
                SponsorComputer.Text = sponsor.Computer.ToString();
                SponsorDevelopment.Text = sponsor.Development_fee.ToString();
                SponsorUCE.Text = sponsor.Uce.ToString();
                SponsorUACE.Text = sponsor.Uace.ToString();
                SponsorMock.Text = sponsor.Mock.ToString();
                SponsorUniforms.Text = sponsor.Uniforms.ToString();
                SponsorOthers.Text = sponsor.Others.ToString();
                SponsorSciences.Text = sponsor.Sciences.ToString();
                SponsorClass1AdditionalFees.Content = sponsor.AdditionalClass1Fees.ToString();
                SponsorClass2AdditionalFees.Content = sponsor.AdditionalClass2Fees.ToString();
                SponsorClass3AdditionalFees.Content = sponsor.AdditionalClass3Fees.ToString();
                SponsorClass4AdditionalFees.Content = sponsor.AdditionalClass4Fees.ToString();
                SponsorClass5AdditionalFees.Content = sponsor.AdditionalClass5Fees.ToString();
                SponsorClass6AdditionalFees.Content = sponsor.AdditionalClass6Fees.ToString();

                SponsorBClass1AdditionalFees.Content = sponsor.BAdditionalClass1Fees.ToString();
                SponsorBClass2AdditionalFees.Content = sponsor.BAdditionalClass2Fees.ToString();
                SponsorBClass3AdditionalFees.Content = sponsor.BAdditionalClass3Fees.ToString();
                SponsorBClass4AdditionalFees.Content = sponsor.BAdditionalClass4Fees.ToString();
                SponsorBClass5AdditionalFees.Content = sponsor.BAdditionalClass5Fees.ToString();
                SponsorBClass6AdditionalFees.Content = sponsor.BAdditionalClass6Fees.ToString();


                SponsorReceiptsList_Loaded(sponsor);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {

            }
            
        }

        
        private void SponsorReceiptsList_Loaded(Sponsor sponsor)
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sponsor_receipts/", Method.GET);
            Common.addHeaders(request);
            request.AddParameter("sponsor", sponsor.Id.ToString());
            var asyncHandle = client.ExecuteAsync<List<Receipt>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                SponsorReceipts = new ObservableCollection<Receipt>();
                                response.Data.ToList().ForEach(SponsorReceipts.Add);
                                SponsorReceiptsList.ItemsSource = SponsorReceipts;
                            }));
            });
        }

        private void EditSponsorReceipt_Click(object sender, RoutedEventArgs e)
        {
            Receipt receipt = (Receipt)SponsorReceiptsList.SelectedItem;

            Registry.getInstance().register(receipt, "SPONSOR_RECEIPT");
            SponsorReceipt window = new SponsorReceipt();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closed += new EventHandler(NewSponsorAddedCloseEvent);
            window.ShowDialog();
        }

        
        private void ToggleSearchView()
        {
            List<Control> SearchComponents = new List<Control>();
            SearchComponents.Add(SearchName);
            SearchComponents.Add(SearchNo);
            SearchComponents.Add(SearchClass);
            SearchComponents.Add(SearchOffering);
            SearchComponents.Add(SearchSubjects);
            SearchComponents.Add(SearchSponsor);
            SearchComponents.Add(ttballabel);
            SearchComponents.Add(SearchTuitionBalance);

            foreach (Control control in SearchComponents)
            {
                if (control.IsVisible)
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
                
            }

            if (!IsSearchVisible())
            {
                SearchString.Visibility = Visibility.Visible;
            }
            else
            {
                SearchString.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsSearchVisible()
        {
            List<Control> SearchComponents = new List<Control>();
            SearchComponents.Add(SearchName);
            SearchComponents.Add(SearchNo);
            SearchComponents.Add(SearchClass);
            SearchComponents.Add(SearchOffering);
            SearchComponents.Add(SearchSubjects);
            SearchComponents.Add(SearchSponsor);
            SearchComponents.Add(ttballabel);
            SearchComponents.Add(SearchTuitionBalance);

            foreach (Control control in SearchComponents)
            {
                if (!control.IsVisible)
                    return false;
            }

            return true;
        }

        private void ToggleSearch_Click(object sender, RoutedEventArgs e)
        {
            List<Control> SearchComponents = new List<Control>();
            SearchComponents.Add(SearchName);
            SearchComponents.Add(SearchNo);
            SearchComponents.Add(SearchClass);
            SearchComponents.Add(SearchOffering);
            SearchComponents.Add(SearchSubjects);
            SearchComponents.Add(SearchSponsor);
            SearchComponents.Add(ttballabel);
            SearchComponents.Add(SearchTuitionBalance);

            foreach (Control control in SearchComponents)
            {
                control.Visibility = Visibility.Collapsed;

            }

            SearchString.Visibility = Visibility.Visible;
        }

        private void ExportBalances_Click(object sender, RoutedEventArgs e)
        {
            Registry.getInstance().register("BALANCES", "EXPORT_DATA_TYPE");
            ExportStudentPaymentRecord();
            
        }

        private void BalanceFowardedMenuOption_Click(object sender, RoutedEventArgs e)
        {
            if (SELECTED_ITEM_STUDENT == null)
            {
                var client = new RestClient(Common.getConnectionString());
                var StudentRequest = new RestRequest("student/{id}/", Method.GET);
                Common.addHeaders(StudentRequest);
                StudentRequest.AddUrlSegment("id", SELECTED_ITEM.ToString());
                client.ExecuteAsync<Student>(StudentRequest, StudentResponse =>
                {
                    Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new ThreadStart(
                                delegate
                                {
                                    Registry.getInstance().register(StudentResponse.Data, "SELECTED_STUDENT");

                                    BalanceForwarded window = new BalanceForwarded();
                                    window.Owner = this;
                                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                    window.Closed += new EventHandler(ReceiptWindowClosed);
                                    window.ShowDialog();

                                }));
                });
            }
            else
            {
                Registry.getInstance().register(SELECTED_ITEM_STUDENT, "SELECTED_STUDENT");

                BalanceForwarded window = new BalanceForwarded();
                window.Owner = this;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Closed += new EventHandler(ReceiptWindowClosed);
                window.ShowDialog();
            }
            
        }

        private void AboutCRABSTUDENT_Click(object sender, RoutedEventArgs e)
        {
            AboutCRABSTUDENT window = new AboutCRABSTUDENT();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            GUISwitch.ShowStudentsList();
        }

        private void SwitchSession_Click(object sender, RoutedEventArgs e)
        {
            SwitchSessions window = new SwitchSessions();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        private void EditSponsor_Click(object sender, RoutedEventArgs e)
        {
            Registry.getInstance().register(SELECTED_ITEM_SPONSOR, "SELECTED_SPONSOR_TO_MANAGE");
            SponsorManager window = new SponsorManager();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closed += new EventHandler(MenuSponsorPaymentReviewOption_Click);
            window.ShowDialog();
            
        }

        private void MenuNewSponsorOption_Click(object sender, RoutedEventArgs e)
        {
            SponsorManager window = new SponsorManager();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closed += new EventHandler(MenuSponsorPaymentReviewOption_Click);
            window.ShowDialog();
        }
    }
}
