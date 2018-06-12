using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class Receipt
    {
        private int id;
        private int amount_paid;
        private string receipt_date;
        private string receipt_number;
        private int student;
        private int payment;
        private int sponsor;
        private int session;
        private string date_created;
        private string date_modified;

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

        public int Amount_paid
        {
            get
            {
                return amount_paid;
            }

            set
            {
                amount_paid = value;
            }
        }

        public string Receipt_date
        {
            get
            {
                return receipt_date;
            }

            set
            {
                receipt_date = value;
            }
        }

        public string Receipt_number
        {
            get
            {
                return receipt_number;
            }

            set
            {
                receipt_number = value;
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

        public string Date_modified
        {
            get
            {
                return date_modified;
            }

            set
            {
                date_modified = value;
            }
        }

        public int Payment
        {
            get
            {
                return payment;
            }

            set
            {
                payment = value;
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
    }
}
