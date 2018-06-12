using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class User
    {
        private string username;
        private string password;
        private bool status;
        private string token;
        private int id;
        private string email;
        private string first_name;
        private string last_name;
        private string sessionJson;
        private Session session;
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public bool Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public string Token
        {
            get
            {
                return token;
            }

            set
            {
                token = value;
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

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string First_name
        {
            get
            {
                return first_name;
            }

            set
            {
                first_name = value;
            }
        }

        public string Last_name
        {
            get
            {
                return last_name;
            }

            set
            {
                last_name = value;
            }
        }

        internal Session Session
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

        public string SessionJson
        {
            get
            {
                return sessionJson;
            }

            set
            {
                sessionJson = value;
            }
        }
    }
}
