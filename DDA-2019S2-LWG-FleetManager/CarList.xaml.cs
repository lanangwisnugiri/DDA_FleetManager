﻿using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
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
    /// Interaction logic for CarList.xaml
    /// </summary>
    public partial class CarList : MetroWindow
    {
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
        public Vehicle vehicle;
        public static ObservableCollection<Vehicle> vehicleList { get; private set; }
        public CarList()
        {
            InitializeComponent();
            ScanStatusKeysInBackground();
            FillVehicleTable();
            vehicleList = new ObservableCollection<Vehicle>();
            InitializeDb();
            refreshVehicleList();
        }
        /// <summary>
        /// this is a method to initialize database
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
        /// this is a method to check toggled key on your keyboard
        /// </summary>
        public void CheckKeyStatus()
        {
            var isNumLockToggled = Keyboard.IsKeyToggled(Key.NumLock); // variable for NumLock
            var isScrollLockToggled = Keyboard.IsKeyToggled(Key.Scroll); // variable for ScrollLock
            var isCapsLockToggled = Keyboard.IsKeyToggled(Key.CapsLock); // variable for CapsLock

            if (isNumLockToggled)
            {
                // if the NumLock is toggled on keyboard
                // The text color will change into red
                NumLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                // if you untoggle the NumLock on your keyboard
                // The text color will change into gray
                NumLockStatus.Foreground = Brushes.Gray;
            }

            if (isCapsLockToggled)
            {
                // if the CapsLock is toggled on keyboard
                // The text color will change into red
                CapsLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                // if you untoggle the CapsLock on your keyboard
                // The text color will change into gray
                CapsLockStatus.Foreground = Brushes.Gray;
            }

            if (isScrollLockToggled)
            {
                // if the ScrollLock is toggled on keyboard
                // The text color will change into red
                ScrollLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                // if you untoggle the ScrollLock on your keyboard
                // The text color will change into gray
                ScrollLockStatus.Foreground = Brushes.Gray;
            }
        }
        /// <summary>
        /// to check the toggled key using await
        /// </summary>
        /// <returns></returns>
        private async Task ScanStatusKeysInBackground()
        {
            while (true)
            {
                CheckKeyStatus();
                await Task.Delay(100);
            }
        }
        /// <summary>
        /// this is a click event to add vehicle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddvehicle_Clicked(object sender, RoutedEventArgs e)
        {
            AddVehicle addVehicle = new AddVehicle();
            addVehicle.TextBoxRegisId.Text = "";
            addVehicle.TextBoxManufacture.Text = "";
            addVehicle.TextBoxModel.Text = "";
            addVehicle.TextBoxYear.Text = "";
            addVehicle.TextBoxFuelCapacity.Text = "";
            addVehicle.TextBoxOdometer.Text = "";

            addVehicle.Owner = this;
            addVehicle.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (addVehicle.ShowDialog() == true)
            {
                UpdateStatus(5000, "Vehicle Added");
                FillVehicleTable();
                UpdateStatus(50, "Ready...");
            }
            addVehicle.Close();
        }


        /// <summary>
        /// this is a method to fillVehicleTable
        /// </summary>
        /// <param name="searchTerm"></param>
        private void FillVehicleTable(string searchTerm = "")
        {

            // Data Source String (Data Source Name)
            string dsnString = "server=localhost;" +
                "user=nmt_fleet_manager;" +
                "database=nmt_fleet_manager;" +
                "port=3306;" +
                "password=Password1";
            MessageTextBlock.Background = this.FindResource
                (SystemColors.ControlLightBrushKey) as Brush;
            MessageTextBlock.Foreground = Brushes.Black;

            try
            {
                // Perform database operations

                UpdateStatus(500, "Processing...");

                using (MySqlConnection connection = new MySqlConnection(dsnString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM `nmt_fleet_manager`.`vehicles`";
                    if (!searchTerm.Equals(""))
                    {
                        sql += " WHERE car_manufacture LIKE '%" + searchTerm + "%'";
                    }
                    MessageTextBox.Text = sql;

                    using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                        
                        da.Fill(dt);
                        vehicleList = MapDataTableToObservableCollection(dt);
                        VehicleListView.ItemsSource = vehicleList;
                        //VehicleListView.ItemsSource = dt.DefaultView;
                    }
                    //refreshVehicleList();
                    connection.Close();
                    UpdateStatus(500, "Ready...");
                }

            }
            catch (Exception ex)
            {
                Debug.Write("ERROR: " + ex.ToString() + "\n");
                MessageTextBlock.Text = "Unable to connect Database...";
                MessageTextBlock.Background = Brushes.Red;
                MessageTextBlock.Foreground = Brushes.White;

            }
        }
        /// <summary>
        /// this is a method to filterbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTextBox_Changed(object sender, TextChangedEventArgs e)
        {
            FillVehicleTable(FilterTextBox.Text);
        }
        /// <summary>
        /// this is a method to updateStatus
        /// </summary>
        /// <param name="delayperiod"></param>
        /// <param name="MessageToShow"></param>
        private async void UpdateStatus(int delayperiod = 1500, string
            MessageToShow = "")
        {
            MessageTextBlock.Text = MessageToShow;
            await Task.Delay(delayperiod);
        }
        /// <summary>
        /// mapping data from data table into observablecollection
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public ObservableCollection<Vehicle> MapDataTableToObservableCollection(DataTable dt)
        {
            ObservableCollection<Vehicle> vehicleList = new ObservableCollection<Vehicle>();

            foreach (DataRow row in dt.Rows)
            {
                vehicleList.Add(
                  new Vehicle(int.Parse(row["id"].ToString()))
                  {
                      RegistrationId = row["registration_id"].ToString(),
                      CarManufacture = row["car_manufacture"].ToString(),
                      CarModel = row["car_model"].ToString(),
                      CarYear = int.Parse(row["car_year"].ToString()),
                      VehicleOdometer = int.Parse(row["vehicle_odometer"].ToString()),
                      TankCapacity = double.Parse(row["tank_capacity"].ToString()),
                  }
                );
            }
            return vehicleList;
        }
        /// <summary>
        /// this is a click event to trigger edit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditVehicle_Clicked(object sender, RoutedEventArgs e)
        {
            Button vehicleEditButton = sender as Button;
            Vehicle vehicle = vehicleEditButton.DataContext as Vehicle;
            EditVehicle editVehicleWindow = new EditVehicle(vehicle.Id, vehicle.RegistrationId, vehicle.CarManufacture,
                vehicle.CarModel, vehicle.CarYear, vehicle.TankCapacity, vehicle.VehicleOdometer);
           
            editVehicleWindow.Owner = this;
            editVehicleWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (editVehicleWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Vehicle Updated");
                FillVehicleTable();
                UpdateStatus(50, "Ready...");
            }
            editVehicleWindow.Close();
        }
        /// <summary>
        /// this is a refresh method
        /// </summary>
        public void refreshVehicleList()
        {
            CollectionViewSource.GetDefaultView(VehicleListView.ItemsSource).Refresh();
        }
        /// <summary>
        /// this is a click event for delete vehicle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteVehicle_Clicked(object sender, RoutedEventArgs e)
        {
            Button deleteVehicleButton = (Button)sender;
            Vehicle vehicle = deleteVehicleButton.DataContext as Vehicle;
            vehicleList.Remove(vehicle);
            string deleteVehicleSQL = "DELETE FROM `nmt_fleet_manager`.`vehicles`" +
                " WHERE `id`='" + vehicle.Id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(deleteVehicleSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
            }
        }
        /// <summary>
        /// this is a click event to open bookingList menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bookingListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            BookingList bookingList = new BookingList();
            bookingList.ShowDialog();
        }
        /// <summary>
        /// this is a click event to book vehicle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBookVehicle_Clicked(object sender, RoutedEventArgs e)
        {
            int newId = new int();
            Button bookVehicleButton = (Button)sender;
            Vehicle vehicleItem = bookVehicleButton.DataContext as Vehicle;
            BookingForm bookingFormWindow = new BookingForm(vehicleItem.Id ,vehicleItem.CarManufacture, vehicleItem.CarModel, vehicleItem.VehicleOdometer, newId);

            bookingFormWindow.Owner = this;
            bookingFormWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (bookingFormWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Booking Added");
                FillVehicleTable();
                UpdateStatus(50, "Ready...");
            }
            bookingFormWindow.Close();
        }
        /// <summary>
        /// this is a click event for journeyListMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JourneyListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            JourneyList journeyList = new JourneyList();
            journeyList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for addService button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addServiceButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button serviceVehicleButton = (Button)sender;
            Vehicle vehicleItem = serviceVehicleButton.DataContext as Vehicle;
            AddService addServiceWindow = new AddService(vehicleItem.Id, vehicleItem.CarManufacture, vehicleItem.CarModel, vehicleItem.VehicleOdometer);

            addServiceWindow.Owner = this;
            addServiceWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (addServiceWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Service Added");
                FillVehicleTable();
                UpdateStatus(50, "Ready...");
            }
            addServiceWindow.Close();
        }
        /// <summary>
        /// this is a click event for fuelPurchaseMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fuelPurchasesMenu_Clicked(object sender, RoutedEventArgs e)
        {
            FuelPurchaseList fuelPurchaseList = new FuelPurchaseList();
            fuelPurchaseList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for serviceMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceMenu_Clicked(object sender, RoutedEventArgs e)
        {
            ServiceList serviceList = new ServiceList();
            serviceList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for About Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutMenu_Clicked(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}
