using JulMar.Windows.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using WebSearcher.Models;
using System.Collections.Generic;

namespace WebSearcher.ViewModels
{
	internal class MainWindowViewModel : ViewModel
	{
		private SearchStats SearchStats;

		public ICommand SearchCommand { get; set; }
		public ICommand VisitSelectedSiteCommand { get; set; }
		public ObservableCollection<SearchResult> SearchResults { get; set; }
		public SearchResult SelectedResult { get; set; }
		#region public string SearchText {get; set;}
		private string searchText;
		public string SearchText
		{
			get { return searchText; }
			set
			{
				searchText = value;
				CommandManager.InvalidateRequerySuggested();
				this.OnPropertyChanged( "SearchText" );
			}
		}
		#endregion
		#region public string StatsText {get; set;}

		private string statsText;
		public string StatsText
		{
			get { return statsText; }
			set
			{
				statsText = value;
				CommandManager.InvalidateRequerySuggested();
				this.OnPropertyChanged( "StatsText" );
			}
		}
		#endregion
		#region public bool IsSearching {get; set;}
		private bool isSearching;
		public bool IsSearching
		{
			get
			{
				return isSearching;
			}
			set
			{
				isSearching = value;
				CommandManager.InvalidateRequerySuggested();
			}
		}
		#endregion

		public MainWindowViewModel()
		{
			SearchCommand = new DelegatingCommand( OnSearch, CanSearch );
			VisitSelectedSiteCommand = new DelegatingCommand( OnVisit, CanVisit );

			SearchResults = new ObservableCollection<SearchResult>();
			SearchText = "";
			StatsText = "Idle";

			UpdateStats();
		}

		private async void OnSearch()
		{
			IsSearching = true;
			SearchResults.Clear();

			SearchStats.DownloadCount = TheEntireInternet.Urls.Count;

			List<Task<DownloadResult>> tasks = new List<Task<DownloadResult>>();

			foreach ( string url in TheEntireInternet.Urls )
			{
				tasks.Add( DownloadWithUrlTrackingTaskAsync( url ) );
			}

			while ( tasks.Count > 0 )
			{
				Task<DownloadResult> finishTask = await Task.WhenAny<DownloadResult>( tasks.ToArray() );
				tasks.Remove( finishTask );
				IsSearching = tasks.Count > 0;

				SearchStats.DownloadCount--;
				SearchStats.CurrentTarget = finishTask.Result.Url;
				SearchStats.CurrentAction = "Searching text";

				SearchResult[] terms = await Task.Run<SearchResult[]>( () =>
				{
					DownloadResult res = (DownloadResult)finishTask.Result;
					string html = res.Data;
					string url = res.Url;
					return SearchEngine.Search( SearchText, html, url );
				} );

				foreach ( SearchResult result in terms )
				{
					SearchResults.Add( result );
				}
			}
		}

		private bool CanSearch()
		{
			return
				IsSearching == false &&
				SearchText != null &&
				SearchText.Trim().Length > 0;
		}

		private void OnVisit()
		{
			Process.Start( SelectedResult.SourceUrl );
		}

		private bool CanVisit()
		{
			return SelectedResult != null;
		}

		private async void UpdateStats()
		{
			while ( true )
			{
				await Task.Delay( 100 );

				if ( SearchStats.DownloadCount > 0 )
				{
					StatsText = string.Format( "[{0} pending] {1} on {2}.",
										SearchStats.DownloadCount,
										SearchStats.CurrentAction,
										SearchStats.CurrentTarget );
				}
				else
				{
					StatsText = "Current action: Idle";
				}
			}
		}

		private async Task<DownloadResult> DownloadWithUrlTrackingTaskAsync(string url)
		{
			DownloadResult result = new DownloadResult();
			result.Url = url;
			result.Data = await Downloader.DownloadHtmlAsyncTask( url );

			return result;
		}
	}
}