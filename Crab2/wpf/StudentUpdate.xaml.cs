using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CRABSTUDENT.wpf
{
    /// <summary>
    /// Interaction logic for StudentUpdate.xaml
    /// </summary>
    public partial class StudentUpdate : Window
    {
        Student student;
        Sponsor sponsor;
        private ObservableCollection<Sponsor> Sponsors;
        private ObservableCollection<string> subjects;

        public StudentUpdate()
        {
            InitializeComponent();
            student = (Student)Registry.getInstance().getReference("SELECTED_STUDENT");
            Populate_Form();
        }

        private void Populate_Form()
        {
            
            StudentNoTextBox.Text = student.Student_no.ToString();
            StudentNameTextBox.Text = student.Student_name;
            RegistrationGrid_Loaded();

        }


        private void RegistrationGrid_Loaded()
        {
            //setup combo box content

            subjects = new ObservableCollection<string>();

            if(student.Student_class < 5)
            {
                subjects.Add("ALL");
            }
            else
            {
                subjects.Add("ARTS");
                subjects.Add("SCIENCES");
            }
            SubjectsComboBox.ItemsSource = subjects;

            List<int> classes = new List<int>();
            for (int x = 1; x < 7; x++)
            {
                classes.Add(x);
            }

            foreach (int aClass in classes)
            {
                StudentClassComboBox.Items.Add(aClass);
            }

            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sponsor/", Method.GET);
            Common.addHeaders(request);
            client.ExecuteAsync<List<Sponsor>>(request, response =>
            {
                var TermRegRequest = new RestRequest("terms/", Method.GET);
                Common.addHeaders(TermRegRequest);
                TermRegRequest.AddParameter("student", this.student.Id.ToString());
                client.ExecuteAsync<List<TermRegistration>>(TermRegRequest, TermRegResponse =>
                {
                 
                        Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                Sponsors = new ObservableCollection<Sponsor>();
                                Sponsors.Clear();
                                response.Data.ToList().ForEach(Sponsors.Add);
                                SponsorComboBox.ItemsSource = Sponsors;
                                if (TermRegResponse.Data.Count > 0)
                                {
                                    StudentClassComboBox.SelectedItem = TermRegResponse.Data[0].Student_class;
                                    StudentFeesOffer.Text = TermRegResponse.Data[0].Fees_offer.ToString();

                                    foreach (Sponsor sponsor in response.Data)
                                    {
                                        if (sponsor.Id == TermRegResponse.Data[0].Sponsor)
                                        {
                                            SponsorComboBox.SelectedItem = sponsor;
                                            this.sponsor = sponsor;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Sponsor sponsor in response.Data)
                                    {
                                        if (sponsor.Id == student.Sponsor)
                                        {
                                            SponsorComboBox.SelectedItem = sponsor;
                                            this.sponsor = sponsor;
                                            break;
                                        }
                                    }
                                }

                            }));
                    
                });
                    
            });
            SubjectsComboBox.SelectedItem = student.Subjects;


        }

        private void StudentEditButton_Click(object sender, RoutedEventArgs e)
        {
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
                    var client = new RestClient(Common.getConnectionString());

                    var TermRegRequest = new RestRequest("terms/", Method.GET);
                    Common.addHeaders(TermRegRequest);
                    TermRegRequest.AddParameter("student", this.student.Id.ToString());
                    client.ExecuteAsync<List<TermRegistration>>(TermRegRequest, TermRegResponse =>
                    {
                        if (TermRegResponse.Data.Count > 0)
                        {
                            var request = new RestRequest("student/{id}/", Method.PUT);
                            request.AddJsonBody(student);
                            request.AddUrlSegment("id", this.student.Id.ToString());
                            Common.addHeaders(request);
                            client.ExecuteAsync<Student>(request, response =>
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                        new ThreadStart(delegate
                                        {
                                            student = response.Data;
                                            StudentNoReg.Content = student.Student_no.ToString();
                                            StudentNameReg.Content = student.Student_name;
                                            StudentClassReg.Content = student.Student_class.ToString();
                                            SubjectsReg.Content = student.Subjects;
                                            StudentFeesOfferReg.Content = student.Fees_offer.ToString();
                                            
                                            TermRegistration mTermReg = TermRegResponse.Data[0];
                                            mTermReg.Sponsor = student.Sponsor;
                                            mTermReg.Student_class = student.Student_class;
                                            mTermReg.Fees_offer = student.Fees_offer;
                                            var cascadeRequest = new RestRequest("terms/{id}/", Method.PUT);
                                            Common.addHeaders(cascadeRequest);
                                            cascadeRequest.AddJsonBody(mTermReg);
                                            cascadeRequest.AddUrlSegment("id", mTermReg.Id.ToString());
                                            client.ExecuteAsync<TermRegistration>(cascadeRequest, cascadeResponse =>
                                            {
                                                var SponsorRequest = new RestRequest("sponsor/{id}/", Method.GET);
                                                Common.addHeaders(SponsorRequest);
                                                SponsorRequest.AddUrlSegment("id", cascadeResponse.Data.Sponsor.ToString());
                                                client.ExecuteAsync<Sponsor>(SponsorRequest, SponsorResponse =>
                                                {
                                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                new ThreadStart(delegate
                                                {
                                                    StudentSponsorReg.Content = SponsorResponse.Data.Name;
                                                }));
                                                });


                                            });

                                            if ((this.student.Student_class == 4 && student.Student_class != 4) ||
                                            (this.student.Student_class == 6 && student.Student_class != 6))
                                            {
                                                var ExamDumpRequest = new RestRequest("dump_exams/{id}/", Method.GET);
                                                Common.addHeaders(ExamDumpRequest);
                                                ExamDumpRequest.AddUrlSegment("id", student.Id.ToString());
                                                client.ExecuteAsync<int>(ExamDumpRequest, ExamDumpResponse =>
                                                {

                                                });
                                            }
                                        }));
                            });
                        }
                        else
                        {
                            MessageBox.Show("No student registration record for this session,\nRegister student and try again.");
                        }

                    });
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (FormatException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    MessageBox.Show("Invalid Data");
                }
            }
            else
            {
                MessageBox.Show("Invalid Data");
            }
        }

        private void StudentClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (((int)StudentClassComboBox.SelectedItem) < 5)
            {
                subjects.Clear();
                subjects.Add("ALL");
                SubjectsComboBox.SelectedIndex = 0;
                SubjectsComboBox.SelectedItem = student.Subjects;
            }
            else if (((int)StudentClassComboBox.SelectedItem) > 4)
            {
                subjects.Clear();
                subjects.Add("ARTS");
                subjects.Add("SCIENCES");
                SubjectsComboBox.SelectedIndex = 0;
                SubjectsComboBox.SelectedItem = student.Subjects;
            }
        }
    }
}
