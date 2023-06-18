using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace pnkfirst
{
    internal class Med
    {
        static void Main(string[] args)
        {
            var records = XDocument.Load("records.xml");
            var patients = XDocument.Load("patients.xml");
            var services = XDocument.Load("services.xml");


            XDocument partA = new XDocument(
                new XElement("partA",
                    from record in records.Descendants("record")
                    join patient in patients.Descendants("patient")
                        on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                    join service in services.Descendants("service")
                        on record.Element("service_id").Value equals service.Element("service_id").Value

                    group new
                    {
                        date = record.Element("date").Value,
                        surname = patient.Element("patient_surname").Value,
                        price = service.Element("service_price").Value,
                        service = service.Element("service_name").Value
                    }
                    by record.Element("date").Value into dayGroup
                    select new XElement("day_patients",
                    new XAttribute("date", dayGroup.Key),
                    from r in dayGroup
                    group r by r.surname into patGroup
                    orderby patGroup.Key ascending
                    select new XElement("services",
                    new XAttribute("patient", patGroup.Key),
                    from pg in patGroup
                    orderby Int32.Parse(pg.price) descending
                    select new XElement("service",
                    new XElement("service_name", pg.service),
                    new XElement("price", pg.price)
                    )
                    )
                    )

                ));

            partA.Save("partA.xml");

            XDocument partB = new XDocument(
            new XElement("partB",
                from record in records.Descendants("record")
                join service in services.Descendants("service")
                    on record.Element("service_id").Value equals service.Element("service_id").Value

                group new
                {
                    price = service.Element("service_price").Value,
                    service = service.Element("service_name").Value
                }
                by service.Element("service_name").Value into serviceGroup
                let total = serviceGroup.Sum(x => Int32.Parse(x.price))
                orderby total ascending
                select new XElement("service",
                new XAttribute("name", serviceGroup.Key),
                new XElement("count", serviceGroup.Count()),
                new XElement("total_price", total)
                )

            ));

            partB.Save("partB.xml");

            XDocument partC = new XDocument(
            new XElement("partC",
                new XElement("top_services_by_age",
                new XAttribute("age", "under 18 and 18"),

                from record in records.Descendants("record")
                join patient in patients.Descendants("patient")
                    on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                join service in services.Descendants("service")
                    on record.Element("service_id").Value equals service.Element("service_id").Value

                where DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(19)
                > DateTime.Now

                group new
                {
                    service = service.Element("service_name").Value
                }
                by new
                {
                    service = service.Element("service_name").Value
                }
                into serviceGroup
                where serviceGroup.Count() == ((
                                from record in records.Descendants("record")
                                join patient in patients.Descendants("patient")
                                    on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                                join service in services.Descendants("service")
                                    on record.Element("service_id").Value equals service.Element("service_id").Value

                                where DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(19)
                                > DateTime.Now
                                select new {serv = service.Element("service_name").Value}
                ).GroupBy(x => x.serv).Select(y => y.ToArray())).Max(arr => arr.Length)
                select new XElement("service",
                new XAttribute("name", serviceGroup.Key.service),
                new XAttribute("count", serviceGroup.Count())
                )

                ),


                new XElement("top_services_by_age",
                new XAttribute("age", "19 to 60"),

                from record in records.Descendants("record")
                join patient in patients.Descendants("patient")
                    on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                join service in services.Descendants("service")
                    on record.Element("service_id").Value equals service.Element("service_id").Value

                where DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(19)
                <= DateTime.Now
                &
                 DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(61)
                > DateTime.Now

                group new
                {
                    service = service.Element("service_name").Value
                }
                by new
                {
                    service = service.Element("service_name").Value
                }
                into serviceGroup
                where serviceGroup.Count() == ((
                                from record in records.Descendants("record")
                                join patient in patients.Descendants("patient")
                                    on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                                join service in services.Descendants("service")
                                    on record.Element("service_id").Value equals service.Element("service_id").Value

                                where DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(19)
                                <= DateTime.Now
                                &
                                 DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(61)
                                > DateTime.Now
                                select new { serv = service.Element("service_name").Value }
                ).GroupBy(x => x.serv).Select(y => y.ToArray())).Max(arr => arr.Length)
                select new XElement("service",
                new XAttribute("name", serviceGroup.Key.service),
                new XAttribute("count", serviceGroup.Count())
                )

                ),



                new XElement("top_services_by_age",
                new XAttribute("age", "61 plus"),

                from record in records.Descendants("record")
                join patient in patients.Descendants("patient")
                    on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                join service in services.Descendants("service")
                    on record.Element("service_id").Value equals service.Element("service_id").Value

                where
                 DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(61)
                <= DateTime.Now

                group new
                {
                    service = service.Element("service_name").Value
                }
                by new
                {
                    service = service.Element("service_name").Value
                }
                into serviceGroup
                where serviceGroup.Count() == ((
                                from record in records.Descendants("record")
                                join patient in patients.Descendants("patient")
                                    on record.Element("patient_id").Value equals patient.Element("patient_id").Value
                                join service in services.Descendants("service")
                                    on record.Element("service_id").Value equals service.Element("service_id").Value

                                where
                                 DateTime.Parse(patient.Element("patient_bdate").Value).AddYears(61)
                                <= DateTime.Now
                                select new { serv = service.Element("service_name").Value }
                ).GroupBy(x => x.serv).Select(y => y.ToArray())).Max(arr => arr.Length)
                select new XElement("service",
                new XAttribute("name", serviceGroup.Key.service),
                new XAttribute("count", serviceGroup.Count())
                )

                )


            ));

            partC.Save("partC.xml");



        }
    }
}


