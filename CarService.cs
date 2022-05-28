using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;


namespace UDS
{
    class CarService
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;
        public CarService(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }
        public void CarClassAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var carClasses = (from _carClasses in context.CreateQuery("uds_carclass")
                                  select _carClasses)
                                   .ToArray();
                Entity rndmCarClassEntity = carClasses.ElementAt(new Random().Next(0, carClasses.Length));
                EntityReference refName = new EntityReference("uds_carclass", rndmCarClassEntity.Id);
                entity["uds_carclass"] = refName;
            }
        }

        public void CarAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var cars = (from _cars in context.CreateQuery("uds_car")
                            where _cars["uds_carclass"].Equals(entity.Attributes["uds_carclass"])
                            select _cars
                           ).ToArray();
                EntityReference carClassRef = new EntityReference("uds_car", cars.ElementAt(new Random().Next(0, cars.Length)).Id);
                entity["uds_car"] = carClassRef;
            }
        }
    }
}
