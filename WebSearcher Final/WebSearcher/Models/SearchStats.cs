using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JulMar.Windows.Mvvm;

namespace WebSearcher.Models
{
	struct SearchStats 
	{
		public string CurrentAction { get; set; }
		public string CurrentTarget { get; set; }
		public int DownloadCount { get; set; }

		public void Clear()
		{
			this.CurrentAction = "Idle";
			this.CurrentTarget = "";
			this.DownloadCount = 0;
		}
	}
}
