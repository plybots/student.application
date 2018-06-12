using CRABSTUDENT.model;
using CRABSTUDENT.model.entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace CRABSTUDENT
{
    class DataExporter
    {
        private List<List<Object>> students = new List<List<object>>();
        private JavaScriptSerializer serializer;
        public DataExporter(List<List<Object>> students)
        {
            this.students = students;
            serializer = new JavaScriptSerializer();
        }

        public void Export2Excel()
        {
            // creating Excel Application 
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();


            // creating new WorkBook within Excel application 
            Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(1);


            // creating new Excelsheet in workbook 
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

            // see the excel sheet behind the program 
            app.Visible = true;

            // get the reference of first sheet. By default its name is Sheet1. 
            // store its reference to worksheet 
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;

            // changing the name of active sheet 
            worksheet.Name = "Exported From Crab";

            int counter = 1;
            foreach(string header in get_headers())
            {
                worksheet.Cells[1, counter] = header;
                counter++;
            }

            
            int row_count = 2;
            int student_count = 0;
            //MessageBox.Show(students.Count.ToString());
            foreach(List<Object> student in students)
            {
                Student st = serializer.Deserialize<Student>(serializer.Serialize(student[0]));

                worksheet.Cells[row_count, 1] = st.Student_no;
                worksheet.Cells[row_count, 2] = st.Student_name;
                worksheet.Cells[row_count, 3] = st.Student_class;
                worksheet.Cells[row_count, 4] = st.Subjects;
                worksheet.Cells[row_count, 5] = st.Sponsor;
                worksheet.Cells[row_count, 6] = st.Fees_offer;
                worksheet.Cells[row_count, 7] = st.Date_created;

                TuitionFees tuition = serializer.Deserialize<TuitionFees>(serializer.Serialize(student[1]));
                worksheet.Cells[row_count, 9] = tuition.Tuition;
                worksheet.Cells[row_count, 10] = tuition.Admission;
                worksheet.Cells[row_count, 11] = tuition.Computer;
                worksheet.Cells[row_count, 12] = tuition.Development_fee;

                Exam exams = serializer.Deserialize<Exam>(serializer.Serialize(student[2]));
                worksheet.Cells[row_count, 14] = exams.Uce;
                worksheet.Cells[row_count, 15] = exams.Uace;
                worksheet.Cells[row_count, 16] = exams.Mock;

                student_count++;
                row_count++;
            }
            

            try
            {
                workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing
                    );
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (COMException ex) { }
#pragma warning restore CS0168 // Variable is declared but never used
        }

        private List<string> get_headers()
        {
            List<string> headers = new List<string>();
            headers.Add("Student No");
            headers.Add("Student Name");
            headers.Add("Class");
            headers.Add("Subjects");
            headers.Add("Sponsor");
            headers.Add("Fees Offer");
            headers.Add("Date Registered");
            headers.Add("");
            headers.Add("Tuition");
            headers.Add("Admission");
            headers.Add("Computer");
            headers.Add("Development");
            headers.Add("");
            headers.Add("UCE");
            headers.Add("UACE");
            headers.Add("MOCK");
            return headers;
        }
    }
}
