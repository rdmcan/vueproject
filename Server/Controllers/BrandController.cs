/*  INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Define the Controller to handle URL request by the action methods */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    [Authorize] //Only authorized customers can have access to the data from the server
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        AppDbContext _db;
        public BrandController(AppDbContext context) //injects here
        {
            _db = context;
        }
        public ActionResult<List<Brand>> Index()
        {
            BrandDAO dao = new BrandDAO(_db);
            List<Brand> allBrands = dao.GetAll();
            return allBrands;
        }
    }
}