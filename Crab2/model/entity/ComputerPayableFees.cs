using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class ComputerPayableFees
    {
        private int id;
        private int fees;

        private bool isClass1_applicable;
        private bool isClass2_applicable;
        private bool isClass3_applicable;
        private bool isClass4_applicable;
        private bool isClass5_applicable;
        private bool isClass6_applicable;

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

        public int Fees
        {
            get
            {
                return fees;
            }

            set
            {
                fees = value;
            }
        }

        public bool IsClass1_applicable
        {
            get
            {
                return isClass1_applicable;
            }

            set
            {
                isClass1_applicable = value;
            }
        }

        public bool IsClass2_applicable
        {
            get
            {
                return isClass2_applicable;
            }

            set
            {
                isClass2_applicable = value;
            }
        }

        public bool IsClass3_applicable
        {
            get
            {
                return isClass3_applicable;
            }

            set
            {
                isClass3_applicable = value;
            }
        }

        public bool IsClass4_applicable
        {
            get
            {
                return isClass4_applicable;
            }

            set
            {
                isClass4_applicable = value;
            }
        }

        public bool IsClass5_applicable
        {
            get
            {
                return isClass5_applicable;
            }

            set
            {
                isClass5_applicable = value;
            }
        }

        public bool IsClass6_applicable
        {
            get
            {
                return isClass6_applicable;
            }

            set
            {
                isClass6_applicable = value;
            }
        }
    }
}
