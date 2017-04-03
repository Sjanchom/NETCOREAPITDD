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
                                   }
                               };
            return students;
        }
    }
}
