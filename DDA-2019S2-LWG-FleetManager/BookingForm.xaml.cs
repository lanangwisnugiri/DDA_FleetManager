﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DDA_2019S2_LWG_FleetManager
{
    /// <summary>
    /// Interaction logic for BookingForm.xaml
    /// </summary>
    public partial class BookingForm : Window
    {
        private int newId;
        private int vehicleID;
        public CarList carlist;
        /// <summary>
        /// Interaction logic for LocationEdit.xaml
        /// </summary>
        private string server;
        private string database;
        private string db_user;
        private string password;
        private int port;
        /// <summary>
        /// DSN String (Data Source Name)
        /// </summary>
        private string dsnString;

        /// <summary>
        /// Property: Database connection
        /// </summary>
        private MySqlConnection connection;

        public BookingForm()
        {
            InitializeComponent();
            InitializeDb();
            HideErrors();
            OpenConnection();
            CloseConnection();
        }
        /// <summary>
        /// this is a constructor to initialize db connection
        /// </summary>
        private void InitializeDb()
        {
            server = "localhost";
            database = "nmt_fleet_manager";
            db_user = "nmt_fleet_manager";
            password = "Password1";
            port = 3306;

            dsnString = "SERVER=" + server + ";"
            + "PORT=" + port + ";"
            + "DATABASE=" + database + ";"
            + "UID=" + db_user + ";"
            + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(dsnString);
        }
        /// <summary>
        /// this is a constructor to set item inside odometer textbox
        /// , selected vehicle, and combotbox items.
        /// </summary>
        /// <param name="carManufacture"></param>
        /// <param name="carModel"></param>
        /// <param name="vehicleOdometer"></param>
        public BookingForm(string carManufacture, string carModel, int vehicleOdometer)
        {
            InitializeComponent();
            InitializeDb();
            // set value on booking start odometer textbox
            BookingStartOdometerTextBox.Text = vehicleOdometer.ToString();
            // set text on selected vehicle textbox
            SelectedVehicleTextBox.Text = carManufacture + " " + carModel.ToString();
            // items for comboBox
            ComboBoxRentalType.Items.Add(BookingType.Day);
            ComboBoxRentalType.Items.Add(BookingType.Km);
        }

        public BookingForm(int vehicleId, string carManufacture, string carModel, int vehicleOdometer, int newId) : this(carManufacture, carModel, vehicleOdometer)
        {
            InitializeComponent();
            InitializeDb();
            this.newId = newId;
            this.vehicleID = vehicleId;
        }

        /// <summary>
        /// Hide error messages
        /// </summary>
        private void HideErrors()
        {
            LabelRentDateError.Visibility = Visibility.Hidden;
            LabelEndDateError.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Open connection to the MySQL Database
        /// </summary>
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact Administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Close the MySQL Database connection
        /// </summary>
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// this is a click event for cancelBooking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelBookButton_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a click event for saveBooking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBookingNow_Clicked(object sender, RoutedEventArgs e)
        {
            HideErrors();
            int vehicleId = vehicleID;
            string customerName = CustomerNameTextBox.Text;
            string selectedVehicle = SelectedVehicleTextBox.Text;
            string bookingType = ComboBoxRentalType.Text;
            string startOdometer = BookingStartOdometerTextBox.Text;
            string startBookingDate = BookingStartDatePicker.Text;
            string endBookingDate = BookingEndDatePicker.Text;
            bool validStartDate = ValidateStartDate(startBookingDate);
            bool validEndDate = ValidateEndDate(endBookingDate);

            if (!validStartDate)
            {
                // TODO: Display error to the user
                LabelRentDateError.Content = "cannot be null";
                LabelRentDateError.Visibility = Visibility.Visible;
            }
            else if (!validEndDate)
            {
                // TODO: Display error to the user
                LabelEndDateError.Content = "cannot be null";
                LabelEndDateError.Visibility = Visibility.Visible;
            }
            if (validStartDate && validEndDate)
            {
                AddBookingToDB(vehicleId, customerName, selectedVehicle, bookingType,
                    startOdometer, startBookingDate, endBookingDate);
                this.DialogResult = true;
                this.Close();
            }
        }
        /// <summary>
        /// this is a bool to check if its null or not
        /// </summary>
        /// <param name="regisId"></param>
        /// <returns></returns>
        private bool ValidateStartDate(string startBookingDate)
        {
            if (startBookingDate == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// this is a bool to check if its null or not
        /// </summary>
        /// <param name="carManufacture"></param>
        /// <returns></returns>
        private bool ValidateEndDate(string endBookingDate)
        {
            if (endBookingDate == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// this is a method to addvehicle to database
        /// </summary>
        /// <param name="regisId"></param>
        /// <param name="carManufacture"></param>
        /// <param name="carModel"></param>
        /// <param name="carYear"></param>
        /// <param name="fuelCapacity"></param>
        /// <param name="carOdometer"></param>
        /// <returns></returns>
        private bool AddBookingToDB(int vehicleId, string customerName, string selectedVehicle, string bookingType,
            string startOdometer, string startBookingDate, string endBookingDate)
        {
            string addBookingSQL = "INSERT INTO `nmt_fleet_manager`.`bookings`(vehicle_id, customer_name, selected_vehicle, booking_type, start_odometer, start_at, end_at) "
                + " VALUES ( '" + vehicleId + "', '" + customerName + "', '" + selectedVehicle + "', '" + bookingType + "', '" + startOdometer + "', STR_TO_DATE( '" + startBookingDate + "', '%d/%m/%Y %H:%i:%s'), STR_TO_DATE( '" + endBookingDate + "', '%d/%m/%Y %H:%i:%s'))";

            using (MySqlCommand cmdSel = new MySqlCommand(addBookingSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows == 0;
            }
        }
    }
}
