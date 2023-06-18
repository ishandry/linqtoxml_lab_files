using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace cscsv
{
    internal class Biblio
    {
        static void Main(string[] args)
        {
            var biblio = XDocument.Load("biblio.xml");
            var books = XDocument.Load("books.xml");
            var genres = XDocument.Load("genres.xml");
            var publishers = XElement.Load("publishers.xml");
            Console.WriteLine("Loaded files");

            XDocument partA = new XDocument(new XElement("partA", 
                from record in biblio.Descendants("record")
                join book in books.Descendants("book")
                    on record.Element("book_id").Value equals book.Element("book_id").Value
                join publ in publishers.Descendants("publisher")
                    on record.Element("publisher_id").Value equals publ.Element("publisher_id").Value
                group new
                {
                    title = book.Element("title").Value,
                    publisherName = publ.Element("publisher_name").Value
                } 
                by book.Element("author").Value into authorGroup
                orderby authorGroup.Key ascending
                select
                new XElement("author", 
                new XAttribute("name", authorGroup.Key),
                from g in authorGroup
                orderby g.title ascending
                select new XElement("book",
                new XAttribute("title", g.title),
                new XElement("publisher_name",
                g.publisherName
                ))
                ))
                );

                //)));

            partA.Save("partA.xml");

            XDocument partB = new XDocument(new XElement("partB",
            from record in biblio.Descendants("record")
            join book in books.Descendants("book")
                on record.Element("book_id").Value equals book.Element("book_id").Value
            join publ in publishers.Descendants("publisher")
                on record.Element("publisher_id").Value equals publ.Element("publisher_id").Value
            group new
            {
                title = book.Element("title").Value,
                date = DateOnly.Parse(record.Element("delivery_day").Value),
                publisherName = publ.Element("publisher_name").Value
            }
            by book.Element("author").Value into authorGroup
            orderby authorGroup.Key ascending
            select
            new XElement("author",
            new XAttribute("name", authorGroup.Key),
            from g in authorGroup
            where g.date == authorGroup.Max(x => x.date)
            orderby g.title ascending
            select new XElement("book",
            new XAttribute("title", g.title),
            new XElement("publisher_name",
            g.publisherName
            ),
            new XElement("date", g.date)
            )
            ))
            );

            partB.Save("partB.xml");




            var partC = new XDocument(new XElement("partC",
            from record in biblio.Descendants("record")
            join publ in publishers.Descendants("publisher")
                on record.Element("publisher_id").Value equals publ.Element("publisher_id").Value
            join book in books.Descendants("book")
                on record.Element("book_id").Value equals book.Element("book_id").Value
            group new
            {
                author = book.Element("author").Value
            }
            by 
                publ.Element("publisher_name").Value

            into publGroup
            orderby publGroup.Key ascending
            select new XElement("publisher",
            new XAttribute("label", publGroup.Key),
            from g in publGroup
            group g by g.author into ag
            orderby ag.Key ascending
            select new XElement("author", new XAttribute("name", ag.Key))
            )
            ));

            partC.Save("partC.xml");

            var r = biblio.Descendants("record").GroupBy(record => record.Element("genre_id").Value).Max(g => g.Count());
            Console.WriteLine(r);

            var partD = new XDocument(new XElement("top_genres",
                new XElement("book_of_top_genre", (from record in biblio.Descendants("record")
                                       group record by record.Element("genre_id").Value into countGroup
                                       select countGroup.Count()).Max()),
                from record in biblio.Descendants("record")
                join genre in genres.Descendants("genre")
                    on record.Element("genre_id").Value equals genre.Element("genre_id").Value
                group genre
                by genre.Element("genre_name").Value into genreGroup
                where genreGroup.Count() == (from record in biblio.Descendants("record")
                                             group record by record.Element("genre_id").Value into countGroup
                                             select countGroup.Count()).Max()
                orderby genreGroup.Key ascending
                select new XElement("genre",
                new XAttribute("name", genreGroup.Key)
                )
                 )) ;

            partD.Save("partD.xml");

        }
    }
}