using System;
using System.Collections.Generic;
using System.Text;
using HappyKids.Test.Controllers;

namespace HappyKids.Test.Helper
{
    public class DataInitializer
    {
        public static List<Student> GetAllProducts()
        {
            var students = new List<Student>
                               {
                                   new Student()
                                   {
                                       Name = "Samsung",
                                       BirthDate = UtilHelper.PareDateTime("24/01/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Mobile",
                                       BirthDate = UtilHelper.PareDateTime("24/01/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "HardDrive",
                                       BirthDate = UtilHelper.PareDateTime("24/11/2013")
                                   },
                                   new Student()
                                   {
                                       Name = "IPhone",
                                       BirthDate = UtilHelper.PareDateTime("28/08/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "IPad",
                                       BirthDate = UtilHelper.PareDateTime("29/07/2015")
                                   },
                                       new Student()
                                   {
                                       Name = "Fan",
                                       BirthDate = UtilHelper.PareDateTime("21/04/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Air Condition",
                                       BirthDate = UtilHelper.PareDateTime("04/09/2016")
                                   },
                                   new Student()
                                   {
                                       Name = "Triumph",
                                       BirthDate = UtilHelper.PareDateTime("19/11/2013")
                                   },
                                   new Student()
                                   {
                                       Name = "Kawasaki",
                                       BirthDate = UtilHelper.PareDateTime("28/12/2014")
                                   },
                                   new Student()
                                   {
                                       Name = "Vespa",
                                       BirthDate = UtilHelper.PareDateTime("29/11/2012")
                                   },
                                       new Student()
                                   {
                                       Name = "Harry",
                                       BirthDate = UtilHelper.PareDateTime("18/02/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Nelson",
                                       BirthDate = UtilHelper.PareDateTime("30/03/2012")
                                   },
                                   new Student()
                                   {
                                       Name = "Micheal",
                                       BirthDate = UtilHelper.PareDateTime("12/12/2012")
                                   },
                                   new Student()
                                   {
                                       Name = "Lawry",
                                       BirthDate = UtilHelper.PareDateTime("28/06/2014")
                                   },
                                   new Student()
                                   {
                                       Name = "Nass",
                                       BirthDate = UtilHelper.PareDateTime("29/07/2015")
                                   },
                                       new Student()
                                   {
                                       Name = "Irwin",
                                       BirthDate = UtilHelper.PareDateTime("24/01/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Foller",
                                       BirthDate = UtilHelper.PareDateTime("24/01/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Hasik",
                                       BirthDate = UtilHelper.PareDateTime("24/11/2013")
                                   },
                                   new Student()
                                   {
                                       Name = "Brad",
                                       BirthDate = UtilHelper.PareDateTime("28/08/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Bruno",
                                       BirthDate = UtilHelper.PareDateTime("22/07/2015")
                                   }
                                   ,    new Student()
                                   {
                                       Name = "Sunny",
                                       BirthDate = UtilHelper.PareDateTime("31/01/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Mark",
                                       BirthDate = UtilHelper.PareDateTime("19/01/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Dawson",
                                       BirthDate = UtilHelper.PareDateTime("08/11/2013")
                                   },
                                   new Student()
                                   {
                                       Name = "Tim",
                                       BirthDate = UtilHelper.PareDateTime("22/08/2015")
                                   },
                                   new Student()
                                   {
                                       Name = "Jack",
                                       BirthDate = UtilHelper.PareDateTime("11/07/2013")
                                   }
                               };
            return students;
        }
    }
}
