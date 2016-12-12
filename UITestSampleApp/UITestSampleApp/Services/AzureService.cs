using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

using Xamarin.Forms;

namespace UITestSampleApp
{
	public class AzureService : IDataService
	{
		const string _azureDataServiceUrl = @"https://mobile-864df958-bcca-401d-8f93-ae159cd5a9d3.azurewebsites.net";

		bool _isInitialized;
		MobileServiceUser _user;
		MobileServiceClient _mobileService;


		public async Task Initialize()
		{
			if (_isInitialized)
				return;

			// MobileServiceClient handles communication with our backend, auth, and more for us.
			_mobileService = new MobileServiceClient(_azureDataServiceUrl);

			// Configure online/offline sync.
			var path = DependencyService.Get<IEnvironment>().GetFilePath("app.db");
			var store = new MobileServiceSQLiteStore(path);
			store.DefineTable<ListPageDataModel>();
			await _mobileService.SyncContext.InitializeAsync(store);//, new SyncHandler(MobileService));

			_isInitialized = true;
		}

		#region Data Access
		public async Task<IEnumerable<T>> GetItems<T>() where T : EntityData
		{
			await Initialize();

			await SyncItems<T>();

			return await _mobileService.GetSyncTable<T>().ToEnumerableAsync();
		}

		public async Task<T> GetItem<T>(string id) where T : EntityData
		{
			await Initialize();

			await SyncItems<T>();

			return await _mobileService.GetSyncTable<T>().LookupAsync(id);
		}

		public async Task AddItem<T>(T item) where T : EntityData
		{
			await Initialize();

			await _mobileService.GetSyncTable<T>().InsertAsync(item);
			await SyncItems<T>();
		}

		public async Task UpdateItem<T>(T item) where T : EntityData
		{
			await Initialize();

			await _mobileService.GetSyncTable<T>().UpdateAsync(item);
			await SyncItems<T>();
		}

		public async Task RemoveItem<T>(T item) where T : EntityData
		{
			await Initialize();

			await _mobileService.GetSyncTable<T>().DeleteAsync(item);
			await SyncItems<T>();
		}

		public async Task SyncItems<T>() where T : EntityData
		{
			await Initialize();

			try
			{
				await _mobileService.SyncContext.PushAsync();
				await _mobileService.GetSyncTable<T>().PullAsync($"all{typeof(T).Name}", _mobileService.GetSyncTable<T>().CreateQuery());
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error during Sync occurred: {ex.Message}");
			}
		}

		public async Task<bool> LoginAsync()
		{
			await Initialize();

			var success = false;
			var message = string.Empty;

			try
			{
				// Sign in with Facebook login using a server-managed flow.
				if (_user == null)
				{
					_user = await DependencyService.Get<IAuthenticate>().Authenticate(_mobileService);
					if (_user != null)
					{
						message = string.Format($"You are now signed-in as {_user.UserId}.");
						success = true;
					}
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}

			await App.Current.MainPage.DisplayAlert("Sign-in result", message, "OK");

			return success;
		}
		#endregion
	}
}
