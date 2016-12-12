using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

namespace UITestSampleApp
{
	public interface IAuthenticate
	{
		Task<MobileServiceUser> Authenticate(MobileServiceClient client);
	}
}
