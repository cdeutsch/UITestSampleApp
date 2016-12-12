using System;
using System.Threading.Tasks;

using UIKit;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;

using UITestSampleApp.iOS;

[assembly: Dependency(typeof(Authenticate_iOS))]
namespace UITestSampleApp.iOS
{
	public class Authenticate_iOS : IAuthenticate
	{
		public Task<MobileServiceUser> Authenticate(MobileServiceClient client)
		{
			return client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
		}
	}
}
