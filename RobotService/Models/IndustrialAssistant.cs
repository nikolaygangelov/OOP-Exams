using System;
using System.Collections.Generic;
using System.Text;

namespace RobotService.Models
{
    public class IndustrialAssistant : Robot
    {
        private const int DefaultBatteryCapacity = 40000;
        private const int DefaultconvertionCapacityIndex = 5000;
        public IndustrialAssistant(string model) :
            base(model, DefaultBatteryCapacity, DefaultconvertionCapacityIndex)
        {
        }
    }
}
