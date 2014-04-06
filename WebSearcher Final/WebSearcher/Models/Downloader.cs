using System;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebSearcher.Models
{
	class Downloader
	{
		public async static Task<string> DownloadHtmlAsyncTask(string url)
		{
			Debug.WriteLine( "Starting download for " + url );

			WebClient client = new WebClient();
			var download = Task.Run<string>( () => { try { return client.DownloadString( url ); } catch { return ""; } } );
			await download;

			Debug.WriteLine( "Finished download of " + url );

			return download.Result;
		}

		public static string DownloadHtml(string url)
		{
			Debug.WriteLine( "Starting download for " + url );

			WebClient client = new WebClient();
			var download = client.DownloadString( url );

			Debug.WriteLine( "Finished download of " + url );
			
			return download;
		}
	}
}
