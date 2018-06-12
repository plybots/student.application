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

namespace CRABSTUDENT.wpf
{
    /// <summary>
    /// Interaction logic for ReceiptUpdate.xaml
    /// </summary>
    public partial class ReceiptUpdate : Window
    {
        Receipt receipt;

        //Receipt Form Values
        private int RECEIPT_AMOUNT;
        private int ADMISSION;
        private int TUITION;
        private int DEVELOPMENT;
        private int COMPUTER;

        private int UACE;
        private int UCE;
        private int MOCK;

        private int TEXT_BOOK_COST;
        private string TEXT_BOOK_NAME;
        private string TEXT_BOOK_ROLE;

        private int CLASS_UNIFORM;
        private int CASUAL_UNIFORM;
        private int SWEATER_UNIFORM;
        private int SPORTS_UNIFORM;
        private int JOGGING_UNIFORM;
        private int T_SHIRT;
        private int BADGE;

        private int PASSPORT_PHOTOS;
        private int ID;
        private int HAIR_CUTTING;
        private int LIBRARY;
        private int MEDICAL;
        private int SCOUTS;

        private int MISC_COST;
        private string MISC_ROLE;

        //form Categories
        private const string RECEIPT_CAT = "receipt";
        private const string TUITION_CAT = "tuition";
        private const string TEXT_BOOK_CAT = "textbook";
        private const string UNIFORM_CAT = "uinform";
        private const string EXAM_CAT = "exam";
        private const string MISC_CAT = "misc";
        private const string OTHERS_CAT = "others";
        private TuitionFees current_tuition;
        private Exam current_exams;
        private Uniform current_uniform;
        private Others current_others;
        private TextBooks current_textbooks;
        private Misc current_misc;
        private Session ReceiptSession;

        public ReceiptUpdate()
        {
            InitializeComponent();
            receipt = (Receipt)Registry.getInstance().getReference("SELECTED_RECEIPT");
            ReceiptSession = (Session)Registry.getInstance().getReference("ACTIVE_SESSION");
            PopulateReceipt();
        }

        private List<string> getControls(string category)
        {
            List<string> controls = new List<string>();

            switch (category)
            {
                case RECEIPT_CAT:
                    controls.Add(ReceiptAmount.Text);
                    return controls;

                case TUITION_CAT:
                    controls.Add(ReceiptTuition.Text);
                    controls.Add(ReceiptAdmission.Text);
                    controls.Add(ReceiptDevelopment.Text);
                    controls.Add(ReceiptComputer.Text);
                    return controls;
                case EXAM_CAT:
                    controls.Add(ReceiptUCE.Text);
                    controls.Add(ReceiptUACE.Text);
                    controls.Add(ReceiptMock.Text);
                    return controls;

                case TEXT_BOOK_CAT:
                    controls.Add(ReceiptTextBookCost.Text);
                    return controls;

                case UNIFORM_CAT:
                    controls.Add(ReceiptClassUniform.Text);
                    controls.Add(ReceiptCasualUniform.Text);
                    controls.Add(ReceiptSportsUniform.Text);
                    controls.Add(ReceiptSweaterUniform.Text);
                    controls.Add(ReceiptJoggingUniform.Text);
                    controls.Add(ReceiptTShirtUniform.Text);
                    controls.Add(ReceiptUniformBadge.Text);
                    return controls;

                case OTHERS_CAT:
                    controls.Add(ReceiptPassportPhotos.Text);
                    controls.Add(ReceiptIdentityCard.Text);
                    controls.Add(ReceiptHairCutting.Text);
                    controls.Add(ReceiptLibrary.Text);
                    controls.Add(ReceiptMedical.Text);
                    controls.Add(ReceiptScouts.Text);
                    return controls;

                case MISC_CAT:
                    controls.Add(ReceiptMiscCost.Text);
                    return controls;

                default:
                    return controls;
            }
        }

