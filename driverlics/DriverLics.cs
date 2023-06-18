using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace pnkfirst
{
    internal class DriverLics
    {
        static void Main(string[] args)
        {
            var theories = XDocument.Load("theors.xml");
            var records = XDocument.Load("records.xml");
            var drives = XDocument.Load("drives.xml");
            var lics = XDocument.Load("licenses.xml");

            DateTime dt = DateTime.Now;


            XDocument partA = new XDocument(
                new XElement("partA",

                from lic in lics.Descendants("lic")
                join record in records.Descendants("record")
                    on lic.Element("lic_id").Value equals record.Element("lic_id").Value

                let recordDate = DateTime.Parse(record.Element("date").Value)
                orderby lic.Element("lastname").Value ascending

                select new XElement("license", 
                new XElement("id", lic.Element("lic_id").Value),
                new XElement("lastname", lic.Element("lastname").Value),
                new XElement("category", lic.Element("category").Value),
                new XElement("from_date", record.Element("date").Value),
                new XElement("to_date", lic.Element("exp_date").Value)
                )
                ));

            partA.Save("partA.xml");

            XDocument partB = new XDocument(
            new XElement("partB",
            from record in records.Descendants("record")
            join lic in lics.Descendants("lic")
                on record.Element("lic_id").Value equals lic.Element("lic_id").Value
            join drive in drives.Descendants("drive")
                on record.Element("drive_id").Value equals drive.Element("drive_id").Value
            join theory in theories.Descendants("theory")
                on record.Element("theory_id").Value equals theory.Element("theory_id").Value

            orderby lic.Element("lastname").Value ascending
            select new XElement("license",
            new XAttribute("id", lic.Element("lic_id").Value),
            new XElement("lastname", lic.Element("lastname").Value),
            new XElement("category", lic.Element("category").Value),
            new XElement("from_date", record.Element("date").Value),
            new XElement("to_date", lic.Element("exp_date").Value),
            new XElement("drive_result", drive.Element("dresult").Value),
            new XElement("theory_result", theory.Element("tresult").Value)
            )
            ));

            partB.Save("partB.xml");

            XDocument partC = new XDocument(
            new XElement("partC",
            from record in records.Descendants("record")
            join lic in lics.Descendants("lic")
                on record.Element("lic_id").Value equals lic.Element("lic_id").Value

            group new
            {
                Surname = lic.Element("lastname").Value,
                fromDate = DateTime.Parse(record.Element("date").Value.ToString()),
                expDate = DateTime.Parse(lic.Element("exp_date").Value.ToString())
            }
            by lic.Element("category").Value into categGroup
            select new XElement("temporary",
            new XAttribute("category", categGroup.Key),
            from l in categGroup

            where l.fromDate.AddYears(2) >= l.expDate 
            select new XElement("surname", l.Surname)
            )
            ));

            partC.Save("partC.xml");


            XDocument partD = new XDocument(
            new XElement("partD",
            from record in records.Descendants("record")
            join lic in lics.Descendants("lic")
                on record.Element("lic_id").Value equals lic.Element("lic_id").Value
            join theory in theories.Descendants("theory")
                on record.Element("theory_id").Value equals theory.Element("theory_id").Value

            group new
            {
                theoryResult = Int32.Parse(theory.Element("tresult").Value),
                question = theory.Element("question").Value,
            }
            by lic.Element("category").Value into categGroup
            select new XElement("hardest_questions", new XAttribute("category", categGroup.Key),
            from l in categGroup

            where l.theoryResult == (from record in records.Descendants("record")
                                     join lic in lics.Descendants("lic")
                                         on record.Element("lic_id").Value equals lic.Element("lic_id").Value
                                     join theory in theories.Descendants("theory")
                                         on record.Element("theory_id").Value equals theory.Element("theory_id").Value
                                     where lic.Element("category").Value == categGroup.Key
                                     select Int32.Parse(theory.Element("tresult").Value.ToString())).Min()
            select new XElement("question", l.question)
            )
            ));;

            partD.Save("partD.xml");

        }
    }
}
