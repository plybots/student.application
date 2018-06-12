using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class Student
    {
      

        private int student_no;
        private string student_name;
        private int student_class;
        private string subjects;
        private int sponsor;
        private bool computer;
        private string offering;
        private string date_created;
        private string last_modified;
        private int id;
        private string sponsorName;
        private int fees_offer;
        private int count;

        

        public int Student_no
        {
            get
            {
                return student_no;
            }

            set
            {
                student_no = value;
            }
        }

        public string Student_name
        {
            get
            {
                return student_name;
            }

            set
            {
                student_name = value;
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

        public string Subjects
        {
            get
            {
                return subjects;
            }

            set
            {
                subjects = value;
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

        public string Date_created
        {
            get
            {
                return date_created;
            }

            set
            {
                date_created = value;
            }
        }

        public string Last_modified
        {
            get
            {
                return last_modified;
            }

            set
            {
                last_modified = value;
            }
        }

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

        public string SponsorName
        {
            get
            {
                return sponsorName;
            }

            set
            {
                sponsorName = value;
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

        public bool Computer
        {
            get
            {
                return computer;
            }

            set
            {
                computer = value;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
            }
        }
    }
}
