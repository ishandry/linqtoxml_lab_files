using System.Xml.Linq;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Demo
{
    internal class Projects
    {
        static void Main(string[] args)
        {
            var auds = XElement.Load("auds.xml");
            var employees = XElement.Load("employees.xml");
            var positions = XElement.Load("positions.xml");
            var projects = XElement.Load("projects.xml");

            Console.WriteLine(auds);
            Console.WriteLine(employees);
            Console.WriteLine(positions);
            Console.WriteLine(projects);

            XDocument taskA = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("results",
                from a in auds.Descendants("aud")
                join e in employees.Descendants("employee")
                     on a.Element("emp_id").Value equals e.Element("emp_id").Value
                join p in projects.Descendants("project")
                    on a.Element("prj_id").Value equals
                    p.Element("prj_id").Value
                group new
                {
                    prj_name = p.Element("prj_name").Value,
                    emp_name = e.Element("lname").Value,
                    total_hours = decimal.Parse(a.Element("hours").Value)
                } by new
                {
                    prj_name = p.Element("prj_name").Value,
                    emp_name = e.Element("lname").Value,
                } into g
                orderby g.Key.prj_name, g.Key.emp_name
                select
                new XElement("result",
                    new XElement("prj_name", g.Key.prj_name),
                    new XElement("emp_name", g.Key.emp_name),
                    new XElement("total_hours", g.Sum(x => x.total_hours))
                )
            ));

            taskA.Save("taskA.xml");

            XDocument taskB = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("results",
                from a in auds.Descendants("aud")
                join e in employees.Descendants("employee")
                     on a.Element("emp_id").Value equals e.Element("emp_id").Value
                join p in projects.Descendants("project")
                    on a.Element("prj_id").Value equals
                    p.Element("prj_id").Value
                join pos in positions.Descendants("position")
                     on e.Element("pos_id").Value equals
                     pos.Element("pos_id").Value
                group new
                {
                    prj_name = p.Element("prj_name").Value,
                    emp_name = e.Element("lname").Value,
                    payrate = decimal.Parse(pos.Element("payrate").Value),
                    total_hours = decimal.Parse(a.Element("hours").Value)
                } by new
                {
                    prj_name = p.Element("prj_name").Value,
                    emp_name = e.Element("lname").Value,
                } into g
                orderby g.Key.prj_name, g.Key.emp_name
                select
                new XElement("result",
                    new XElement("prj_name", g.Key.prj_name),
                    new XElement("emp_name", g.Key.emp_name),
                    new XElement("total_hours", g.Sum(x => x.total_hours * x.payrate))
                )
            ));

            taskB.Save("taskB.xml");

            XDocument taskС = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("results",
                 from prg in projects.Descendants("project")
                 orderby prg.Element("prj_name").Value
                 select new XElement("project",
                 new XElement("prj_name", prg.Element("prj_name").Value),
                 from a in auds.Descendants("aud")
                 join e in employees.Descendants("employee")
                      on a.Element("emp_id").Value equals e.Element("emp_id").Value
                 join p in projects.Descendants("project")
                     on a.Element("prj_id").Value equals
                     p.Element("prj_id").Value
                 join pos in positions.Descendants("position")
                      on e.Element("pos_id").Value equals
                      pos.Element("pos_id").Value
                 group new
                 {
                     prj_name = p.Element("prj_name").Value,
                     position = pos.Element("pos_name").Value,
                     spent_hours = decimal.Parse(a.Element("hours").Value)
                 } by new
                 {
                     prj_name = p.Element("prj_name").Value,
                     position = pos.Element("pos_name").Value
                 } into g
                 where g.Key.prj_name == prg.Element("prj_name").Value
                 select
                 new XElement("position_hours",
                     new XElement("position", g.Key.position),
                     new XElement("total_hours", g.Sum(x => x.spent_hours))
                 )
            ))

                );

            taskС.Save("taskС.xml");



            XDocument taskG = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("results",
                 from prg in projects.Descendants("project")
                 orderby prg.Element("prj_name").Value
                 select new XElement("project",
                 new XElement("prj_name", prg.Element("prj_name").Value),
                 from a in auds.Descendants("aud")
                 join e in employees.Descendants("employee")
                      on a.Element("emp_id").Value equals e.Element("emp_id").Value
                 join p in projects.Descendants("project")
                     on a.Element("prj_id").Value equals
                     p.Element("prj_id").Value
                 join pos in positions.Descendants("position")
                      on e.Element("pos_id").Value equals
                      pos.Element("pos_id").Value
                 group new
                 {
                     prj_name = p.Element("prj_name").Value,
                     payrate = decimal.Parse(pos.Element("payrate").Value),
                     spent_hours = decimal.Parse(a.Element("hours").Value)
                 } by new
                 {
                     prj_name = p.Element("prj_name").Value,
                 } into g
                 where g.Key.prj_name == prg.Element("prj_name").Value
                 select
                 new XElement("position_hours",
                     new XElement("costs", g.Sum(x => x.spent_hours * x.payrate)),
                     new XElement("total_hours", g.Sum(x => x.spent_hours))
                 )
            ))

                );

            taskG.Save("taskG.xml");
        }
    }
}


