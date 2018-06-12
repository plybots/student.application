using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CRABSTUDENT
{
    /// <summary>
    /// Interaction logic for NewReceiptWindow.xaml
    /// </summary>

    public partial class ReceiptWindow : Window
    {
        private Session CurrentSession;
        private int SELECTED_STUDENT;

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

        //last category for posting
        private string LAST_CATEGORY = "none";

        public ReceiptWindow()
        {
            InitializeComponent();

            CurrentSession = (Session)Registry.getInstance().getReference("ACTIVE_SESSION");
            SELECTED_STUDENT = (int)Registry.getInstance().getReference("STUDENT");
            GetStudent();
        }

        private void GetStudent()
        {
            var client = new RestClient(Common.getConnectionString());
            var StudentRequest = new RestRequest("student/{id}/", Method.GET);
            Common.addHeaders(StudentRequest);
            StudentRequest.AddUrlSegment("id", SELECTED_STUDENT.ToString());
            var TuitionAsyncHandle = client.ExecuteAsync<Student>(StudentRequest, StudentResponse =>
            {
                Student Student = StudentResponse.Data;
            });
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

        /**
        Determines the last category to be posted to help
            redirection to show results when recording is complete.
        **/
        private string LastRecordForPosting()
        {
            if (ReceiptValidators.AreFieldsSet(getControls(MISC_CAT)))
                return MISC_CAT;
            if (ReceiptValidators.AreFieldsSet(getControls(UNIFORM_CAT)))
                return UNIFORM_CAT;
            if (ReceiptValidators.AreFieldsSet(getControls(OTHERS_CAT)))
                return OTHERS_CAT;
            if (ReceiptValidators.AreFieldsSet(getControls(TEXT_BOOK_CAT)))
                return TEXT_BOOK_CAT;
            if (ReceiptValidators.AreFieldsSet(getControls(EXAM_CAT)))
                return EXAM_CAT;
            if (ReceiptValidators.AreFieldsSet(getControls(TUITION_CAT)))
                return TUITION_CAT;
            return null;
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
            StudentRequest.AddUrlSegment("id", SELECTED_STUDENT.ToString());
            client.ExecuteAsync<Student>(StudentRequest, StudentResponse =>
            {
                Student Student = StudentResponse.Data;
                var TermRegRequest = new RestRequest("terms/", Method.GET);
                Common.addHeaders(TermRegRequest);
                TermRegRequest.AddParameter("student", Student.Id.ToString());
                client.ExecuteAsync<List<TermRegistration>>(TermRegRequest, TermRegResponse =>
                {
                    try
                    {
                        TermRegistration termReg = TermRegResponse.Data[0];

                        var StudentTuitionBFRequest = new RestRequest("student_bf/{student_id}/", Method.GET);
                        StudentTuitionBFRequest.AddUrlSegment("student_id", Student.Id.ToString());
                        Common.addHeaders(StudentTuitionBFRequest);
                        client.ExecuteAsync<BalanceFowarded>(StudentTuitionBFRequest, StudentTuitionBFResponse =>
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                     new ThreadStart(delegate
                                     {
                                         try
                                         {
                                             if (ReceiptValidators.ValidateFields(NumberControls, TextControls, ReceiptDate.SelectedDate.Value.ToString("yyy-MM-dd"), CurrentSession)
                                             && ReceiptValidators.AreExamsValid(ReceiptUACE.Text, ReceiptUCE.Text, ReceiptMock.Text, Student, StudentTuitionBFResponse.Data, termReg)
                                             && ReceiptValidators.DoReceiptValuesBalance(NumberControls, ReceiptAmount.Text))
                                                 {
                                                     SetFormValues();
                                                     LAST_CATEGORY = LastRecordForPosting();

                                                     Receipt receipt = new Receipt();
                                                     receipt.Receipt_number = ReceiptNo.Text;
                                                     receipt.Amount_paid = RECEIPT_AMOUNT;
                                                     receipt.Receipt_date = ReceiptDate.SelectedDate.Value.ToString("yyy-MM-dd");
                                                     receipt.Session = CurrentSession.Id;
                                                     receipt.Student = SELECTED_STUDENT;

                                                     var ReceiptRequest = new RestRequest("receipts/", Method.POST);
                                                     Common.addHeaders(ReceiptRequest);
                                                     ReceiptRequest.AddJsonBody(receipt);
                                                     var ReceiptAsyncHandle = client.ExecuteAsync<Receipt>(ReceiptRequest, ReceiptResponse =>
                                                     {
                                                         if (!ReceiptResponse.StatusCode.ToString().Equals("BadRequest"))
                                                         {
                                                             Application.Current.Dispatcher.Invoke(
                                                                            DispatcherPriority.Background,
                                                                            new ThreadStart(
                                                                                delegate
                                                                                {
                                                                                    if (ReceiptValidators.AreFieldsSet(getControls(TUITION_CAT)))
                                                                                    {
                                                                                        TuitionFees tuition = new TuitionFees();
                                                                                        tuition.Tuition = TUITION;
                                                                                        tuition.Admission = ADMISSION;
                                                                                        tuition.Computer = COMPUTER;
                                                                                        tuition.Development_fee = DEVELOPMENT;
                                                                                        tuition.Student = SELECTED_STUDENT;
                                                                                        tuition.Receipt = ReceiptResponse.Data.Id;
                                                                                        tuition.Session = CurrentSession.Id;

                                                                                        var TuitionRequest = new RestRequest("tuition/", Method.POST);
                                                                                        Common.addHeaders(TuitionRequest);
                                                                                        TuitionRequest.AddJsonBody(tuition);
                                                                                        var TuitionAsyncHandle = client.ExecuteAsync<TuitionFees>(TuitionRequest, TuitionResponse =>
                                                                                        {
                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                   DispatcherPriority.Background,
                                                                                                   new ThreadStart(
                                                                                                       delegate
                                                                                                       {
                                                                                                           if (!TuitionResponse.StatusCode.ToString().Equals("BadRequest"))
                                                                                                           {
                                                                                                               if (LAST_CATEGORY.Equals(TUITION_CAT))
                                                                                                                   Close();
                                                                                                           }

                                                                                                       }));
                                                                                        });
                                                                                    }

                                                                                    if (ReceiptValidators.AreFieldsSet(getControls(EXAM_CAT)))
                                                                                    {
                                                                                        Exam exam = new Exam();
                                                                                        exam.Receipt = ReceiptResponse.Data.Id;
                                                                                        exam.Student = SELECTED_STUDENT;
                                                                                        exam.Mock = MOCK;
                                                                                        exam.Uace = UACE;
                                                                                        exam.Uce = UCE;
                                                                                        exam.Session = CurrentSession.Id;

                                                                                        var ExamRequest = new RestRequest("exams/", Method.POST);
                                                                                        Common.addHeaders(ExamRequest);
                                                                                        ExamRequest.AddJsonBody(exam);
                                                                                        var TuitionAsyncHandle = client.ExecuteAsync<Exam>(ExamRequest, ExamResponse =>
                                                                                        {
                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                   DispatcherPriority.Background,
                                                                                                   new ThreadStart(
                                                                                                       delegate
                                                                                                       {
                                                                                                           if (!ExamResponse.StatusCode.ToString().Equals("BadRequest"))
                                                                                                           {
                                                                                                               if (LAST_CATEGORY.Equals(EXAM_CAT))
                                                                                                                   this.Close();
                                                                                                           }

                                                                                                       }));
                                                                                        });

                                                                                    }


                                                                                    if (ReceiptValidators.AreFieldsSet(getControls(TEXT_BOOK_CAT)))
                                                                                    {
                                                                                        TextBooks textbook = new TextBooks();
                                                                                        textbook.Receipt = ReceiptResponse.Data.Id;
                                                                                        textbook.Student = SELECTED_STUDENT;
                                                                                        textbook.Cost = TEXT_BOOK_COST;
                                                                                        textbook.Name = TEXT_BOOK_NAME;
                                                                                        textbook.Role = TEXT_BOOK_ROLE;
                                                                                        textbook.Session = CurrentSession.Id;


                                                                                        var TextBookRequest = new RestRequest("textbooks/", Method.POST);
                                                                                        Common.addHeaders(TextBookRequest);
                                                                                        TextBookRequest.AddJsonBody(textbook);
                                                                                        var TuitionAsyncHandle = client.ExecuteAsync<Exam>(TextBookRequest, TextBookResponse =>
                                                                                        {
                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                   DispatcherPriority.Background,
                                                                                                   new ThreadStart(
                                                                                                       delegate
                                                                                                       {
                                                                                                           if (!TextBookResponse.StatusCode.ToString().Equals("BadRequest"))
                                                                                                           {
                                                                                                               if (LAST_CATEGORY.Equals(TEXT_BOOK_CAT))
                                                                                                                   this.Close();
                                                                                                           }

                                                                                                       }));
                                                                                        });

                                                                                    }

                                                                                    if (ReceiptValidators.AreFieldsSet(getControls(OTHERS_CAT)))
                                                                                    {
                                                                                        Others others = new Others();
                                                                                        others.Receipt = ReceiptResponse.Data.Id;
                                                                                        others.Student = SELECTED_STUDENT;
                                                                                        others.Identity_card = ID;
                                                                                        others.Passport_photos = PASSPORT_PHOTOS;
                                                                                        others.Hair_cutting = HAIR_CUTTING;
                                                                                        others.Library = LIBRARY;
                                                                                        others.Medical = MEDICAL;
                                                                                        others.Scouts = SCOUTS;
                                                                                        others.Session = CurrentSession.Id;

                                                                                        var OthersRequest = new RestRequest("others/", Method.POST);
                                                                                        Common.addHeaders(OthersRequest);
                                                                                        OthersRequest.AddJsonBody(others);
                                                                                        var TuitionAsyncHandle = client.ExecuteAsync<Exam>(OthersRequest, OthersResponse =>
                                                                                        {
                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                   DispatcherPriority.Background,
                                                                                                   new ThreadStart(
                                                                                                       delegate
                                                                                                       {
                                                                                                           if (!OthersResponse.StatusCode.ToString().Equals("BadRequest"))
                                                                                                           {
                                                                                                               if (LAST_CATEGORY.Equals(OTHERS_CAT))
                                                                                                                   this.Close();
                                                                                                           }

                                                                                                       }));
                                                                                        });

                                                                                    }


                                                                                    if (ReceiptValidators.AreFieldsSet(getControls(UNIFORM_CAT)))
                                                                                    {
                                                                                        Uniform uniform = new Uniform();
                                                                                        uniform.Receipt = ReceiptResponse.Data.Id;
                                                                                        uniform.Student = SELECTED_STUDENT;
                                                                                        uniform.Class_wear = CLASS_UNIFORM;
                                                                                        uniform.Casual_wear = CASUAL_UNIFORM;
                                                                                        uniform.Sports_wear = SPORTS_UNIFORM;
                                                                                        uniform.Sweater = SWEATER_UNIFORM;
                                                                                        uniform.Jogging = JOGGING_UNIFORM;
                                                                                        uniform.School_t_shirt = T_SHIRT;
                                                                                        uniform.Badge = BADGE;
                                                                                        uniform.Session = CurrentSession.Id;

                                                                                        var UniformRequest = new RestRequest("uniforms/", Method.POST);
                                                                                        Common.addHeaders(UniformRequest);
                                                                                        UniformRequest.AddJsonBody(uniform);
                                                                                        var TuitionAsyncHandle = client.ExecuteAsync<Uniform>(UniformRequest, UniformResponse =>
                                                                                        {
                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                   DispatcherPriority.Background,
                                                                                                   new ThreadStart(
                                                                                                       delegate
                                                                                                       {
                                                                                                           if (!UniformResponse.StatusCode.ToString().Equals("BadRequest"))
                                                                                                           {
                                                                                                               if (LAST_CATEGORY.Equals(UNIFORM_CAT))
                                                                                                                   this.Close();
                                                                                                           }

                                                                                                       }));
                                                                                        });

                                                                                    }


                                                                                    if (ReceiptValidators.AreFieldsSet(getControls(MISC_CAT)))
                                                                                    {
                                                                                        Misc misc = new Misc();
                                                                                        misc.Receipt = ReceiptResponse.Data.Id;
                                                                                        misc.Student = SELECTED_STUDENT;
                                                                                        misc.Role = MISC_ROLE;
                                                                                        misc.Cost = MISC_COST;
                                                                                        misc.Session = CurrentSession.Id;

                                                                                        var MiscRequest = new RestRequest("misc/", Method.POST);
                                                                                        Common.addHeaders(MiscRequest);
                                                                                        MiscRequest.AddJsonBody(misc);
                                                                                        var TuitionAsyncHandle = client.ExecuteAsync<Misc>(MiscRequest, MiscResponse =>
                                                                                        {
                                                                                            Application.Current.Dispatcher.Invoke(
                                                                                                   DispatcherPriority.Background,
                                                                                                   new ThreadStart(
                                                                                                       delegate
                                                                                                       {
                                                                                                           if (!MiscResponse.StatusCode.ToString().Equals("BadRequest"))
                                                                                                           {
                                                                                                               if (LAST_CATEGORY.Equals(MISC_CAT))
                                                                                                                   this.Close();
                                                                                                           }

                                                                                                       }));
                                                                                        });

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
                                                         "3. Examination and class mismatch.\n4. Receipt date is invalid/out of sync with session.");
                                                 }
                                         }catch(InvalidOperationException o)
                                         {
                                            MessageBox.Show("Receipt date might not be set.\nCheck field anf try again");
                                         }

                                     }));

                        });
                    }catch(ArgumentOutOfRangeException ex)
                    {
                        MessageBox.Show("Failed to retrieve student registration");
                    }
                });

            });

        }
    }
}
