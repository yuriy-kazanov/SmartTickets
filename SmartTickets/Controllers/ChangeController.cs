﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartTickets.Models;
using System.Data.Entity;
using System.Globalization;
using Microsoft.AspNet.Identity;


namespace SmartTickets.Controllers
{
    public class ChangeController : Controller
    {
        TicketsContext db = new TicketsContext();
        // GET: Change
        public ActionResult Index(int id)
        {
            var email = User.Identity.GetUserName();
            var order = db.Orders.First(x => x.Id == id && x.Email == email);
            return View(order);
        }

        public ActionResult NewChange()
        {
            var email = User.Identity.GetUserName();
            var changes = db.TicketsToChange.Where(x => x.Email != email).ToList();

            return View(changes);
        }

        public ActionResult Sell(int id, int count, string price)
        {
            var email = User.Identity.GetUserName();
            var order = db.Orders.First(x => x.Id == id && x.Email == email);
            Change item = new Change();
            item.Count = count;
            item.Price = int.Parse(price);
            item.OrderId = id;
            item.Email = email;
            item.EventId = order.EventId;
            db.TicketsToChange.Add(item);
            order.Count -= count;
            if (order.Count == 0)
            {
                db.Orders.Remove(order);
            }
            db.SaveChanges();
            return View(item);
        }
        public ActionResult Buy(int changeId)
        {
            var email = User.Identity.GetUserName();
            var change = db.TicketsToChange.First(x => x.Id == changeId);
            Order order = new Order();
            order.Email = email;
            order.EventId = change.EventId;

            order.Count = change.Count;
            order.Number = (email + (change.EventId * 100).ToString() + change.Count.ToString()).GetHashCode().ToString();
            order.Date = DateTime.Now;
            db.Orders.Add(order);
            db.TicketsToChange.Remove(change);
            db.SaveChanges();
            return View();
        }
    }
}