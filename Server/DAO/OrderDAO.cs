/*  INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Define the Data Access Objet, and act as interface */


using System;
using System.Collections.Concurrent;
using System.Data.OracleClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Casestudy.DAL.DAO
{
    public class OrderDAO

    {
        public bool ProductOnBack { get; private set; }
        
       
        private AppDbContext _db;

        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public int AddOrder(
            int customerid, OrderSelectionHelper[] selections)
        {
            int orderId = -1;
            using (_db)
            {
                // usnig statement to handle a transaction as multiple entities are involved
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Order order = new Order();
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0.0M;
                        order.CustomerId = customerid;
                        // calculate the totalAmount and then add the order row to the table
                        foreach (OrderSelectionHelper selection in selections)

                        {
                            order.OrderAmount += Convert.ToDecimal(selection.product.MSRP * selection.Qty);
                        }

                        _db.Orders.Add(order);
                        _db.SaveChanges();

                        // then add each item to the OrderLineitems table // multiple rows
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            OrderLineItem oLineI = new OrderLineItem();
                            oLineI.OrderId = order.Id;
                            oLineI.SellingPrice += Convert.ToDecimal(selection.product.MSRP);
                            oLineI.ProductId = selection.product.Id;


                            if (selection.Qty < selection.product.QtyOnHand)  // enought stock
                            {
                                oLineI.QtyOrdered = selection.Qty;
                                oLineI.QtySold = selection.Qty; ;
                                oLineI.QtyBackOrdered = 0;
                            }
                            else // no enought stock  
                            {
                                oLineI.QtyOrdered = selection.Qty;
                                oLineI.QtySold = selection.product.QtyOnHand;
                                oLineI.QtyBackOrdered = selection.Qty - selection.product.QtyOnHand;

                                // help to show that a product is on backorder
                                ProductOnBack = true; 
                            }
                            _db.OrderLineItems.Add(oLineI);
                            _db.SaveChanges();

                            //Upadating the inventary

                            {
                                Product selproduct = new Product();
                                selproduct.Id = selection.product.Id;
                                selproduct.ProductName = selection.product.ProductName;
                                selproduct.GraphicName = selection.product.GraphicName;
                                selproduct.Timer = selection.product.Timer;
                                selproduct.CostPrice = selection.product.CostPrice;
                                selproduct.MSRP = selection.product.MSRP;
                                selproduct.QtyOnHand = 0;
                                selproduct.QtyOnBackOrder = 0;
                                selproduct.Description = selection.product.Description;

                                if (selection.Qty < selection.product.QtyOnHand)  // enought stock
                                {
                                    selproduct.QtyOnHand = selection.product.QtyOnHand -= selection.Qty;
                                    selproduct.QtyOnBackOrder = selection.product.QtyOnBackOrder;

                                }
                                else // not enought stock
                                {
                                    selproduct.QtyOnHand = selection.product.QtyOnHand = 0;
                                    selproduct.QtyOnBackOrder = selection.product.QtyOnBackOrder += selection.Qty -= selection.product.QtyOnHand;

                                }
                                    _db.Products.Update(selproduct);
                                _db.SaveChanges();
                            }


                    }
                        _trans.Commit();
                        orderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _trans.Rollback();
                    }
                }
            }
            return orderId;
        }
    }
}

