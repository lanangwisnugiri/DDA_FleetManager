﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDA_2019S2_LWG_FleetManager
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string RegistrationID { get; set; }
        public string CarManufacture { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public string FuelType { get; set; }
        public double TankCapacity { get; set; }
        public int VehicleOdometer { get; set; }
        public DateTime VehicleModifiedDate { get; set; }

    }
}
