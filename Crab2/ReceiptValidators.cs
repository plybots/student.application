using CRABSTUDENT.model.entity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CRABSTUDENT
{
    class ReceiptValidators
    {

        public static bool ValidateFields(
            List<string> NumberControls,
            List<string> TextControls,
            string date,
            Session session)
        {
            foreach (string control in NumberControls)
            {
                if (!string.IsNullOrWhiteSpace(control) && Convert.ToInt32(control) >= 0)
                {
                    try
                    {
                        int x = Convert.ToInt32(control);
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (FormatException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                    
            }

            foreach (string control in TextControls)
            {
                if (string.IsNullOrWhiteSpace(control))
                {
                    return false;
                }
                    
            }

            if (!string.IsNullOrWhiteSpace(date) &&
                Convert.ToDateTime(date) <= Convert.ToDateTime(session.End) &&
                    Convert.ToDateTime(date) >= Convert.ToDateTime(session.Start))
            {
                return true;
            }

            return false;
        }



        public static bool AreExamsValid(string UACE, string UCE, string Mock, Student Student, BalanceFowarded bf, TermRegistration reg)
        {
            if (bf.Uace > 0 || bf.Uce > 0 || bf.Mock > 0)
            {
                if (bf.Uace <= 0 && Convert.ToInt32(UACE) > 0)
                    return false;
                if (bf.Uce <= 0 && Convert.ToInt32(UCE) > 0)
                    return false;
                if (bf.Mock <= 0 && Convert.ToInt32(Mock) > 0)
                    return false;
                return true;
            }
            else
            {
                if (reg.Student_class == 4 || reg.Student_class == 6)
                {
                    if (Convert.ToInt32(UACE) == 0 && Convert.ToInt32(UCE) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        if (reg.Student_class == 4)
                        {
                            if (Convert.ToInt32(UACE) == 0 && Convert.ToInt32(UCE) > 0)
                                return true;
                            else return false;
                        }

                        if (reg.Student_class == 6)
                        {
                            if (Convert.ToInt32(UACE) > 0 && Convert.ToInt32(UCE) == 0)
                                return true;
                            else return false;
                        }
                    }
                }
                else
                {
                    if (Convert.ToInt32(UACE) == 0 &&
                        Convert.ToInt32(UCE) == 0 &&
                        Convert.ToInt32(Mock) == 0)
                        return true;
                    else return false;
                }
            }
            

            return false;
        }

        public static bool AreFieldsSet(List<string> controls)
        {
            foreach (string control in controls)
            {
                if (Convert.ToInt32(control) > 0)
                    return true;
            }
            return false;
        }

        public static bool DoReceiptValuesBalance(List<string> controls, string ReceiptAmount)
        {
            int breakdown_total = 0;
            int receipt_total = Convert.ToInt32(ReceiptAmount);

            foreach (string control in controls)
            {
                breakdown_total += Convert.ToInt32(control);
            }

            if (receipt_total == (breakdown_total - receipt_total))
            {

                return true;
            }
            return false;
        }
    }
}
