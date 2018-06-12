using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
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
    /// Interaction logic for BalanceForwarded.xaml
    /// </summary>
    public partial class BalanceForwarded : Window
    {
        private Student student;
        private BalanceFowarded bf;
        private Session session;
        private RestClient client;

        public BalanceForwarded()
        {
            InitializeComponent();
            student = (Student)Registry.getInstance().getReference("SELECTED_STUDENT");
            client = new RestClient(Common.getConnectionString());
            InitializeForm();
        }

        private void InitializeForm()
        {
            var StudentTuitionBFRequest = new RestRequest("bf_original/1/", Method.GET);
            StudentTuitionBFRequest.AddParameter("student", student.Id.ToString());
            Common.addHeaders(StudentTuitionBFRequest);
            client.ExecuteAsync<BalanceFowarded>(StudentTuitionBFRequest, StudentTuitionBFResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                bf = StudentTuitionBFResponse.Data;
                                BFTuition.Text = bf.Tuition.ToString();
                                BFDevelopment.Text = bf.Development_fee.ToString();
                                BFAdmission.Text = bf.Admission.ToString();
                                BFComputer.Text = bf.Computer.ToString();
                                BFUCE.Text = bf.Uce.ToString();
                                BFUACE.Text = bf.Uace.ToString();
                                BFMock.Text = bf.Mock.ToString();

                            }));
            });
            
            var request2 = new RestRequest("sessions/", Method.GET);
            Common.addHeaders(request2);
            request2.AddParameter("now", "true");
            client.ExecuteAsync<List<Session>>(request2, response2 =>
            {
                try
                {
                    session = response2.Data[0];
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (IndexOutOfRangeException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                {

                }
                
            });
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BalanceFowarded bal = new BalanceFowarded();
                bal.Tuition = Convert.ToInt32(BFTuition.Text);
                bal.Development_fee = Convert.ToInt32(BFDevelopment.Text);
                bal.Admission = Convert.ToInt32(BFAdmission.Text);
                bal.Computer = Convert.ToInt32(BFComputer.Text);
                bal.Uce = Convert.ToInt32(BFUCE.Text);
                bal.Uace = Convert.ToInt32(BFUACE.Text);
                bal.Mock = Convert.ToInt32(BFMock.Text);
                bal.Manual = true;

                if (bf.Id == 0)
                {
                    bal.Session = session.Id;
                    bal.Student = student.Id;
                    var StudentTuitionBFRequest = new RestRequest("bf_original/", Method.POST);
                    StudentTuitionBFRequest.AddJsonBody(bal);
                    Common.addHeaders(StudentTuitionBFRequest);
                    client.ExecuteAsync<BalanceFowarded>(StudentTuitionBFRequest, StudentTuitionBFResponse =>
                    {
                        MessageBox.Show("Request Excecuted");
                    });

                }
                else
                {
                    bal.Session = bf.Session;
                    bal.Student = bf.Student;
                    var StudentTuitionBFRequest = new RestRequest("bf_original/{id}/", Method.PUT);
                    StudentTuitionBFRequest.AddJsonBody(bal);
                    StudentTuitionBFRequest.AddUrlSegment("id", bf.Id.ToString());
                    Common.addHeaders(StudentTuitionBFRequest);
                    client.ExecuteAsync<BalanceFowarded>(StudentTuitionBFRequest, StudentTuitionBFResponse =>
                    {
                        MessageBox.Show("Request Excecuted");
                    });
                }
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (FormatException ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                MessageBox.Show("Incorrect Form Data");
            }
        }
    }
}
