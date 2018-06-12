using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class BalanceFowarded
    {
        private int id;
        private int student;
        private int tuition;
        private int admission;
        private int computer;
        private int development_fee;
        private int uce;
        private int uace;
        private int mock;
        private int session;
        private bool manual;

        

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

        public int Uce
        {
            get
            {
                return uce;
            }

            set
            {
                uce = value;
            }
        }

        public int Uace
        {
            get
            {
                return uace;
            }

            set
            {
                uace = value;
            }
        }

        public int Mock
        {
            get
            {
                return mock;
            }

            set
            {
                mock = value;
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

        public bool Manual
        {
            get
            {
                return manual;
            }

            set
            {
                manual = value;
            }
        }
    }
}
