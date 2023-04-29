using System;
using System.Collections.Generic;
using System.Text;

namespace RobotService.Models
{
    public class DomesticAssistant : Robot
    {
        private const int DefaultBatteryCapacity = 20000;
        private const int DefaultconvertionCapacityIndex = 2000;
        public DomesticAssistant(string model) : 
            base(model, DefaultBatteryCapacity, DefaultconvertionCapacityIndex)
        {
        }
    }
}
