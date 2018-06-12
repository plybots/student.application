using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class TermPayableFees
    {
        private int id;
        private int global;
        private int boarding;
        private int uce;
        private int uace;
        private int mock;
        private int sciences;
        private int arts;
        private int development;
        private int admission;
        private List<int> termClassAdditionalFees;
        private int computerPayableFees;
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

        public int Global
        {
            get
            {
                return global;
            }

            set
            {
                global = value;
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

        public int Sciences
        {
            get
            {
                return sciences;
            }

            set
            {
                sciences = value;
            }
        }

        public int Arts
        {
            get
            {
                return arts;
            }

            set
            {
                arts = value;
            }
        }

       
        public int ComputerPayableFees
        {
            get
            {
                return computerPayableFees;
            }

            set
            {
                computerPayableFees = value;
            }
        }

        public int Development
        {
            get
            {
                return development;
            }

            set
            {
                development = value;
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

        public int Boarding
        {
            get
            {
                return boarding;
            }

            set
            {
                boarding = value;
            }
        }

        public List<int> TermClassAdditionalFees
        {
            get
            {
                return termClassAdditionalFees;
            }

            set
            {
                termClassAdditionalFees = value;
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
