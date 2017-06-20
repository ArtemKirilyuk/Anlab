﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnlabMvc.Services
{
    public interface IOrderService
    {
        Task PopulateOrder(OrderSaveModel model, Order orderToUpdate);
        void PopulateOrderWithLabDetails(OrderSaveModel model, Order orderToUpdate);
        Task SendOrderToAnlab(Order order);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<TestDetails[]> CalculateTestDetails(OrderDetails orderDetails)
        {
            var selectedTestCodes = orderDetails.SelectedTests.Select(t => t.Code);
            var tests = await _context.TestItems.Where(a => selectedTestCodes.Contains(a.Code)).ToListAsync();

            var calcualtedTests = new List<TestDetails>();

            foreach (var test in orderDetails.SelectedTests)
            {
                var dbTest = tests.Single(t => t.Code == test.Code);

                var cost = orderDetails.Payment.IsInternalClient ? dbTest.InternalCost : dbTest.ExternalCost;
                var costAndQuantity = cost * orderDetails.Quantity;

                calcualtedTests.Add(new TestDetails
                {
                    Analysis = dbTest.Analysis,
                    Code = dbTest.Code,
                    SetupCost = dbTest.SetupCost,
                    InternalCost = dbTest.InternalCost,
                    ExternalCost = dbTest.ExternalCost,
                    Cost = cost,
                    SubTotal = costAndQuantity,
                    Total = costAndQuantity + dbTest.SetupCost
                });
            }

            return calcualtedTests.ToArray();
        }
        public async Task PopulateOrder(OrderSaveModel model, Order orderToUpdate)
        {
            orderToUpdate.Project = model.Project;
            orderToUpdate.JsonDetails = JsonConvert.SerializeObject(model);
            var orderDetails = orderToUpdate.GetOrderDetails();

            var tests = await CalculateTestDetails(orderDetails);

            orderDetails.SelectedTests = tests.ToArray();
            orderDetails.Total = orderDetails.SelectedTests.Sum(x=>x.Total);

            AddAdditionalFees(orderDetails);

            orderToUpdate.SaveDetails(orderDetails);
            orderToUpdate.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);
        }

        public void PopulateOrderWithLabDetails(OrderSaveModel model, Order orderToUpdate)
        {
            var orderDetails = orderToUpdate.GetOrderDetails();
            orderDetails.Total += orderDetails.AdjustmentAmount;
            orderToUpdate.SaveDetails(orderDetails);
        }

        public async Task SendOrderToAnlab(Order order)
        {
            //TODO: Implement this.
            return;
        }

        private static void AddAdditionalFees(OrderDetails orderDetails)
        {
            if (string.Equals(orderDetails.SampleType, "Water", StringComparison.OrdinalIgnoreCase))
            {
                if (orderDetails.FilterWater)
                {
                    orderDetails.Total += orderDetails.Quantity * (orderDetails.Payment.IsInternalClient ? 11 : 17);
                }
            }
            if (string.Equals(orderDetails.SampleType, "Soil", StringComparison.OrdinalIgnoreCase))
            {
                if (orderDetails.Grind)
                {
                    orderDetails.Total += orderDetails.Quantity * (orderDetails.Payment.IsInternalClient ? 6 : 9);
                }
                if (orderDetails.ForeignSoil)
                {
                    orderDetails.Total += orderDetails.Quantity * (orderDetails.Payment.IsInternalClient ? 9 : 14);
                }
            }
            if (string.Equals(orderDetails.SampleType, "Plant", StringComparison.OrdinalIgnoreCase))
            {
                if (orderDetails.Grind)
                {
                    orderDetails.Total += orderDetails.Quantity * (orderDetails.Payment.IsInternalClient ? 6 : 9);
                }
            }
        }
    }
}

