
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace pnkfirst
{
    internal class Concerts
    {
        static void Main(string[] args)
        {
            var records = XDocument.Load("records.xml");
            var performers = XDocument.Load("performers.xml");
            var concerts = XDocument.Load("concerts.xml");


            XDocument partA = new XDocument(
                new XElement("partA",
                    from record in records.Descendants("record")
                    join perf in performers.Descendants("performer")
                        on record.Element("performer_id").Value equals perf.Element("performer_id").Value

                    orderby DateTime.Parse(record.Element("date").Value) ascending
                    select new XElement("concert",
                    new XElement("title", record.Element("concert_title").Value),
                    new XElement("date", record.Element("date").Value),
                    new XElement("performer", perf.Element("name").Value))

                ));

            partA.Save("partA.xml");




            XDocument partAMod = new XDocument(
            new XElement("partAMod",
                from record in records.Descendants("record")
                join perf in performers.Descendants("performer")
                    on record.Element("performer_id").Value equals perf.Element("performer_id").Value

                group new
                {
                    title = record.Element("concert_title").Value,
                    performer = perf.Element("name").Value
                }
                by
                new
                {
                    date = record.Element("date").Value
                } into dayGroup
                orderby DateTime.Parse(dayGroup.Key.date) ascending
                select new XElement("day_concerts",
                new XAttribute("date", dayGroup.Key.date),
                from c in dayGroup
                group c by c.performer into perfGroup
                select new XElement("perf_concert",
                new XAttribute("perf", perfGroup.Key),
                from conc in perfGroup
                select new XElement("concert",
                new XAttribute("title", conc.title)
                )
                )
                )
                ));

            partAMod.Save("partAMod.xml");







            XDocument partB = new XDocument(
            new XElement("partB",
                from record in records.Descendants("record")
                join perf in performers.Descendants("performer")
                    on record.Element("performer_id").Value equals perf.Element("performer_id").Value
                join concert in concerts.Descendants("concert")
                    on record.Element("concert_id").Value equals concert.Element("concert_id").Value

                orderby DateTime.Parse(record.Element("date").Value) ascending
                select new XElement("concert",
                new XElement("title", record.Element("concert_title").Value),
                new XElement("date", record.Element("date").Value),
                new XElement("performer", perf.Element("name").Value),
                new XElement("starts", concert.Element("time").Value),
                new XElement("at", concert.Element("place").Value)
                )

            ));

            partB.Save("partB.xml");

            XDocument partC = new XDocument(
            new XElement("concerts",
            new XAttribute("by", "Awesome theature"),
                from record in records.Descendants("record")
                join perf in performers.Descendants("performer")
                    on record.Element("performer_id").Value equals perf.Element("performer_id").Value
                join concert in concerts.Descendants("concert")
                    on record.Element("concert_id").Value equals concert.Element("concert_id").Value

                where perf.Element("name").Value.ToString() == "Awesome theature"
                group record.Element("date").Value
                by record.Element("concert_title").Value
                into titleGroup
                orderby titleGroup.Key ascending
                select new XElement("dates",
                new XAttribute("title", titleGroup.Key),

                from c in titleGroup
                select new XElement("date", c)
                )

            ));

            partC.Save("partC.xml");

            XDocument partD = new XDocument(
            new XElement("cities-top-performers",
                from record in records.Descendants("record")
                join perf in performers.Descendants("performer")
                    on record.Element("performer_id").Value equals perf.Element("performer_id").Value
                join concert in concerts.Descendants("concert")
                    on record.Element("concert_id").Value equals concert.Element("concert_id").Value

                group new 
                {
                    perfName = perf.Element("name").Value
                }
                
                by new 
                {
                    city = concert.Element("place").Value
                }
                into cityGroup
                orderby cityGroup.Key.city ascending
                select new XElement("top",
                new XAttribute("city", (string)cityGroup.Key.city),


                from pf in cityGroup
                where cityGroup.Count(x => x.perfName == pf.perfName) ==
                    (cityGroup.GroupBy(x => x.perfName).Select(y => y.ToArray())).Max(arr => arr.Length)
                group pf by pf.perfName into pfGroup
                select new XElement("performer", pfGroup.Key)
                )

            ));


            partD.Save("partD.xml");



        }
    }
}

