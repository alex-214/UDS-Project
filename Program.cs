using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace UDS
{
    class Program
    {
        private const int recordsToCreate = 40000;
        static void Main()
        {
            string connectionString = @"AuthType=OAuth;
               Username=alex@sample.uds.com;
               Password=9Nxc4uPDu;
               Url=https://sample.uds.com;
               AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
               RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";
            CrmServiceClient svc = new CrmServiceClient(connectionString);

            CreateRecords(svc);
        }

        private static void CreateRecords(CrmServiceClient svc)
        {
            for (int i = 1; i <= recordsToCreate; i++)
            {
                Console.WriteLine("{0,2} of {1}", i, recordsToCreate);
                Entity rent = new Entity("uds_rent");
                rent["uds_name"] = "" + DateTime.Now.ToString("dd/MM - HH:mm:ss");
                PickupHandoverService pickupHandover = new PickupHandoverService(svc, rent);
                pickupHandover.PickupDateAssign();
                pickupHandover.HandoverDateAssign();
                pickupHandover.PickupReturnLocationsAssign();

                rent["uds_price"] = (new Random().NextDouble()) * 1000;

                CarService car = new CarService(svc, rent);
                car.CarClassAssign();
                car.CarAssign();

                StatusService rentStatus = new StatusService(svc, rent);
                ReportService reports = new ReportService(svc, rent);
                int t = i % 20;
                if (t <= 14)
                {
                    rentStatus.status = StatusService.Statuses.Returned;
                    reports.CreatePickupReport();
                    reports.CreateReturnReport();
                    if (new Random().NextDouble() <= 0.9998)
                        rent["uds_paid"] = true;
                }

                switch (t)
                {
                    case 15:
                        rentStatus.status = StatusService.Statuses.Created;
                        break;
                    case 16:
                        rentStatus.status = StatusService.Statuses.Confirmed;
                        if ((new Random().NextDouble()) <= 0.9)
                            rent["uds_paid"] = true;
                        break;
                    case 17:
                        rentStatus.status = StatusService.Statuses.Renting;
                        reports.CreatePickupReport();
                        if (new Random().NextDouble() <= 0.999)
                            rent["uds_paid"] = true;
                        break;
                    case 18:
                        rentStatus.status = StatusService.Statuses.Canceled;
                        break;
                    case 19:
                        rentStatus.status = StatusService.Statuses.Canceled;
                        break;
                }
                rentStatus.StatusAssign(rentStatus.status);

                CustomerService customer = new CustomerService(svc, rent);
                customer.CustomerAssign();
                Guid g = svc.Create(rent);
                Console.WriteLine(g);
            }
        }
    }
}
