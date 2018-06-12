using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CRABSTUDENT
{
    class Common
    {
        public static void createConfigFile()
        {
            string folderName = @"config";
            Directory.CreateDirectory(folderName);
            string server = @"config/server.txt";
            if (!File.Exists(server))
            {
                using (FileStream fs = File.Create(server))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("http://127.0.0.1:8000;http://localhost:8081/phpmyadmin/");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        public static void startMySQLServer()
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = "F:\\Software\\software-development-tools\\server-support\\xampp\\xampp-control.exe";
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself. 
                // Given that is is started without a window so you cannot terminate it 
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static void startWebServer()
        {
            System.Diagnostics.Process.Start("CMD.exe", "/K Python-Portable serve.py");
        }

        public static string getConnectionString()
        {
            /*using (StreamReader sr = File.OpenText(@"config/server.txt"))
            {
                return sr.ReadLine().Split(';')[0].Trim();
            }*/
            return "http://127.0.0.1:8000";

        }

        public static string getDatabaseConnectionString()
        {
            using (StreamReader sr = File.OpenText(@"config/server.txt"))
            {
                return sr.ReadLine().Split(';')[1].Trim();
            }
        }


        public static void addHeaders(RestRequest request)
        {
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", ((User)Registry.getInstance().getReference(typeof(User).FullName)).Token);
        }

        public static string getServerLocation()
        {
            using (StreamReader sr = File.OpenText(@"config/server.txt"))
            {
                return sr.ReadLine().Split(';')[1].Trim();
            }

        }

        public static int generateStudentNumber(int @class, List<Student> students)
        {
            int number = 0;
            string last_section = "";
            int last_number = 1;
            int next_sequence = 1;

            //get students in that class
            

            //check if student list is empty
            if (students.Count > 0)
            {
                int highest = students[0].Id;
                Student latest = students[0];
                foreach(Student student in students)
                {
                    if (student.Id > highest)
                    {
                        highest = student.Id;
                        latest = student;
                    }
                }
                last_number = latest.Student_no;
            }
                

            //get current calendar year
            int year = DateTime.Today.Year;

            //assign next sequnce number
            if (students.Count > 0)
                next_sequence = Convert.ToInt32(last_number.ToString().Substring(5)) + 1;
            else
                next_sequence = last_number;


            //get sequence string
            if (next_sequence.ToString().Length == 1)
                last_section = "00" + next_sequence.ToString();
            if (next_sequence.ToString().Length == 2)
                last_section = "0" + next_sequence.ToString();
            if (next_sequence.ToString().Length == 3)
                last_section = next_sequence.ToString();


            //concatenate parts to form number
            number = Convert.ToInt32(@class.ToString() + year.ToString() + last_section);

            return number;
        }

        public static string GetDate(DatePicker DatePicker)
        {
            try
            {
                try
                {
                    return DatePicker.SelectedDate.Value.ToString("yyy-MM-dd");
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (InvalidOperationException e)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    return null;
                }
                
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (IndexOutOfRangeException e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return null;
            }

        }

        public static bool AreNullOrWhiteSpace(List<TextBox> controls)
        {
            foreach (TextBox control in controls)
            {
                if (string.IsNullOrWhiteSpace(control.Text))
                    return false;
            }

            return true;
        }
    }
}
