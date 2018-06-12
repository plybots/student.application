using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class TermRegistration
    {
        private int id;
        private int term;
        private int year;
        private string offering;
        private int student;
        private int sponsor;
        private int fees_offer;
        private int student_class;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int Term
        {
            get
            {
                return term;
            }

            set
            {
                term = value;
            }
        }

        public int Year
        {
            get
            {
                return year;
            }

            set
            {
                year = value;
            }
        }

        public string Offering
        {
            get
            {
                return offering;
            }

            set
            {
                offering = value;
            }
        }

        public int Student
        {
            get
            {
                return student;
            }

            set
            {
                student = value;
            }
        }

        public int Sponsor
        {
            get
            {
                return sponsor;
            }

            set
            {
                sponsor = value;
            }
        }

        public int Fees_offer
        {
            get
            {
                return fees_offer;
            }

            set
            {
                fees_offer = value;
            }
        }

        public int Student_class
        {
            get
            {
                return student_class;
            }

            set
            {
                student_class = value;
            }
        }
    }
}
