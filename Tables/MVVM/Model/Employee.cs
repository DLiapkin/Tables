﻿using System;

namespace Tables.MVVM.Model
{
    /// <summary>
    /// Represents employee entity for Database
    /// </summary>
    class Employee
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