        private void SetFormValues()
        {
            RECEIPT_AMOUNT = Convert.ToInt32(ReceiptAmount.Text);
            ADMISSION = Convert.ToInt32(ReceiptAdmission.Text);
            TUITION = Convert.ToInt32(ReceiptTuition.Text);
            DEVELOPMENT = Convert.ToInt32(ReceiptDevelopment.Text);
            COMPUTER = Convert.ToInt32(ReceiptComputer.Text);

            UACE = Convert.ToInt32(ReceiptUACE.Text);
            UCE = Convert.ToInt32(ReceiptUCE.Text);
            MOCK = Convert.ToInt32(ReceiptMock.Text);

            TEXT_BOOK_COST = Convert.ToInt32(ReceiptTextBookCost.Text);
            TEXT_BOOK_NAME = ReceiptTextBookName.Text;
            TEXT_BOOK_ROLE = ReceiptTextBookRole.Text;

            CLASS_UNIFORM = Convert.ToInt32(ReceiptClassUniform.Text);
            CASUAL_UNIFORM = Convert.ToInt32(ReceiptCasualUniform.Text);
            SWEATER_UNIFORM = Convert.ToInt32(ReceiptSweaterUniform.Text);
            SPORTS_UNIFORM = Convert.ToInt32(ReceiptSportsUniform.Text);
            JOGGING_UNIFORM = Convert.ToInt32(ReceiptJoggingUniform.Text);
            T_SHIRT = Convert.ToInt32(ReceiptTShirtUniform.Text);
            BADGE = Convert.ToInt32(ReceiptUniformBadge.Text);

            PASSPORT_PHOTOS = Convert.ToInt32(ReceiptPassportPhotos.Text);
            ID = Convert.ToInt32(ReceiptIdentityCard.Text);
            HAIR_CUTTING = Convert.ToInt32(ReceiptHairCutting.Text);
            LIBRARY = Convert.ToInt32(ReceiptLibrary.Text);
            MEDICAL = Convert.ToInt32(ReceiptMedical.Text);
            SCOUTS = Convert.ToInt32(ReceiptScouts.Text);

            MISC_COST = Convert.ToInt32(ReceiptMiscCost.Text);
            MISC_ROLE = ReceiptMiscRole.Text;
        }

