using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WebSearcher.Models
{
	public static class SearchEngine
	{
		public static SearchResult[] Search(string term, string htmlSource, string url)
		{
			HtmlToText converter = new HtmlToText();
			string text = converter.ConvertHtml( htmlSource );

			var lowerTerm = term.ToLower();
            string[] words = text.ToLower().Split(',', ' ', '.', ' ', '\t', '\r', '\n', '!', '?');

			var results =
				(from word in words
				let isRealWord = word != null && word.Trim().Length > 0
				let isMatch = isRealWord && word.Contains(lowerTerm)
				where isRealWord && isMatch
				select new SearchResult
				{
					ArticleTitle = GetTitle(htmlSource),
					Term = term,
					SourceUrl = url,
				}).ToList();

			FillOutCalloutText( results, text );

			return results.ToArray();
		}

		private static void FillOutCalloutText(List<SearchResult> results, string text)
		{
			int index = 0;
			text = text.ToLower().Replace( "\r\n", " " );
			foreach ( var result in results )
			{
				if ( index < 0 )
					result.CalloutText = "[no call out text available]";
				else
				{
					index = text.IndexOf( result.Term.ToLower(), index );
					result.CalloutText = GetCalloutSentence( text, index );
					index++;
				}
				
			}
		}

		private static string GetCalloutSentence(string text, int index)
		{
			int start = Math.Max( 0, index - 10 );
			int end = Math.Min( text.Length - 1, index + 40 );

			try
			{
				return "... " + text.Substring( start, end - start ).Trim() + "...";
			}
			catch ( Exception x)
			{
				Debug.WriteLine( x );
				return "[no sentence]";
			}
		}

		private static string GetTitle(string htmlSource)
		{
			var regex = new Regex( @"(?<=<title.*>)([\s\S]*)(?=</title>)" );
			Match match = regex.Match( htmlSource );

			if ( match.Success )
				return match.Value;
			else
				return "[no title]";
		}
	}
}