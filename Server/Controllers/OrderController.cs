/*  INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Define the Controller to handle URL request by the action methods */

using System;
using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    [Authorize] //Only authorized customers can have access to the data from the server
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        AppDbContext _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }
        [HttpPost]
        [Produces("application/json")]
        public ActionResult<string> Index(OrderHelper helper)
        {
            string retVal = "";
            try
            {
                CustomerDAO cDao = new CustomerDAO(_ctx);
                Customer OrderOwner = cDao.GetByEmail(helper.email);
                OrderDAO oDao = new OrderDAO(_ctx);
                int orderId = oDao.AddOrder(OrderOwner.Id, helper.selections);
                
                if (orderId > 0 )
                {
                    retVal = "Order" + orderId + " created!";
                    if (oDao.ProductOnBack)
                    {
                        retVal += "Goods Backordered!!";
                    }
                }
                else 
                {
                    retVal = "Order not created ";
                }
            }
            catch (Exception ex)
            {
                retVal = "Order not created " + ex.Message;
            }
            return retVal;
        }

    }
}