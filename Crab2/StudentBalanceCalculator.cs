using CRABSTUDENT.model.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CRABSTUDENT
{
    class StudentBalanceCalculator
    {

        Student student;
        Sponsor sponsor;
        TermClassAdditionalFees additionalFees;
        TermClassAdditionalFees BAdditionalFees;
        TermPayableFees termFees;
        ComputerPayableFees computerFees;
        TermRegistration termReg;

        public StudentBalanceCalculator
            (Student student,
            Sponsor sponsor,
            TermClassAdditionalFees additionalFees,
            TermClassAdditionalFees BAdditionalFees,
            TermPayableFees termFees,
            ComputerPayableFees computerFees,
            TermRegistration termReg
            )
        {
            this.student = student;
            this.sponsor = sponsor;
            this.termFees = termFees;
            this.additionalFees = additionalFees;
            this.BAdditionalFees = BAdditionalFees;
            this.computerFees = computerFees;
            this.termReg = termReg;
        }


        public int TuitionBalance(int total_tuition_paid)
        {
            int totaltuition = 0;
            totaltuition += termFees.Global;
            if (termReg.Offering.Equals("BOARDING", StringComparison.InvariantCultureIgnoreCase))
            {
                totaltuition += termFees.Boarding;
                total_tuition_paid += sponsor.Boarding;
                switch (student.Student_class)
                {
                    case 1:
                        totaltuition += (BAdditionalFees.Class1 - sponsor.BAdditionalClass1Fees);
                        break;
                    case 2:
                        totaltuition += (BAdditionalFees.Class2 - sponsor.BAdditionalClass2Fees);
                        break;
                    case 3:
                        totaltuition += (BAdditionalFees.Class3 - sponsor.BAdditionalClass3Fees);
                        break;
                    case 4:
                        totaltuition += (BAdditionalFees.Class4 - sponsor.BAdditionalClass4Fees);
                        break;
                    case 5:
                        totaltuition += (BAdditionalFees.Class5 - sponsor.BAdditionalClass5Fees);
                        break;
                    case 6:
                        totaltuition += (BAdditionalFees.Class6 - sponsor.BAdditionalClass6Fees);
                        break;
                }
            }

            switch (student.Student_class)
            {
                case 1:
                    totaltuition += (additionalFees.Class1 - sponsor.AdditionalClass1Fees);
                    break;
                case 2:
                    totaltuition += (additionalFees.Class2 - sponsor.AdditionalClass2Fees);
                    break;
                case 3:
                    totaltuition += (additionalFees.Class3 - sponsor.AdditionalClass3Fees);
                    break;
                case 4:
                    totaltuition += (additionalFees.Class4 - sponsor.AdditionalClass4Fees);
                    break;
                case 5:
                    totaltuition += (additionalFees.Class5 - sponsor.AdditionalClass5Fees);
                    break;
                case 6:
                    totaltuition += (additionalFees.Class6 - sponsor.AdditionalClass6Fees);
                    break;
            }

            if (student.Subjects.Equals("SCIENCES", StringComparison.InvariantCultureIgnoreCase))
            {
                totaltuition += (termFees.Sciences - sponsor.Sciences);

            }
            else if (student.Subjects.Equals("ARTS", StringComparison.InvariantCultureIgnoreCase))
            {
                totaltuition += (termFees.Arts - sponsor.Arts);
            }

            return (total_tuition_paid + termReg.Fees_offer + sponsor.Tuition) - (totaltuition - sponsor.Discount);
        }

        public int DevelopmentBalance(int total_dev_paid)
        {
            return (total_dev_paid+sponsor.Development_fee) - termFees.Development;

        }

        public int AdmissionBalance(int total_admission_paid)
        {
            return (total_admission_paid+sponsor.Admission) - termFees.Admission;
            
        }

        public int ComputerBalance(int total_computer_paid)
        {
            return (total_computer_paid+sponsor.Computer) - computerFees.Fees;

        }

        public int UCEBalance(int total_uce_paid)
        {
            if (termReg.Student_class == 4)
                return (total_uce_paid + sponsor.Uce) - termFees.Uce;
            else return total_uce_paid;
        }

        public int UACEBalance(int total_UACE_paid)
        {
            if (termReg.Student_class == 6)
                return (total_UACE_paid + sponsor.Uace) - termFees.Uace;
            else return total_UACE_paid;
        }

        public int MockBalance(int total_Mock_paid)
        {
            if (termReg.Student_class == 4 || termReg.Student_class == 6)
                return (total_Mock_paid + sponsor.Mock) - termFees.Mock;
            else return total_Mock_paid;
        }
    }
}
