using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class TextBooks
    {
        /*Id = models.AutoField(primary_key=True)
    Name = models.CharField(max_length=100)
    Role = models.TextField()
    Cost = models.IntegerField()
    Student = models.ForeignKey(Student)
    Receipt = models.ForeignKey(Receipt)*/

        private int id;
        private string name;
        private string role;
        private int cost;
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

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Role
        {
            get
            {
                return role;
            }

            set
            {
                role = value;
            }
        }

        public int Cost
        {
            get
            {
                return cost;
            }

            set
            {
                cost = value;
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
