using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CRABSTUDENT
{
    class GUISwitch
    {
        Grid StudentsList;
        Grid RegistrationGrid;
        Grid Payments;
        Grid ProgressDialog;
        Grid PayableFees;
        Grid SponsorsGrid;

        public GUISwitch()
        {

        }

        public GUISwitch(
            Grid StudentsList,
            Grid RegistrationGrid,
            Grid Payments,
            Grid ProgressDialog,
            Grid PayableFees,
            Grid SponsorsGrid
            )
        {
            this.StudentsList = StudentsList;
            this.RegistrationGrid = RegistrationGrid;
            this.Payments = Payments;
            this.ProgressDialog = ProgressDialog;
            this.PayableFees = PayableFees;
            this.SponsorsGrid = SponsorsGrid;
        }

        public void InitializeGUI(Grid MainContent, Border LoginMenu)
        {
            MainContent.Visibility = Visibility.Collapsed;
            LoginMenu.Visibility = Visibility.Visible;
        }

        public void HideAll()
        {
            StudentsList.Visibility = Visibility.Collapsed;
            RegistrationGrid.Visibility = Visibility.Collapsed;
            Payments.Visibility = Visibility.Collapsed;
            ProgressDialog.Visibility = Visibility.Collapsed;
            PayableFees.Visibility = Visibility.Collapsed;
            SponsorsGrid.Visibility = Visibility.Collapsed;
        }

        public void ShowStudentsList()
        {
            HideAll();
            StudentsList.Visibility = Visibility.Visible;
        }

        public void ShowRegistrationGrid()
        {
            HideAll();
            RegistrationGrid.Visibility = Visibility.Visible;
        }
        

        public void ShowPaymentsGrid()
        {
            HideAll();
            Payments.Visibility = Visibility.Visible;
        }

        public void ShowProgressDialog()
        {
            HideAll();
            ProgressDialog.Visibility = Visibility.Visible;
        }

        public void HideProgressDialog(Grid showGrid)
        {
            HideAll();
            showGrid.Visibility = Visibility.Visible;
        }

        public void ShowPayableFees()
        {
            HideAll();
            PayableFees.Visibility = Visibility.Visible;
        }

        public void ShowSponsrsGrid()
        {
            HideAll();
            SponsorsGrid.Visibility = Visibility.Visible;
        }

        

    }
}
