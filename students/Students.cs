
//Лабораторна робота з програмування (16 травня)
//Виконав: Іщук Андрій, студент групи ПМі-21

// * Усі вхідні та вихідні файли повинні знаходитися (знаходяться) в папці {деректорія проекту}/bin/Debug/net{version}

//Вхідні файли (обов'язкові, для запуску програми):
//  1) "students.xml"
//  2) "teachers.xml"
//  3) "his_results.xml"
//  4) "prog_results.xml"

//Вхідні файли (створюються програмою):
//  1) "partA.xml"
//  2) "partB.xml"
//  3) "partC.xml"
//  4) "partD.xml"
//(відповідають підзавданням лабораторної: (а) - partA; (б) - partB; (в) - partC; (г) - partD;)

//          Завдання лабораторної роботи:
//1.Розробити засоби для пiдведення пiдсумкiв вивчення дисциплiн на окремому курсi окремої спецi-
//альностi.
//Студент характеризується номером студквитка, прiзвищем та iм’ям, назвою групи.
//Викладач характеризується iдентифiкацiйним номером, прiзвищем та iм’ям.
//Усi персональнi данi задано окремими xml-файлами.
//Результати з окремої дисциплiни задано xml-файлом за схемою: назва дисциплiни, iдентифiкацiйний
//номер викладача i рядки з парами <номер студквитка, кiлькiсть балiв>.
//Данi з рiзних дисциплiн (не менше двi дисциплiни, довiльна кiлькiсть результатiв) задано окремими
//файлами.

//2. Отримати (використовуючи linq):

//(а)xml - файл, де результати систематизованi за схемою <назва дисциплiни, прiзвище та iнiцiали
//викладача, назва групи, перелiк результатiв у виглядi пар <прiзвище та iнiцiали студента,
// кiлькiсть балiв>; вмiст впорядкувати у лексико-графiчному порядку за назвою дисциплiни,
// назвою групи i прiзвищем студента;

//(б)xml - файл, де результати систематизованi за схемою < назва групи, перелiк результатiв у ви-
//глядi <прiзвище та iнiцiали студента> та пари <назва дисциплiни, кiлькiсть балiв>; вмiст
//впорядкувати у лексико-графiчному порядку за назвою групи i прiзвищем студента;

//(в)xml - файл, описаний у попередньому завданнi, але без врахування студентiв з незадовiльними
//балами (меншими 51);

//(г)xml - файл, в якому подано рейтинг студентiв за сумарною кiлькiстю балiв з усiх дисциплiн без
//врахування студентiв з незадовiльними балами.

