using System;
using System.Threading.Tasks;

using Android.App;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;

using UITestSampleApp.Droid;

[assembly: Dependency(typeof(Authenticate_Android))]
namespace UITestSampleApp.Droid
{
	public class Authenticate_Android : IAuthenticate
	{
		MobileServiceUser user;

		public async Task<bool> Authenticate()
		{
			var success = false;
			var message = string.Empty;
			try
			{
				// Sign in with Facebook login using a server-managed flow.
				user = await DependencyService.Get<IDataService>().Login();
				if (user != null)
				{
					message = string.Format($"you are now signed-in as {user.UserId}.");
					success = true;
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}

			DisplayAlert(message);

			return success;
		}

		void DisplayAlert(string message)
		{
			var builder = new AlertDialog.Builder(Android.App.Application.Context);
			builder.SetMessage(message);
			builder.SetTitle("Sign-in result");
			builder.Create().Show();
		}
	}
}
