using RobotService.Models.Contracts;
using RobotService.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotService.Repositories
{
    public class RobotRepository : IRepository<IRobot>
    {
        private readonly List<IRobot> models;

        public RobotRepository()
        {
            models = new List<IRobot>();
        }
        public void AddNew(IRobot model)
        {
            models.Add(model);
        }

        public IRobot FindByStandard(int interfaceStandard)
        {
            return models.FirstOrDefault(r => r.InterfaceStandards.Any(s => s == interfaceStandard));
        }

        public IReadOnlyCollection<IRobot> Models() => models.AsReadOnly();

        public bool RemoveByName(string robotModel)
        {
            IRobot robot = models.FirstOrDefault(m => m.Model == robotModel);
            return models.Remove(robot);
        }
    }
}
