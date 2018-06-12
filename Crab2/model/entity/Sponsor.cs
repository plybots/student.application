using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRABSTUDENT.model.entity
{
    class Sponsor
    {
        private int id;
        private string name;
        private int discount;

        private int number_of_students;

        private int tuition;
        private int boarding;
        private int admission;
        private int computer;
        private int development_fee;
        private int class_wear;
        private int sports_wear;
        private int casual_wear;
        private int sweater;
        private int jogging;
        private int school_t_shirt;
        private int badge;
        private int identity_card;
        private int passport_photo;
        private int hair_cutting;
        private int library;
        private int medical;
        private int scouts;
        private int uace;
        private int uce;
        private int mock;

        private int additionalClass1Fees;
        private int additionalClass2Fees;
        private int additionalClass3Fees;
        private int additionalClass4Fees;
        private int additionalClass5Fees;
        private int additionalClass6Fees;

        private int bAdditionalClass1Fees;
        private int bAdditionalClass2Fees;
        private int bAdditionalClass3Fees;
        private int bAdditionalClass4Fees;
        private int bAdditionalClass5Fees;
        private int bAdditionalClass6Fees;

        private int sciences;
        private int arts;

        private int uniforms;
        private int others;

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


        public int Discount
        {
            get
            {
                return discount;
            }

            set
            {
                discount = value;
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

        public int Class_wear
        {
            get
            {
                return class_wear;
            }

            set
            {
                class_wear = value;
            }
        }

        public int Sports_wear
        {
            get
            {
                return sports_wear;
            }

            set
            {
                sports_wear = value;
            }
        }

        public int Casual_wear
        {
            get
            {
                return casual_wear;
            }

            set
            {
                casual_wear = value;
            }
        }

        public int Sweater
        {
            get
            {
                return sweater;
            }

            set
            {
                sweater = value;
            }
        }

        public int Jogging
        {
            get
            {
                return jogging;
            }

            set
            {
                jogging = value;
            }
        }

        public int School_t_shirt
        {
            get
            {
                return school_t_shirt;
            }

            set
            {
                school_t_shirt = value;
            }
        }

        public int Badge
        {
            get
            {
                return badge;
            }

            set
            {
                badge = value;
            }
        }

        public int Identity_card
        {
            get
            {
                return identity_card;
            }

            set
            {
                identity_card = value;
            }
        }

        public int Passport_photo
        {
            get
            {
                return passport_photo;
            }

            set
            {
                passport_photo = value;
            }
        }

        public int Hair_cutting
        {
            get
            {
                return hair_cutting;
            }

            set
            {
                hair_cutting = value;
            }
        }

        public int Library
        {
            get
            {
                return library;
            }

            set
            {
                library = value;
            }
        }

        public int Medical
        {
            get
            {
                return medical;
            }

            set
            {
                medical = value;
            }
        }

        public int Scouts
        {
            get
            {
                return scouts;
            }

            set
            {
                scouts = value;
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

        public int Number_of_students
        {
            get
            {
                return number_of_students;
            }

            set
            {
                number_of_students = value;
            }
        }

        public int Uniforms
        {
            get
            {
                return uniforms;
            }

            set
            {
                uniforms = Class_wear+sports_wear+Casual_wear+Sweater+Jogging+School_t_shirt+Badge;
            }
        }

        public int Others
        {
            get
            {
                return others;
            }

            set
            {
                others = identity_card+Passport_photo+Hair_cutting+Library+Medical+Scouts;
            }
        }

        public int AdditionalClass1Fees
        {
            get
            {
                return additionalClass1Fees;
            }

            set
            {
                additionalClass1Fees = value;
            }
        }

        public int AdditionalClass2Fees
        {
            get
            {
                return additionalClass2Fees;
            }

            set
            {
                additionalClass2Fees = value;
            }
        }

        public int AdditionalClass3Fees
        {
            get
            {
                return additionalClass3Fees;
            }

            set
            {
                additionalClass3Fees = value;
            }
        }

        public int AdditionalClass4Fees
        {
            get
            {
                return additionalClass4Fees;
            }

            set
            {
                additionalClass4Fees = value;
            }
        }

        public int AdditionalClass5Fees
        {
            get
            {
                return additionalClass5Fees;
            }

            set
            {
                additionalClass5Fees = value;
            }
        }

        public int AdditionalClass6Fees
        {
            get
            {
                return additionalClass6Fees;
            }

            set
            {
                additionalClass6Fees = value;
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

        public int BAdditionalClass1Fees
        {
            get
            {
                return bAdditionalClass1Fees;
            }

            set
            {
                bAdditionalClass1Fees = value;
            }
        }

        public int BAdditionalClass2Fees
        {
            get
            {
                return bAdditionalClass2Fees;
            }

            set
            {
                bAdditionalClass2Fees = value;
            }
        }

        public int BAdditionalClass3Fees
        {
            get
            {
                return bAdditionalClass3Fees;
            }

            set
            {
                bAdditionalClass3Fees = value;
            }
        }

        public int BAdditionalClass4Fees
        {
            get
            {
                return bAdditionalClass4Fees;
            }

            set
            {
                bAdditionalClass4Fees = value;
            }
        }

        public int BAdditionalClass5Fees
        {
            get
            {
                return bAdditionalClass5Fees;
            }

            set
            {
                bAdditionalClass5Fees = value;
            }
        }

        public int BAdditionalClass6Fees
        {
            get
            {
                return bAdditionalClass6Fees;
            }

            set
            {
                bAdditionalClass6Fees = value;
            }
        }
    }
}
