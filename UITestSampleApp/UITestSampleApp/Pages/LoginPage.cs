using System;
using System.Collections.Generic;

using Xamarin.Forms;

using MyLoginUI.Pages;

using UITestSampleApp.Shared;

namespace UITestSampleApp
{
	public class LoginPage : ReusableLoginPage
	{
		#region Fields
		bool isInitialized = false;
		#endregion

		#region Constructors
		public LoginPage()
		{
			AutomationId = "loginPage";

#if DEBUG
			var crashButton = new Button
			{
				Text = "x",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				AutomationId = AutomationIdConstants.CrashButton
			};
			crashButton.Clicked += (s, e) =>
			{
				throw new Exception("Crash Button Tapped");
			};

			MainLayout.Children.Add(crashButton,
				Constraint.RelativeToParent((parent) => parent.X),
				Constraint.RelativeToParent((parent) => parent.Y)
			);

			var crashButton2 = new Button
			{
				Text = "a",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				AutomationId = AutomationIdConstants.CrashButton
			};
			crashButton2.Clicked += (s, e) =>
			{
				// create a collection container to hold exceptions
				List<Exception> exceptions = new List<Exception>();

				// do some stuff here ........

				// we have an exception with an innerexception, so add it to the list
				exceptions.Add(new TimeoutException("This is part 1 of aggregate exception", new ArgumentException("ID missing")));

				// do more stuff .....

				// Another exception, add to list
				exceptions.Add(new NotImplementedException("This is part 2 of aggregate exception"));

				// all done, now create the AggregateException and throw it
				AggregateException aggEx = new AggregateException(exceptions);
				//throw aggEx;
				throw new Exception("This is a Warning.", aggEx);
			};

			MainLayout.Children.Add(crashButton2,
			                        Constraint.RelativeToParent((parent) => parent.X + crashButton.Width + 3),
				Constraint.RelativeToParent((parent) => parent.Y)
			);
#endif
		}
		#endregion

		#region Properties
		public bool TouchIdSuccess
		{
			set
			{
				if (value)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						Navigation.PopAsync();
					});
				}
				else {
					DependencyService.Get<ILogin>().AuthenticateWithTouchId(this);
				}
			}
		}
		#endregion

		#region Methods
		public override async void Login(string userName, string passWord, bool saveUserName)
		{
			base.Login(userName, passWord, saveUserName);
			AnalyticsHelpers.TrackEvent(AnalyticsConstants.LoginAttempt);

			var success = await DependencyService.Get<ILogin>().CheckLogin(userName, passWord);
			if (success)
			{
				var insightsDict = new Dictionary<string, string> {
					{ "User Type", "NonApprover" },
					{ "Uses TouchId", "Yes" },
				};

				if (saveUserName)
				{
					await DependencyService.Get<ILogin>().SaveUsername(userName);
					insightsDict.Add("Saves username", "Yes");
				}
				else {
					insightsDict.Add("Saves username", "No");
				}

				App.IsLoggedIn = true;

				if (Device.OS == TargetPlatform.iOS)
					await Navigation.PopAsync();
				else {
					await Navigation.PushAsync(new FirstPage(), false);
					Navigation.RemovePage(this);
				}
			}
			else {
				var signUp = await DisplayAlert("Invalid Login", "Sorry, we didn't recoginize the username or password. Feel free to sign up for free if you haven't!", "Sign up", "Try again");

				if (signUp)
				{
					await Navigation.PushModalAsync(new NewUserSignUpPage());

					AnalyticsHelpers.TrackEvent("NewUserSignUp", new Dictionary<string, string> {
						{ "ActionPoint", "System Prompt" },
					});
				}
			}
		}

		public override void NewUserSignUp()
		{
			base.NewUserSignUp();
			Navigation.PushModalAsync(new NewUserSignUpPage());
		}

		public override void RunAfterAnimation()
		{
			base.RunAfterAnimation();

			if (App.UserName != null)
				SetUsernameEntry(App.UserName);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			AnalyticsHelpers.TrackEvent(AnalyticsConstants.LoginPageOnAppeared);

			//Need bug fixed on Material Design for PopToRootAsync() 
			//https://bugzilla.xamarin.com/show_bug.cgi?id=36907
			if (!isInitialized)
			{
				if (Device.OS == TargetPlatform.iOS)
					Navigation.InsertPageBefore(new FirstPage(), this);

				isInitialized = true;
			}
		}
		#endregion
	}
}