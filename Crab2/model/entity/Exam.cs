using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class Exam
    {
        private int id;
        private int uce;
        private int uace;
        private int mock;
        private int student;
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
