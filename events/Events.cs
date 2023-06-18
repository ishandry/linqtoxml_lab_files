using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace pnkfirst
{
    internal class Events
    {
        static void Main(string[] args)
        {
            var agencies = XDocument.Load("agencies.xml");
            var event_records = XDocument.Load("events_records.xml");
            var events = XDocument.Load("events.xml");
            var categories = XDocument.Load("categories.xml");

            TimeOnly to = TimeOnly.Parse("17:00");
            DateTime dt = DateTime.Parse("01/14/2023 01:10");

            DateTime setted = dt.AddHours(to.Hour);
            Console.WriteLine(DateTime.Parse($"{DateOnly.FromDateTime(dt).ToString()} {to.ToString()}"));

            string localChar = "";

            do
            {
                Console.WriteLine("localization for subtask C: (input 'h' (home) or 'a' (abroad))");
                localChar = Console.ReadLine();
            } while (localChar != "a" & localChar != "h");

            string local = localChar == "a" ? "abroad" : "home";


            XDocument partA = new XDocument(
                new XElement("partA",
                    from rec in event_records.Descendants("event_record")
                    join ev in events.Descendants("event")
                        on rec.Element("event_id").Value equals ev.Element("event_id").Value
                    join ag in agencies.Descendants("agency")
                        on rec.Element("agency_id").Value equals ag.Element("agency_id").Value
                    orderby DateTime.Parse(ev.Element("date").Value) descending
                    select new XElement("event",
                    new XElement("event_title", rec.Element("event_title").Value),
                    new XElement("date", ev.Element("date").Value),
                    new XElement("agency_name", ag.Element("agency_name").Value)
                    )
                ));

            partA.Save("partA.xml");

            XDocument partB = new XDocument(
            new XElement("partB",
                from rec in event_records.Descendants("event_record")
                join ev in events.Descendants("event")
                    on rec.Element("event_id").Value equals ev.Element("event_id").Value
                join ag in agencies.Descendants("agency")
                    on rec.Element("agency_id").Value equals ag.Element("agency_id").Value
                join ctg in categories.Descendants("category")
                    on rec.Element("category_id").Value equals ctg.Element("category_id").Value
                group new
                {
                    eventTitle = rec.Element("event_title").Value,
                    eventDate = ev.Element("date").Value,
                    eventAgency = ag.Element("agency_name").Value,
                    eventLocal = ctg.Element("localization").Value
                }
                by ev.Element("date").Value into dayGroup
                orderby DateTime.Parse(dayGroup.Key) descending
                select new XElement("day_events",
                    new XAttribute("date", dayGroup.Key),
                    from evnt in dayGroup

                    group new
                    {
                        eventTitle = evnt.eventTitle,
                        eventDate = evnt.eventDate,
                        eventAgency = evnt.eventAgency,
                        eventLocal = evnt.eventLocal
                    }
                    by evnt.eventLocal
                into localizationGroup
                    select new XElement("by_location",
                    new XAttribute("local", localizationGroup.Key),
                    from e in localizationGroup
                    select
                    new XElement("event",
                    new XElement("event_title", e.eventTitle),
                    new XElement("agency_name", e.eventAgency)
                    )
                    )
                )
            ));

            partB.Save("partB.xml");


            XDocument partC = new XDocument(
            new XElement("partC",
                from rec in event_records.Descendants("event_record")
                join ev in events.Descendants("event")
                    on rec.Element("event_id").Value equals ev.Element("event_id").Value
                join ag in agencies.Descendants("agency")
                    on rec.Element("agency_id").Value equals ag.Element("agency_id").Value
                join ctg in categories.Descendants("category")
                    on rec.Element("category_id").Value equals ctg.Element("category_id").Value
                group new
                {
                    eventTitle = rec.Element("event_title").Value,
                    eventDate = ev.Element("date").Value,
                    eventAgency = ag.Element("agency_name").Value,
                    eventLocal = ctg.Element("localization").Value
                }
                by ev.Element("date").Value into dayGroup
                orderby DateTime.Parse(dayGroup.Key) descending
                select new XElement("day_events",
                    new XAttribute("date", dayGroup.Key),
                    new XAttribute("local", local),
                    from e in dayGroup
                    where e.eventLocal == local
                    select
                    new XElement("event",
                    new XElement("event_title", e.eventTitle),
                    new XElement("agency_name", e.eventAgency)
                    )
                )
            ));

            partC.Save("partC.xml");

            XDocument partD = new XDocument(
            new XElement("partD",
                from rec in event_records.Descendants("event_record")
                join ev in events.Descendants("event")
                    on rec.Element("event_id").Value equals ev.Element("event_id").Value
                join ag in agencies.Descendants("agency")
                    on rec.Element("agency_id").Value equals ag.Element("agency_id").Value
                join ctg in categories.Descendants("category")
                    on rec.Element("category_id").Value equals ctg.Element("category_id").Value
                group new
                {
                    eventTitle = rec.Element("event_title").Value,
                    eventDate = ev.Element("date").Value,
                    eventAgency = ag.Element("agency_name").Value
                }
                by ev.Element("date").Value into dayGroup
                orderby DateTime.Parse(dayGroup.Key) descending
                select new XElement("day_top_agencies",
                    new XAttribute("date", dayGroup.Key),
                    from e in dayGroup


                    where dayGroup.Count(x => x.eventAgency == e.eventAgency) ==
                     (dayGroup.GroupBy(x => x.eventAgency).Select(y => y.ToArray())).Max(x => x.Length)

                    group e by e.eventAgency into agGroup
                    orderby agGroup.Key ascending
                    select
                    new XElement("top_agency",
                    new XAttribute("name", agGroup.Key)
                    //new XAttribute("name", e.eventAgency)
                    )
                )
            ));

            partD.Save("partD.xml");



        }
    }
}
