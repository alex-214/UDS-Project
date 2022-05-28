using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace UDS
{
    class ReportService
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;

        public ReportService(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }

        public void CreatePickupReport()
        {
            Entity report = new Entity("uds_cartransferreport");
            report["uds_name"] = "Pickup_" + DateTime.Now.ToString("dd/MM - HH:mm:ss");

            EntityReference carRef = new EntityReference("uds_car", ((EntityReference)entity["uds_car"]).Id);
            report["uds_car"] = carRef;
            report["uds_type"] = new OptionSetValue(172620000);
            report["uds_date"] = entity["uds_reservedpickup"];
            report["uds_damages"] = false;

            if ((new Random().NextDouble()) < 0.05)
            {
                report["uds_damages"] = true;
                report["uds_damagedescription"] = "Damage";
            }
            Guid pickupReportGuid = svc.Create(report);
            EntityReference rentingRef = new EntityReference("uds_cartransferreport", pickupReportGuid);
            entity["uds_pickupreport"] = rentingRef;
        }

        public void CreateReturnReport()
        {
            Entity report = new Entity("uds_cartransferreport");
            report["uds_name"] = "Return_" + DateTime.Now.ToString("dd/MM-HH:mm:ss");

            EntityReference carRef = new EntityReference("uds_car", ((EntityReference)entity["uds_car"]).Id);
            report["uds_car"] = carRef;
            report["uds_type"] = new OptionSetValue(172620001);
            report["uds_date"] = entity["uds_reservedhandover"];
            report["uds_damages"] = false;
            if ((new Random().NextDouble()) < 0.05)
            {
                report["uds_damages"] = true;
                report["uds_damagedescription"] = "Damage";
            }

            Guid returnReportGuid = svc.Create(report);

            EntityReference returnRef = new EntityReference("uds_cartransferreport", returnReportGuid);
            entity["uds_returnreport"] = returnRef;
        }
    }
}
