using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class Session
    {
        private int id;
        private string start;
        private string end;
        private int term;
        private int year;
        private bool is_active;

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

        public string Start
        {
            get
            {
                return start;
            }

            set
            {
                start = value;
            }
        }

        public string End
        {
            get
            {
                return end;
            }

            set
            {
                end = value;
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

        public bool Is_active
        {
            get
            {
                return is_active;
            }

            set
            {
                is_active = value;
            }
        }
    }
}
