using RestSharp;
using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
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
    /// Interaction logic for SponsorManager.xaml
    /// </summary>
    public partial class SponsorManager : Window
    {
        private Sponsor SELECTED_SPONSOR = null;

        public SponsorManager()
        {
            InitializeComponent();

            SELECTED_SPONSOR = (Sponsor)Registry.getInstance().getReference("SELECTED_SPONSOR_TO_MANAGE");

            if (SELECTED_SPONSOR != null)
            {
                
                DataContext = SELECTED_SPONSOR;
                Registry.getInstance().remove("SELECTED_SPONSOR_TO_MANAGE");

            }else
            {
                DataContext = new Sponsor();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(SELECTED_SPONSOR == null)
            {
                if (!string.IsNullOrWhiteSpace(SponsorName.Text))
                {
                    var client = new RestClient(Common.getConnectionString());
                    var request = new RestRequest("sponsor/", Method.POST);
                    Common.addHeaders(request);
                    request.AddJsonBody(DataContext);
                    var asyncHandle = client.ExecuteAsync<Sponsor>(request, response =>
                    {
                    });
                }
                else
                {
                    MessageBox.Show("Sponsor Name is required");
                    SponsorName.BorderBrush = Brushes.Red;
                }
                
            }
            else
            {
                var client = new RestClient(Common.getConnectionString());
                var request = new RestRequest("sponsor/{id}/", Method.PUT);
                Common.addHeaders(request);
                request.AddJsonBody(DataContext);
                request.AddUrlSegment("id", SELECTED_SPONSOR.Id.ToString());
                var asyncHandle = client.ExecuteAsync<Sponsor>(request, response =>
                {
                });
            }
            
        }
    }
}