using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace csstudentslab
{
    internal class Students
    {
        static void Main(string[] args)
        {
            var students = XDocument.Load("students.xml");
            var teachers = XDocument.Load("teachers.xml");
            var his_results = XDocument.Load("his_results.xml");
            var prog_results = XDocument.Load("prog_results.xml");

            //обєднання результатів предметів
            XDocument subjects = new XDocument(
                new XElement("subjects",
                from his in his_results.Descendants("results")
                select his,
                from prg in prog_results.Descendants("results")
                select prg
                ));

            Console.WriteLine(subjects);

            XDocument partA = new XDocument(
                new XElement("subjects_results",
                from results in subjects.Descendants("results")
                orderby results.Descendants("subject").First()?.Value ascending
                //ітерація по предметах
                select
                new XElement("results",
                new XAttribute("subject", results.Descendants("subject").First()?.Value),
                new XAttribute("teacher",
                //<Ishchuk> <Andriy>  ---> <Ishchuk A.>
                    String.Format("{0} {1}.",
                        teachers.Descendants("teacher")
                        .FirstOrDefault(t => t.Element("teacher_id")?.Value == results.Descendants("teacher_id").First()?.Value)
                        .Element("lastname")?.Value,
                         teachers.Descendants("teacher")
                        .FirstOrDefault(t => t.Element("teacher_id")?.Value == results.Descendants("teacher_id").First()?.Value)
                        .Element("firstname")?.Value.ToString().Substring(0, 1)

                    )
                ),

                //ітерація по студентах (групування в групи):
                from hres in results.Descendants("result")
                //до результатів з предмету який відповідає даній ітерації
                //додаємо інформацію про імя студента
                join student in students.Descendants("student")
                    on hres.Element("student_id").Value equals student.Element("student_id").Value

                group new
                {
                    score = hres.Element("score").Value,
                    lastname = student.Element("lastname").Value,
                    firstname = student.Element("firstname").Value
                }
                by student.Element("group").Value into academicGroup


                orderby academicGroup.Key ascending

                select new XElement("group_results",
                new XAttribute("group", academicGroup.Key),
                //ітерація по студентах (тільки з певної групи "academicGroup.Key" (student.Element("group").Value)):
                from s in academicGroup
                orderby s.lastname ascending
                select new XElement("student_score",
                new XAttribute("student",
                //<Ishchuk> <Andriy>  ---> <Ishchuk A.>
                String.Format("{0} {1}.",
                s.lastname,
                s.firstname.ToString().Substring(0, 1)
                )
                ),
                //Value елемента з student_score (з атрибутом student="Ishchuk A.") :
                s.score
                )
                )

                )));
            partA.Save("partA.xml");


            XDocument partB = new XDocument(
            new XElement("groups_results",

            from student in students.Descendants("student")
            group student by student.Element("group").Value into academicGroup
            orderby academicGroup.Key ascending

            select new XElement("group_res", 
            new XAttribute("group", academicGroup.Key),
            from s in academicGroup

            orderby s.Element("lastname").Value ascending

            select new XElement("student_res",

                //<Ishchuk> <Andriy>  ---> <Ishchuk A.>
                new XAttribute("student",
                String.Format("{0} {1}.",
                s.Element("lastname").Value,
                s.Element("firstname").Value.ToString().Substring(0, 1)
                )
                ),

                
                from results in subjects.Descendants("results")
                    //subjects - список results-результатів з предметів
                    //results-результати - список результатів з предмету для всіх студентів
                    //(IEnumerable результатів (<result>...Ishchuk...71...<result>) з якогось предмету)
                orderby results.Descendants("subject").First()?.Value ascending

                //для кожного предмету знаходимо result, що відповідає студенту "s"
                //і кладемо в елемент "score" (з атрибутом назви предмету)
                select
                new XElement("score",
                new XAttribute("subject", results.Descendants("subject").First()?.Value),
                from res in results.Descendants("result")
                where res.Element("student_id").Value == s.Element("student_id").Value
                select res.Element("score").Value
                )
                )

            )));
            partB.Save("partB.xml");


            XDocument partC = new XDocument(
            new XElement("groups_results_51plus_only",
                from student in students.Descendants("student")
                group student by student.Element("group").Value into academicGroup
                orderby academicGroup.Key ascending
                select new XElement("group_res",
                new XAttribute("group", academicGroup.Key),
                from s in academicGroup
                //тільки студенти, в яких з усіх дисциплін колекції результатів "subjects"
                //бал більше 51
                where
                    (
                        from sr in
                        subjects.Root.Elements("results")
                            //тут sr (subject-result)- IEnumerable результатів (<result>...Ishchuk...71...<result>) з якогось предмету
                        select sr.Descendants("result").ToList()
                        .All(
                            //усі результати з предмету повинні не відноситись до студента "s" (не наш студент),
                            x => x.Element("student_id").Value.ToString() != s.Element("student_id").Value.ToString()

                            //або бути більше 51
                            | Int32.Parse(x.Element("score").Value.ToString()) > 51
                        )
                    )
                    //перевіряємо чи кожен премет "sr" повернув true ("x")
                    .All(x => x == true)
                
                orderby s.Element("lastname").Value ascending
                select new XElement("student_res",
                    new XAttribute("student",
                    String.Format("{0} {1}.",
                    s.Element("lastname").Value,
                    s.Element("firstname").Value.ToString().Substring(0, 1)
                    )
                    ),
                    from results in subjects.Descendants("results")
                    orderby results.Descendants("subject").First()?.Value ascending
                    select
                    new XElement("score",
                    new XAttribute("subject", results.Descendants("subject").First()?.Value),
                    from res in results.Descendants("result")
                    where res.Element("student_id").Value == s.Element("student_id").Value
                    select res.Element("score").Value
                    )
                    )

                )));
            partC.Save("partC.xml");

            XDocument partD = new XDocument(
            new XElement("ratings",
                from s in students.Descendants("student")
                where
                    (
                        from sr in
                        subjects.Root.Elements("results")
                        select sr.Descendants("result").ToList()
                        .All(
                            x => x.Element("student_id").Value.ToString() != s.Element("student_id").Value.ToString()
                            | Int32.Parse(x.Element("score").Value.ToString()) > 51
                        )
                    )
                    .All(x => x == true)

                //сортуємо за сумою "score" з у усіх предметів (та ідентифікатором студента)
                orderby (from res in subjects.Descendants("results").Descendants("result")
                         where res.Element("student_id").Value == s.Element("student_id").Value
                         select Int32.Parse(res.Element("score").Value)).Sum()
                descending

                select new XElement("student_total",
                    new XAttribute("student",
                    String.Format("{0} {1}.",
                    s.Element("lastname").Value,
                    s.Element("firstname").Value.ToString().Substring(0, 1)
                    )
                    ),
                    (from res in subjects.Descendants("results").Descendants("result")
                    where res.Element("student_id").Value == s.Element("student_id").Value
                    select Int32.Parse(res.Element("score").Value)).Sum()
                    )

                ));
            partD.Save("partD.xml");

        }
    }
}
