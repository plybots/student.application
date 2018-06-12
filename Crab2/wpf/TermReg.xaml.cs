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
    /// Interaction logic for TermReg.xaml
    /// </summary>
    public partial class TermReg : Window
    {
        private Session CurrentSession;
        private int SELECTED_STUDENT;
        private TermRegistration mTermReg = null;
        public TermReg()
        {
            InitializeComponent();

            CurrentSession = (Session)Registry.getInstance().getReference(typeof(Session).FullName);
            SELECTED_STUDENT = (int)Registry.getInstance().getReference("STUDENT");

            SetTermInitials();
        }

        public void SetTermInitials()
        {

            if (CurrentSession != null)
            {
               
                TermRegTermBlock.Text = CurrentSession.Term.ToString();
                TermRegYearBlock.Text = CurrentSession.Year.ToString();
            }
        }

        private void TermRegOffering_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> offerings = new List<string>();
            offerings.Add("DAY");
            offerings.Add("BOARDING");

            foreach (string offer in offerings)
            {
                TermRegOffering.Items.Add(offer);
            }

            var client = new RestClient(Common.getConnectionString());

            var request2 = new RestRequest("terms/", Method.GET);
            Common.addHeaders(request2);
            request2.AddParameter("student", SELECTED_STUDENT.ToString());
            var asyncHandle2 = client.ExecuteAsync<List<TermRegistration>>(request2, response2 =>
            {
                Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Background,
                       new ThreadStart(
                           delegate
                           {
                               if (response2.Data.Count > 0)
                               {
                                   TermRegOffering.SelectedItem = response2.Data[0].Offering;
                                   mTermReg = response2.Data[0];
                               }
                               else
                               {
                                   var request = new RestRequest("student_last_reg/{id}/", Method.GET);
                                   Common.addHeaders(request);
                                   request.AddUrlSegment("id", SELECTED_STUDENT.ToString());
                                   client.ExecuteAsync<TermRegistration>(request, response =>
                                   {
                                       Application.Current.Dispatcher.Invoke(
                                             DispatcherPriority.Background,
                                             new ThreadStart(
                                                 delegate
                                                 {
                                                     TermRegOffering.SelectedItem = response.Data.Offering;
                                                 }));
                                   });
                               }
                               
                           }));
            });

        }

        private void TermRegSaveButton_Click(object sender, RoutedEventArgs e)
        {
            TermRegistration reg = new TermRegistration();
            reg.Term = CurrentSession.Term;
            reg.Year = CurrentSession.Year;
            reg.Offering = (string)TermRegOffering.SelectedItem;
            reg.Student = SELECTED_STUDENT;
            if (mTermReg != null)
            {
                mTermReg.Offering = (string)TermRegOffering.SelectedItem;
            }
            var client = new RestClient(Common.getConnectionString());
            if (mTermReg != null)
            {
                var request3 = new RestRequest("terms/{id}/", Method.PUT);
                Common.addHeaders(request3);
                request3.AddJsonBody(mTermReg);
                request3.AddUrlSegment("id", mTermReg.Id.ToString());
                var asyncHandle3 = client.ExecuteAsync<TermRegistration>(request3, response3 =>
                {
                    mTermReg = response3.Data;                        
                });
            }
            else
            {
                
                var request = new RestRequest("terms/", Method.POST);
                Common.addHeaders(request);
                request.AddJsonBody(reg);
                var asyncHandle = client.ExecuteAsync<TermRegistration>(request, response =>
                {
                    mTermReg = response.Data;
                });
            }
        }
    }
}
