using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class TermClassAdditionalFees
    {
        private int id;

        private int class1;
        private int class2;
        private int class3;
        private int class4;
        private int class5;
        private int class6;
        private bool is_Boarding;

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

        public int Class1
        {
            get
            {
                return class1;
            }

            set
            {
                class1 = value;
            }
        }

        public int Class2
        {
            get
            {
                return class2;
            }

            set
            {
                class2 = value;
            }
        }

        public int Class3
        {
            get
            {
                return class3;
            }

            set
            {
                class3 = value;
            }
        }

        public int Class4
        {
            get
            {
                return class4;
            }

            set
            {
                class4 = value;
            }
        }

        public int Class5
        {
            get
            {
                return class5;
            }

            set
            {
                class5 = value;
            }
        }

        public int Class6
        {
            get
            {
                return class6;
            }

            set
            {
                class6 = value;
            }
        }

        public bool Is_Boarding
        {
            get
            {
                return is_Boarding;
            }

            set
            {
                is_Boarding = value;
            }
        }
    }
}
