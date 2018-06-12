using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CRABSTUDENT.wpf
{
    /// <summary>
    /// Interaction logic for SponsorReceipt.xaml
    /// </summary>
    public partial class SponsorReceipt : Window
    {
        private ObservableCollection<Sponsor> Sponsors;
        private Receipt selectedReceipt;
        private Sponsor selectedSponsor;

        public SponsorReceipt()
        {
            InitializeComponent();
            selectedReceipt = (Receipt)Registry.getInstance().getReference("SPONSOR_RECEIPT");
            Registry.getInstance().remove("SPONSOR_RECEIPT");
            GetSponsors();
            SetValuesToEdit();
        }

        private void GetSponsors()
        {
            var client = new RestClient(Common.getConnectionString());
            var SponsorRequest = new RestRequest("sponsor/", Method.GET);
            Common.addHeaders(SponsorRequest);
            var StudentReceiptAsyncHandle = client.ExecuteAsync<List<Sponsor>>(SponsorRequest, response =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(delegate
                                        {

                                            try
                                            {
                                                
                                                Sponsors = new ObservableCollection<Sponsor>();
                                                Sponsors.Clear();
                                                Sponsor dummy = new Sponsor();
                                                dummy.Name = "SPONSOR";
                                                Sponsors.Add(dummy);
                                                
                                                foreach(Sponsor sponsor in response.Data)
                                                {
                                                    if(sponsor.Name.Equals("PRIVATE", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        Sponsors.Remove(sponsor);
                                                    }
                                                    else
                                                    {
                                                        Sponsors.Add(sponsor);
                                                    }
                                                    if(selectedReceipt != null)
                                                    {
                                                        if (selectedReceipt.Sponsor == sponsor.Id)
                                                            selectedSponsor = sponsor;
                                                    }
                                                }

                                                ReceiptSponsor.ItemsSource = Sponsors;
                                                
                                                if(selectedReceipt != null)
                                                {
                                                    ReceiptSponsor.SelectedItem = selectedSponsor;
                                                }
                                                else
                                                {
                                                    ReceiptSponsor.SelectedIndex = 0;
                                                }
                                            }
#pragma warning disable CS0168 // Variable is declared but never used
                                            catch (ArgumentNullException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used

                                        }));

            });
        }

        private void SetValuesToEdit()
        {
            if(selectedReceipt != null)
            {
                
                ReceiptAmount.Text = selectedReceipt.Payment.ToString();
                ReceiptDate.SelectedDate = Convert.ToDateTime(selectedReceipt.Receipt_date);
                ReceiptNo.Text = selectedReceipt.Receipt_number;

            }
        }


        private void SponsorReceiptSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(ReceiptSponsor.SelectedIndex > 0)
            {
                if (!string.IsNullOrWhiteSpace(ReceiptAmount.Text) && !string.IsNullOrWhiteSpace(ReceiptNo.Text))
                {
                    try
                    {

                        int amount = Convert.ToInt32(ReceiptAmount.Text);
                        if(amount > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(ReceiptDate.SelectedDate.Value.ToString("yyyy-MM-dd")) && 
                                Convert.ToDateTime(ReceiptDate.SelectedDate.Value.ToString("yyyy-MM-dd")) <= DateTime.Now)
                            {
                                var client = new RestClient(Common.getConnectionString());
                                if (selectedReceipt != null)
                                {
                                    
                                    selectedReceipt.Payment = Convert.ToInt32(ReceiptAmount.Text);
                                    selectedReceipt.Receipt_date = ReceiptDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                                    selectedReceipt.Receipt_number = ReceiptNo.Text;

                                    var ReceiptRequest = new RestRequest("sponsor_receipts/{id}/", Method.PUT);
                                    Common.addHeaders(ReceiptRequest);
                                    ReceiptRequest.AddJsonBody(selectedReceipt);
                                    ReceiptRequest.AddUrlSegment("id", selectedReceipt.Id.ToString());
                                    client.ExecuteAsync<Receipt>(ReceiptRequest, ReceiptResponse =>
                                    {
                                        Application.Current.Dispatcher.Invoke(
                                               DispatcherPriority.Background,
                                               new ThreadStart(
                                                   delegate
                                                   {
                                                       if (!ReceiptResponse.StatusCode.ToString().Equals("BadRequest"))
                                                       {
                                                           Registry.getInstance().register(ReceiptSponsor.SelectedItem, "NEW_RECEIPT_SPONSOR");
                                                           MessageBox.Show("Request Executed Successfully");
                                                       }
                                                       else
                                                       {
                                                           MessageBox.Show("Bad Receipt Number");
                                                       }
                                                           

                                                   }));
                                    });
                                }
                                else
                                {
                                    
                                    var SessionRequest = new RestRequest("sessions/", Method.GET);
                                    Common.addHeaders(SessionRequest);
                                    SessionRequest.AddParameter("now", "true");
                                    client.ExecuteAsync<List<Session>>(SessionRequest, SessionResponse =>
                                    {
                                        Application.Current.Dispatcher.Invoke(
                                               DispatcherPriority.Background,
                                               new ThreadStart(
                                                   delegate
                                                   {
                                                       Session session = SessionResponse.Data[0];
                                                       Receipt receipt = new Receipt();
                                                       receipt.Payment = Convert.ToInt32(ReceiptAmount.Text);
                                                       receipt.Receipt_date = ReceiptDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                                                       receipt.Receipt_number = ReceiptNo.Text;
                                                       receipt.Session = session.Id;
                                                       receipt.Sponsor = ((Sponsor)ReceiptSponsor.SelectedItem).Id;

                                                       var ReceiptRequest = new RestRequest("sponsor_receipts/", Method.POST);
                                                       Common.addHeaders(ReceiptRequest);
                                                       ReceiptRequest.AddJsonBody(receipt);
                                                       client.ExecuteAsync<Receipt>(ReceiptRequest, ReceiptResponse =>
                                                       {
                                                           Application.Current.Dispatcher.Invoke(
                                                                  DispatcherPriority.Background,
                                                                  new ThreadStart(
                                                                      delegate
                                                                      {
                                                                          Registry.getInstance().register(ReceiptSponsor.SelectedItem, "NEW_RECEIPT_SPONSOR");
                                                                          MessageBox.Show("Request Executed Successfully");

                                                                      }));
                                                       });

                                                   }));
                                    });
                                }

                            }
                            else
                            {
                                MessageBox.Show("Invalid Receipt Data");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid Receipt Data");
                        }

#pragma warning disable CS0168 // Variable is declared but never used
                    }
                    catch (FormatException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
                        MessageBox.Show("Invalid Receipt Data");
                    }
                }
            }
            else
            {
                MessageBox.Show("No Sponsor Selected");
            }
        }
    }
}
