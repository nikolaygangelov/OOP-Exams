using RobotService.Core.Contracts;
using RobotService.Models;
using RobotService.Models.Contracts;
using RobotService.Repositories;
using RobotService.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RobotService.Core
{
    public class Controller : IController
    {
        private SupplementRepository supplements;
        private RobotRepository robots;

        public Controller()
        {
            supplements = new SupplementRepository();
            robots = new RobotRepository();
        }
        public string CreateRobot(string model, string typeName)
        {

            IRobot robot;

            switch (typeName)
            {
                case nameof(DomesticAssistant):
                    robot = new DomesticAssistant(model);
                    break;
                case nameof(IndustrialAssistant):
                    robot = new IndustrialAssistant(model);
                    break;
                default:
                    return String.Format(OutputMessages.RobotCannotBeCreated, typeName);
            }

            robots.AddNew(robot);

            return String.Format(OutputMessages.RobotCreatedSuccessfully, typeName, model);

        }

        public string CreateSupplement(string typeName)
        {
            ISupplement supplement;

            switch (typeName)
            {
                case nameof(SpecializedArm):
                    supplement = new SpecializedArm();
                    break;
                case nameof(LaserRadar):
                    supplement = new LaserRadar();
                    break;
                default:
                    return String.Format(OutputMessages.SupplementCannotBeCreated, typeName);
            }

            supplements.AddNew(supplement);
           
            return String.Format(OutputMessages.SupplementCreatedSuccessfully, typeName);
        }

        public string PerformService(string serviceName, int intefaceStandard, int totalPowerNeeded)
        {
            var collection = robots.Models()
                .Where(r => r.InterfaceStandards.Contains(intefaceStandard))
                .OrderByDescending(r => r.BatteryLevel);

            if (collection.Count() == 0)
            {
                return String.Format(OutputMessages.UnableToPerform, intefaceStandard);
            }

            int sum = collection.Sum(r => r.BatteryLevel);
            int powerNeeded = totalPowerNeeded - sum;
            
            int counter = 0;

            if (sum < totalPowerNeeded)
            {
                return String.Format(OutputMessages.MorePowerNeeded, serviceName, powerNeeded);
            }
            else
            {
                //while
                foreach (IRobot robot in collection)
                {
                    if (robot.BatteryLevel >= totalPowerNeeded)
                    {
                        robot.ExecuteService(totalPowerNeeded);
                        counter++;
                        break;
                    }
                    else
                    {
                        
                        totalPowerNeeded -= robot.BatteryLevel;
                        robot.ExecuteService(robot.BatteryLevel);
                        counter++;

                        if (totalPowerNeeded == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return String.Format(OutputMessages.PerformedSuccessfully, serviceName, counter); ;

        }

        public string Report()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IRobot robot in robots.Models().OrderByDescending(r => r.BatteryLevel).ThenBy(r => r.BatteryCapacity))
            {
                sb.AppendLine(robot.ToString());
            }

            return sb.ToString().TrimEnd();
        }

        public string RobotRecovery(string model, int minutes)
        {
            int counter = 0;
            foreach (IRobot robot in robots.Models().Where(r => r.Model == model))
            {
                if (robot.BatteryLevel < 0.5 * robot.BatteryCapacity)
                {
                    robot.Eating(minutes);
                    counter++;
                }
            }

            return String.Format(OutputMessages.RobotsFed, counter);
        }

        public string UpgradeRobot(string model, string supplementTypeName)
        {
            ISupplement supplement = supplements.Models().FirstOrDefault(s => s.GetType().Name == supplementTypeName);
            
            int interfaceValue = supplement.InterfaceStandard;
            var collection = robots.Models()
                .Where(r => !r.InterfaceStandards.Contains(interfaceValue))
                .Where(r => r.Model == model);
                   
            if (collection.Count() == 0)
            {
                return String.Format(OutputMessages.AllModelsUpgraded, model);
            }

            IRobot robot = collection.ToList()[0];
            robot.InstallSupplement(supplement);
            supplements.RemoveByName(supplementTypeName);

            return String.Format(OutputMessages.UpgradeSuccessful, model, supplementTypeName);

        }
    }
}
