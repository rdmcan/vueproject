/*  INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Define the Data Access Objet Define the Data Access Objet, and act as interface*/

using System;
using Casestudy.DAL.DomainClasses;
using System.Linq;

namespace Casestudy.DAL.DAO
{
    public class CustomerDAO
    {
        private AppDbContext _db;
        public CustomerDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public Customer Register(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();
            return customer;
        }
        public Customer GetByEmail(string email)
        {
            Customer customer = _db.Customers.FirstOrDefault(c => c.Email == email);
            return customer;
        }
    }
}
