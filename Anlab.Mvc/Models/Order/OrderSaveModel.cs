﻿using System;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnlabMvc.Models.Order
{
    public class OrderSaveModel
    {
        public int? OrderId { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter a sample quantity greater than zero.")]
        public int Quantity { get; set; }

        [Required]
        public string SampleType { get; set; }

        public string AdditionalInfo { get; set; }

        [MinLength(1, ErrorMessage = "You must select at least 1 test.")]
        public TestItem[] SelectedTests { get; set; }

        public decimal Total { get; set; }

        [Required]
        [StringLength(256)]
        public string Project { get; set; }

        public Payment Payment { get; set; }

        public IList<string> AdditionalEmails { get; set; }
    }
}