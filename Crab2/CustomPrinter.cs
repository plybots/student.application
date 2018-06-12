
using CRABSTUDENT.model.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Crab.common
{
    class CustomPrinter
    {
        private Student student;
        private List<TuitionFees> tuitions;
        private Sponsor sponsor;
        private TermRegistration reg;
        private int total_tuition;
        

        public CustomPrinter(
            Student student, 
            List<TuitionFees> tuitions, 
            int total_tuition, 
            Sponsor sponsor,
            TermRegistration reg)
        {
            this.student = student;
            this.tuitions = tuitions;
            this.sponsor = sponsor;
            this.reg = reg;
            this.total_tuition = total_tuition;
        }
        
        public void printStudent()
        {
            PrintDialog printDlg = new PrintDialog();
            FlowDocument doc;
            // Create a FlowDocument dynamically.  
            doc = CreateStudentFlowDocument();
            
            // Create IDocumentPaginatorSource from FlowDocument 

            IDocumentPaginatorSource idpSource = doc;
           
            // Call PrintDocument method to send document to printer 
            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintDocument(idpSource.DocumentPaginator, "__"+student.Student_name.ToLower()+"__tuition_print_out__");
            }

        }

        private FlowDocument CreateStudentFlowDocument()

        {

            // Create a FlowDocument 
            FlowDocument doc = new FlowDocument();
            doc.PageWidth = Double.NaN;
            doc.ColumnWidth = 999999;

            // Create a Section 
            Section sec = new Section();
            sec.FontSize = 12;
            sec.FontFamily = new FontFamily("Global Monospace");

            // Create title Paragraph 
            Paragraph title = new Paragraph();

            // Create header paragraph
            Paragraph header = new Paragraph();

            // create personal info paragraph
            Paragraph personalInfo = new Paragraph();

            // create personal info title paragraph
            Paragraph personalInfotilte = new Paragraph();

            //create payments paragraph
            Paragraph payments = new Paragraph();

            //add text to title paragraph
           
            title.Inlines.Add(new Run(
                "\nST JOHNS SENIOR SECONDARY SCHOOL - NTEBETEBE-BWEYOGERERE"
                ));
            title.Inlines.Add(new Run("\nP.O BOX 26725 KAMPALA - TEL: +256-772-976623/+256-772507500"));
            title.FontFamily = new FontFamily("Global Monospace");
            title.FontSize = 16;
            title.FontWeight = FontWeights.Bold;
            //title.TextAlignment = TextAlignment.Center;

            string fees_offer = student.Fees_offer.ToString();
            personalInfo.Inlines.Add(new Run(student.Student_name + " ["+student.Student_no+"] CLASS: S"+student.Student_class+
                " ["+student.Offering+"]\nSUBJECTS: "+student.Subjects+"\tSPONSOR: "+ student.SponsorName+"\tFEES OFFER: "+ fees_offer));
            Bold date = new Bold();
            date.Inlines.Add(new Run("\n" + DateTime.Today.ToString()));
            personalInfo.Inlines.Add(date);

            //table for receipts
            Table table = new Table();
            table.CellSpacing = 2;
            table.Background = Brushes.White;
            table.Margin = new Thickness(0, 0, 0, 0);
            

            int numberOfColumns = 3;
            for (int x = 0; x < numberOfColumns; x++)
            {
                table.Columns.Add(new TableColumn());
                
                switch (x)
                {
                    case 0:
                        table.Columns[0].Width = new GridLength(200);
                        break;
                    case 1:
                        table.Columns[1].Width = new GridLength(200);
                        break;
                    case 2:
                        table.Columns[2].Width = new GridLength(200);
                        break;
                }
                if (x % 2 == 0)
                    table.Columns[x].Background = Brushes.Beige;
                else
                    table.Columns[x].Background = Brushes.DarkGray;
            }

            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups[0].Rows.Add(new TableRow());
            TableRow currentRow = table.RowGroups[0].Rows[0];
            currentRow.FontFamily = new FontFamily("Global Monospace");
            currentRow.FontSize = 14;
            currentRow.Background = Brushes.LightGray;
            currentRow.Foreground = Brushes.Black;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Term Information"))));
            currentRow.Cells[0].FontWeight = FontWeights.Bold;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Payments"))));
            currentRow.Cells[1].FontWeight = FontWeights.Bold;
            currentRow.Cells[1].Background = Brushes.DarkGray;
            currentRow.Cells[1].Foreground = Brushes.White;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Balance"))));
            currentRow.Cells[2].FontWeight = FontWeights.Bold;

            table.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table.RowGroups[0].Rows[1];
            currentRow.FontFamily = new FontFamily("Global Monospace");
            currentRow.FontSize = 14;
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\nTerm "+reg.Term + ", " + reg.Year))));

            string tuition_st = "\n";
            int paid_tuition = 0;
            int count = 0;
            foreach(TuitionFees fee in tuitions)
            {
                if(count == 2)
                    tuition_st += fee.Tuition + ",\n";
                else
                    tuition_st += fee.Tuition + ", ";

                paid_tuition += fee.Tuition;
                count++;
            }

            // paid_tuition += student.Fees_offer;

            if (student.SponsorName.Equals("PRIVATE", StringComparison.InvariantCultureIgnoreCase))
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(tuition_st))));
            else if (string.IsNullOrWhiteSpace(tuition_st))
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\n[" + student.SponsorName + "]"))));
            else
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(tuition_st))));

            int balance = paid_tuition - total_tuition;

            
            if (balance > 0)
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\n"+balance+" [CR] "))));
            if(balance < 0)
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\n" + (balance*-1) + " [DR] "))));
            if(balance == 0)
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\n" + balance))));

            //signature paragraph
            Paragraph sign = new Paragraph();
            sign.Inlines.Add(new Run("\nStamp & signature:\n\n..................................................................."));
            sign.FontFamily = new FontFamily("Global Monospace");
            sign.FontSize = 12;
            sign.FontWeight = FontWeights.Bold;

            sec.Blocks.Add(title);
            sec.Blocks.Add(personalInfo);

            doc.Blocks.Add(sec);
            doc.Blocks.Add(table);
            doc.Blocks.Add(sign);

            return doc;

        }

     
    }
}
