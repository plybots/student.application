using RestSharp;
using CRABSTUDENT.model.entity;
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
    /// Interaction logic for SwitchSessions.xaml
    /// </summary>
    public partial class SwitchSessions : Window
    {
        private ObservableCollection<Session> sessions { get; set; }
        private Session SELECTED_SESSION = null;
        public SwitchSessions()
        {
            InitializeComponent();
        }

        private void listView_Loaded(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(Common.getConnectionString());
            var request = new RestRequest("sessions/", Method.GET);
            Common.addHeaders(request);
            client.ExecuteAsync<List<Session>>(request, response =>
            {
                Application.Current.Dispatcher.Invoke(
                           DispatcherPriority.Background,
                           new ThreadStart(
                               delegate
                               {
                                   sessions = new ObservableCollection<Session>();
                                   sessions.Clear();
                                   response.Data.ToList().ForEach(sessions.Add);
                                   listView.ItemsSource = sessions;

                               }));
            });
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SELECTED_SESSION = (Session)listView.SelectedItem;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (SELECTED_SESSION != null)
            {
                var client = new RestClient(Common.getConnectionString());
                var request = new RestRequest("switch_sessions/", Method.POST);
                request.AddJsonBody(SELECTED_SESSION);
                Common.addHeaders(request);
                client.ExecuteAsync<Session>(request, response =>
                {
                    Application.Current.Dispatcher.Invoke(
                               DispatcherPriority.Background,
                               new ThreadStart(
                                   delegate
                                   {
                                       MessageBoxResult AskToUpdate = MessageBox.Show("Selected session activated.\nSandstorm will now restart....", "CRABSTUDENT", MessageBoxButton.OK);
                                       if (AskToUpdate == MessageBoxResult.OK)
                                       {
                                           Application.Current.Shutdown();
                                           System.Windows.Forms.Application.Restart();

                                       }

                                   }));
                });
            }
        }
    }
}
