using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class TuitionFees
    {
        private int id;
        private int tuition;
        private int admission;
        private int computer;
        private int development_fee;
        private int student;
        private string date_created;
        private string last_modified;
        private int receipt;
        private int session;

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

        public int Tuition
        {
            get
            {
                return tuition;
            }

            set
            {
                tuition = value;
            }
        }

        public int Admission
        {
            get
            {
                return admission;
            }

            set
            {
                admission = value;
            }
        }

        public int Computer
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

        public int Development_fee
        {
            get
            {
                return development_fee;
            }

            set
            {
                development_fee = value;
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

        public int Receipt
        {
            get
            {
                return receipt;
            }

            set
            {
                receipt = value;
            }
        }

        public int Session
        {
            get
            {
                return session;
            }

            set
            {
                session = value;
            }
        }
    }
}