        private void PopulateReceipt()
        {
            ReceiptNo.Text = receipt.Receipt_number;
            ReceiptAmount.Text = receipt.Amount_paid.ToString();
            ReceiptDate.SelectedDate = Convert.ToDateTime(receipt.Receipt_date);
            var client = new RestClient(Common.getConnectionString());

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
                                    current_tuition = TuitionResponse.Data[0];

                                    ReceiptTuition.Text =  TuitionResponse.Data[0].Tuition.ToString();
                                    ReceiptAdmission.Text = TuitionResponse.Data[0].Admission.ToString();
                                    ReceiptComputer.Text = TuitionResponse.Data[0].Computer.ToString();
                                    ReceiptDevelopment.Text = TuitionResponse.Data[0].Development_fee.ToString();
                                }

                            }));
            });

            var ExamRequest = new RestRequest("exams/", Method.GET);
            Common.addHeaders(ExamRequest);
            ExamRequest.AddParameter("receipt", receipt.Id.ToString());
            var EaxmAsyncHandle = client.ExecuteAsync<List<Exam>>(ExamRequest, ExamResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                if (ExamResponse.Data.Count > 0)
                                {
                                    current_exams = ExamResponse.Data[0];

                                    ReceiptUCE.Text = ExamResponse.Data[0].Uce.ToString();
                                    ReceiptUACE.Text = ExamResponse.Data[0].Uace.ToString();
                                    ReceiptMock.Text = ExamResponse.Data[0].Mock.ToString();
                                }

                            }));
            });

            var UniformRequest = new RestRequest("uniforms/", Method.GET);
            Common.addHeaders(UniformRequest);
            UniformRequest.AddParameter("receipt", receipt.Id.ToString());
            var UniformAsyncHandle = client.ExecuteAsync<List<Uniform>>(UniformRequest, UniformResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                if (UniformResponse.Data.Count > 0)
                                {
                                    current_uniform = UniformResponse.Data[0];

                                    ReceiptClassUniform.Text = UniformResponse.Data[0].Class_wear.ToString();
                                    ReceiptCasualUniform.Text = UniformResponse.Data[0].Casual_wear.ToString();
                                    ReceiptSportsUniform.Text = UniformResponse.Data[0].Sports_wear.ToString();
                                    ReceiptSweaterUniform.Text = UniformResponse.Data[0].Sweater.ToString();
                                    ReceiptJoggingUniform.Text = UniformResponse.Data[0].Jogging.ToString();
                                    ReceiptTShirtUniform.Text = UniformResponse.Data[0].School_t_shirt.ToString();
                                    ReceiptUniformBadge.Text = UniformResponse.Data[0].Badge.ToString();
                                }

                            }));
            });

            var OthersRequest = new RestRequest("others/", Method.GET);
            Common.addHeaders(OthersRequest);
            OthersRequest.AddParameter("receipt", receipt.Id.ToString());
            var OthersAsyncHandle = client.ExecuteAsync<List<Others>>(OthersRequest, OthersResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                if (OthersResponse.Data.Count > 0)
                                {
                                    current_others = OthersResponse.Data[0];

                                    ReceiptIdentityCard.Text = OthersResponse.Data[0].Identity_card.ToString();
                                    ReceiptPassportPhotos.Text = OthersResponse.Data[0].Passport_photos.ToString();
                                    ReceiptHairCutting.Text = OthersResponse.Data[0].Hair_cutting.ToString();
                                    ReceiptLibrary.Text = OthersResponse.Data[0].Library.ToString();
                                    ReceiptMedical.Text = OthersResponse.Data[0].Medical.ToString();
                                    ReceiptScouts.Text = OthersResponse.Data[0].Scouts.ToString();
                                }

                            }));
            });

            var TextBooksRequest = new RestRequest("textbooks/", Method.GET);
            Common.addHeaders(TextBooksRequest);
            TextBooksRequest.AddParameter("receipt", receipt.Id.ToString());
            var TextBooksAsyncHandle = client.ExecuteAsync<List<TextBooks>>(TextBooksRequest, TextBooksResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                if (TextBooksResponse.Data.Count > 0)
                                {
                                    current_textbooks = TextBooksResponse.Data[0];

                                    ReceiptTextBookName.Text = TextBooksResponse.Data[0].Name.ToString();
                                    ReceiptTextBookRole.Text = TextBooksResponse.Data[0].Role.ToString();
                                    ReceiptTextBookCost.Text = TextBooksResponse.Data[0].Cost.ToString();
                                }

                            }));
            });


            var MiscRequest = new RestRequest("misc/", Method.GET);
            Common.addHeaders(MiscRequest);
            MiscRequest.AddParameter("receipt", receipt.Id.ToString());
            var MiscAsyncHandle = client.ExecuteAsync<List<Misc>>(MiscRequest, MiscResponse =>
            {
                Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(
                            delegate
                            {
                                if (MiscResponse.Data.Count > 0)
                                {
                                    current_misc = MiscResponse.Data[0];

                                    ReceiptMiscRole.Text = MiscResponse.Data[0].Role.ToString();
                                    ReceiptMiscCost.Text = MiscResponse.Data[0].Cost.ToString();
                                }

                            }));
            });
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> NumberControls = new List<string>();
            getControls(RECEIPT_CAT).ToList().ForEach(NumberControls.Add);
            getControls(TUITION_CAT).ToList().ForEach(NumberControls.Add);
            getControls(EXAM_CAT).ToList().ForEach(NumberControls.Add);
            getControls(TEXT_BOOK_CAT).ToList().ForEach(NumberControls.Add);
            getControls(OTHERS_CAT).ToList().ForEach(NumberControls.Add);
            getControls(UNIFORM_CAT).ToList().ForEach(NumberControls.Add);
            getControls(MISC_CAT).ToList().ForEach(NumberControls.Add);

            List<string> TextControls = new List<string>();
            TextControls.Add(ReceiptTextBookName.Text);
            TextControls.Add(ReceiptTextBookRole.Text);
            TextControls.Add(ReceiptNo.Text);
            TextControls.Add(ReceiptMiscRole.Text);

            
            var client = new RestClient(Common.getConnectionString());
            var StudentRequest = new RestRequest("student/{id}/", Method.GET);
            Common.addHeaders(StudentRequest);
            StudentRequest.AddUrlSegment("id", receipt.Student.ToString());
            client.ExecuteAsync<Student>(StudentRequest, StudentResponse =>
            {
                Student Student = StudentResponse.Data;
                var StudentTuitionBFRequest = new RestRequest("bal_forwarded/1/", Method.GET);
                StudentTuitionBFRequest.AddParameter("student", Student.Id.ToString());
                Common.addHeaders(StudentTuitionBFRequest);
                client.ExecuteAsync<BalanceFowarded>(StudentTuitionBFRequest, StudentTuitionBFResponse =>
                {
                    var TermRegRequest = new RestRequest("terms/", Method.GET);
                    Common.addHeaders(TermRegRequest);
                    TermRegRequest.AddParameter("student", Student.Id.ToString());
                    client.ExecuteAsync<List<TermRegistration>>(TermRegRequest, TermRegResponse =>
                    {
                        try
                        {
                            TermRegistration termReg = TermRegResponse.Data[0];
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                 new ThreadStart(delegate
                                 {
                                     if (ReceiptValidators.ValidateFields(NumberControls, TextControls, ReceiptDate.SelectedDate.Value.ToString("yyyy-MM-dd"), ReceiptSession)
                                        && ReceiptValidators.AreExamsValid(ReceiptUACE.Text, ReceiptUCE.Text, ReceiptMock.Text, Student, StudentTuitionBFResponse.Data, termReg)
                                        && ReceiptValidators.DoReceiptValuesBalance(NumberControls, ReceiptAmount.Text))
                                     {
                                         SetFormValues();

                                         Receipt new_receipt = new Receipt();
                                         new_receipt.Receipt_number = ReceiptNo.Text;
                                         new_receipt.Receipt_date = ReceiptDate.SelectedDate.Value.ToString("yyy-MM-dd");
                                         new_receipt.Amount_paid = Convert.ToInt32(ReceiptAmount.Text);
                                         new_receipt.Student = receipt.Student;
                                         new_receipt.Session = receipt.Session;

                                         var ReceiptRequest = new RestRequest("receipts/{id}/", Method.PUT);
                                         Common.addHeaders(ReceiptRequest);
                                         ReceiptRequest.AddJsonBody(new_receipt);
                                         ReceiptRequest.AddUrlSegment("id", receipt.Id.ToString());
                                         client.ExecuteAsync<Receipt>(ReceiptRequest, ReceiptResponse =>
                                         {
                                             if (!ReceiptResponse.StatusCode.ToString().Equals("BadRequest"))
                                             {
                                                 TuitionFees tuition = new TuitionFees();
                                                 tuition.Tuition = TUITION;
                                                 tuition.Admission = ADMISSION;
                                                 tuition.Computer = COMPUTER;
                                                 tuition.Development_fee = DEVELOPMENT;
                                                 tuition.Student = receipt.Student;
                                                 tuition.Receipt = receipt.Id;
                                                 tuition.Session = receipt.Session;

                                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                        new ThreadStart(delegate
                                                        {
                                                            if (ReceiptValidators.AreFieldsSet(getControls(TUITION_CAT)))
                                                            {
                                                                try
                                                                {
                                                                    var TuitionRequest = new RestRequest("tuition/{id}/", Method.PUT);
                                                                    Common.addHeaders(TuitionRequest);
                                                                    TuitionRequest.AddUrlSegment("id", current_tuition.Id.ToString());
                                                                    TuitionRequest.AddJsonBody(tuition);
                                                                    var asyncHandle = client.ExecuteAsync<TuitionFees>(TuitionRequest, TuitionResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                {
                                                                    var TuitionRequest = new RestRequest("tuition/", Method.POST);
                                                                    Common.addHeaders(TuitionRequest);
                                                                    TuitionRequest.AddJsonBody(tuition);
                                                                    var asyncHandle = client.ExecuteAsync<TuitionFees>(TuitionRequest, TuitionResponse =>
                                                                    {

                                                                    });
                                                                }

                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    var TuitionRequest = new RestRequest("tuition/{id}/", Method.DELETE);
                                                                    Common.addHeaders(TuitionRequest);
                                                                    TuitionRequest.AddUrlSegment("id", current_tuition.Id.ToString());
                                                                    var asyncHandle = client.ExecuteAsync<TuitionFees>(TuitionRequest, TuitionResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                                                            }
                                                        }));

                                                 Exam exam = new Exam();
                                                 exam.Receipt = receipt.Id;
                                                 exam.Student = receipt.Student;
                                                 exam.Mock = MOCK;
                                                 exam.Uace = UACE;
                                                 exam.Uce = UCE;
                                                 exam.Session = receipt.Session;

                                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                        new ThreadStart(delegate
                                                        {
                                                            if (ReceiptValidators.AreFieldsSet(getControls(EXAM_CAT)))
                                                            {
                                                                try
                                                                {
                                                                    var ExamRequest = new RestRequest("exams/{id}/", Method.PUT);
                                                                    Common.addHeaders(ExamRequest);
                                                                    ExamRequest.AddJsonBody(exam);
                                                                    ExamRequest.AddUrlSegment("id", current_exams.Id.ToString());
                                                                    var EaxmAsyncHandle = client.ExecuteAsync<Exam>(ExamRequest, ExamResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                {
                                                                    var ExamRequest = new RestRequest("exams/", Method.POST);
                                                                    Common.addHeaders(ExamRequest);
                                                                    ExamRequest.AddJsonBody(exam);
                                                                    var EaxmAsyncHandle = client.ExecuteAsync<Exam>(ExamRequest, ExamResponse =>
                                                                    {

                                                                    });
                                                                }

                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    var ExamRequest = new RestRequest("exams/{id}/", Method.DELETE);
                                                                    Common.addHeaders(ExamRequest);
                                                                    ExamRequest.AddUrlSegment("id", current_exams.Id.ToString());
                                                                    var EaxmAsyncHandle = client.ExecuteAsync<Exam>(ExamRequest, ExamResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                                                            }
                                                        }));


                                                 Uniform uniform = new Uniform();
                                                 uniform.Receipt = receipt.Id;
                                                 uniform.Student = receipt.Student;
                                                 uniform.Class_wear = CLASS_UNIFORM;
                                                 uniform.Casual_wear = CASUAL_UNIFORM;
                                                 uniform.Sports_wear = SPORTS_UNIFORM;
                                                 uniform.Sweater = SWEATER_UNIFORM;
                                                 uniform.Jogging = JOGGING_UNIFORM;
                                                 uniform.School_t_shirt = T_SHIRT;
                                                 uniform.Badge = BADGE;
                                                 uniform.Session = receipt.Session;

                                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                        new ThreadStart(delegate
                                                        {
                                                            if (ReceiptValidators.AreFieldsSet(getControls(UNIFORM_CAT)))
                                                            {
                                                                try
                                                                {
                                                                    var UniformRequest = new RestRequest("uniforms/{id}/", Method.PUT);
                                                                    Common.addHeaders(UniformRequest);
                                                                    UniformRequest.AddJsonBody(uniform);
                                                                    UniformRequest.AddUrlSegment("id", current_uniform.Id.ToString());
                                                                    var UniformAsyncHandle = client.ExecuteAsync<Uniform>(UniformRequest, UniformResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                {
                                                                    var UniformRequest = new RestRequest("uniforms/", Method.POST);
                                                                    Common.addHeaders(UniformRequest);
                                                                    UniformRequest.AddJsonBody(uniform);
                                                                    var UniformAsyncHandle = client.ExecuteAsync<Uniform>(UniformRequest, UniformResponse =>
                                                                    {

                                                                    });
                                                                }

                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    var UniformRequest = new RestRequest("uniforms/{id}/", Method.DELETE);
                                                                    Common.addHeaders(UniformRequest);
                                                                    UniformRequest.AddUrlSegment("id", current_uniform.Id.ToString());
                                                                    var UniformAsyncHandle = client.ExecuteAsync<Uniform>(UniformRequest, UniformResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                                                            }
                                                        }));


                                                 Others others = new Others();
                                                 others.Receipt = receipt.Id;
                                                 others.Student = receipt.Student;
                                                 others.Identity_card = ID;
                                                 others.Passport_photos = PASSPORT_PHOTOS;
                                                 others.Hair_cutting = HAIR_CUTTING;
                                                 others.Library = LIBRARY;
                                                 others.Medical = MEDICAL;
                                                 others.Scouts = SCOUTS;
                                                 others.Session = receipt.Session;

                                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                        new ThreadStart(delegate
                                                        {
                                                            if (ReceiptValidators.AreFieldsSet(getControls(OTHERS_CAT)))
                                                            {
                                                                try
                                                                {
                                                                    var OthersRequest = new RestRequest("others/{id}/", Method.PUT);
                                                                    Common.addHeaders(OthersRequest);
                                                                    OthersRequest.AddJsonBody(others);
                                                                    OthersRequest.AddUrlSegment("id", current_others.Id.ToString());
                                                                    var OthersAsyncHandle = client.ExecuteAsync<Others>(OthersRequest, OthersResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                {
                                                                    var OthersRequest = new RestRequest("others/", Method.POST);
                                                                    Common.addHeaders(OthersRequest);
                                                                    OthersRequest.AddJsonBody(others);
                                                                    var OthersAsyncHandle = client.ExecuteAsync<Others>(OthersRequest, OthersResponse =>
                                                                    {

                                                                    });
                                                                }

                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    var OthersRequest = new RestRequest("others/{id}/", Method.DELETE);
                                                                    Common.addHeaders(OthersRequest);
                                                                    OthersRequest.AddUrlSegment("id", current_others.Id.ToString());
                                                                    var OthersAsyncHandle = client.ExecuteAsync<Others>(OthersRequest, OthersResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                                                            }
                                                        }));


                                                 TextBooks textbook = new TextBooks();
                                                 textbook.Receipt = receipt.Id;
                                                 textbook.Student = receipt.Student;
                                                 textbook.Cost = TEXT_BOOK_COST;
                                                 textbook.Name = TEXT_BOOK_NAME;
                                                 textbook.Role = TEXT_BOOK_ROLE;
                                                 textbook.Session = receipt.Session;

                                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                       new ThreadStart(delegate
                                                       {
                                                           if (ReceiptValidators.AreFieldsSet(getControls(TEXT_BOOK_CAT)))
                                                           {
                                                               try
                                                               {
                                                                   var TextBooksRequest = new RestRequest("textbooks/{id}/", Method.PUT);
                                                                   Common.addHeaders(TextBooksRequest);
                                                                   TextBooksRequest.AddJsonBody(textbook);
                                                                   TextBooksRequest.AddUrlSegment("id", current_textbooks.Id.ToString());
                                                                   var TextBooksAsyncHandle = client.ExecuteAsync<TextBooks>(TextBooksRequest, TextBooksResponse =>
                                                                   {

                                                                   });
                                                               }
#pragma warning disable CS0168 // Variable is declared but never used
                                                               catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                               {
                                                                   var TextBooksRequest = new RestRequest("textbooks/", Method.POST);
                                                                   Common.addHeaders(TextBooksRequest);
                                                                   TextBooksRequest.AddJsonBody(textbook);
                                                                   var TextBooksAsyncHandle = client.ExecuteAsync<TextBooks>(TextBooksRequest, TextBooksResponse =>
                                                                   {

                                                                   });
                                                               }

                                                           }
                                                           else
                                                           {
                                                               try
                                                               {
                                                                   var TextBooksRequest = new RestRequest("textbooks/{id}/", Method.DELETE);
                                                                   Common.addHeaders(TextBooksRequest);
                                                                   TextBooksRequest.AddUrlSegment("id", current_textbooks.Id.ToString());
                                                                   var TextBooksAsyncHandle = client.ExecuteAsync<TextBooks>(TextBooksRequest, TextBooksResponse =>
                                                                   {

                                                                   });
                                                               }
#pragma warning disable CS0168 // Variable is declared but never used
                                                               catch (NullReferenceException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
                                                           }
                                                       }));


                                                 Misc misc = new Misc();
                                                 misc.Receipt = receipt.Id;
                                                 misc.Student = receipt.Student;
                                                 misc.Role = MISC_ROLE;
                                                 misc.Cost = MISC_COST;
                                                 misc.Session = receipt.Session;

                                                 Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                        new ThreadStart(delegate
                                                        {
                                                            if (ReceiptValidators.AreFieldsSet(getControls(MISC_CAT)))
                                                            {
                                                                try
                                                                {
                                                                    var MiscRequest = new RestRequest("misc/{id}/", Method.PUT);
                                                                    Common.addHeaders(MiscRequest);
                                                                    MiscRequest.AddJsonBody(misc);
                                                                    MiscRequest.AddUrlSegment("id", current_misc.Id.ToString());
                                                                    var MiscAsyncHandle = client.ExecuteAsync<Misc>(MiscRequest, MiscResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                {
                                                                    var MiscRequest = new RestRequest("misc/", Method.POST);
                                                                    Common.addHeaders(MiscRequest);
                                                                    MiscRequest.AddJsonBody(misc);
                                                                    var MiscAsyncHandle = client.ExecuteAsync<Misc>(MiscRequest, MiscResponse =>
                                                                    {

                                                                    });
                                                                }

                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    var MiscRequest = new RestRequest("misc/{id}/", Method.DELETE);
                                                                    Common.addHeaders(MiscRequest);
                                                                    MiscRequest.AddUrlSegment("id", current_misc.Id.ToString());
                                                                    var MiscAsyncHandle = client.ExecuteAsync<Misc>(MiscRequest, MiscResponse =>
                                                                    {

                                                                    });
                                                                }
#pragma warning disable CS0168 // Variable is declared but never used
                                                                catch (NullReferenceException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                                                {

                                                                }
                                                            }
                                                        }));

                                             }
                                             else
                                             {
                                                 MessageBox.Show("Invalid Receipt Number");
                                             }

                                         });
                                     }
                                     else
                                     {
                                         MessageBox.Show("Form is Invalid.\nPossible Causes:\n1. Some fields are not filled.\n" +
                                             "2. The Receipt Amount is not equal to the total of the breakdown\n" +
                                             "3. Examination and class mismatch.");
                                     }
                                 }));
#pragma warning disable CS0168 // Variable is declared but never used
                        }
                        catch (ArgumentOutOfRangeException exx)
                        {
                            MessageBox.Show("Failed to retrieve student registration");
                        }
#pragma warning restore CS0168 // Variable is declared but never used
                    });

                });
                
            });
        }
    }
}
