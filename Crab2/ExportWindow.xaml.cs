using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
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

namespace CRABSTUDENT
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        public ExportWindow()
        {
            InitializeComponent();
            export();
        }

        private void export()
        {
            
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest();
            Common.addHeaders(request);

            if (((string)Registry.getInstance().getReference("EXPORT_DATA_TYPE")).Equals("PAYMENTS"))
            {
                request = new RestRequest("export_payments/", Method.POST);
                request.AddJsonBody((List<Student>)Registry.getInstance().getReference("STUDENTS_TO_EXPORT"));
                client.ExecuteAsync<List<List<Object>>>(request, response =>
                {
                    DataExporter exporter = new DataExporter(response.Data);
                    exporter.Export2Excel();
                    Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new ThreadStart(
                                delegate
                                {
                                    Close();
                                }));
                });

            }
            else if (((string)Registry.getInstance().getReference("EXPORT_DATA_TYPE")).Equals("BALANCES"))
            {
                request = new RestRequest("export_balances/", Method.POST);
                request.AddJsonBody((List<Student>)Registry.getInstance().getReference("STUDENTS_TO_EXPORT"));
                client.ExecuteAsync<List<List<Object>>>(request, response =>
                {
                    BalancesExporter exporter = new BalancesExporter(response.Data);
                    exporter.Export2Excel();
                    Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new ThreadStart(
                                delegate
                                {
                                    Close();
                                }));
                });
            }
            
            
        }
    }
}
