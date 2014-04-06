using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSearcher.Models
{
	public class SearchResult
	{
		public string ArticleTitle { get; set; }
		public string Term { get; set; }
		public string CalloutText { get; set; }
		public string SourceUrl { get; set; }

		public override string ToString()
		{
			return string.Format( "Search={0}, Title={1}, Callout={2}, Url={3}",
				Term, ArticleTitle, CalloutText, SourceUrl);
		}
	}
}
