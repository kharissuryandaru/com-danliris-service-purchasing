﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.ViewModels.MonitoringUnitReceiptAllViewModel
{
	public class MonitoringUnitReceiptAll
	{
		public long id { get; set; }
		public string no { get; set; }
		public DateTimeOffset dateBon { get; set; }
		public string unit { get; set; }
		public string supplier { get; set; }
		public string doNo { get; set; }
		public string poEksternalNo { get; set; }
		public string poRefPR { get; set; }
		public string roNo { get; set; }
		public string article { get; set; }
		public string productCode { get; set; }
		public string productName { get; set; }
		public decimal qty { get; set; }
		public string uom { get; set; }
		public string remark { get; set; }
		public string user { get; set; }
		public string internNote { get; set; }
	}
}