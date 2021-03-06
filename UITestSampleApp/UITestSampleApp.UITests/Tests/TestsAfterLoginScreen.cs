﻿using NUnit.Framework;

using Xamarin.UITest;

namespace UITestSampleApp.UITests
{
	[Category("TestsAfterLoginScreen")]
	public class TestsAfterLoginScreen : BaseTest
	{
		public TestsAfterLoginScreen(Platform platform) : base(platform)
		{
			this.platform = platform;
		}

		public override void BeforeEachTest()
		{
			base.BeforeEachTest();

			LoginPage.WaitForLoginScreen();

			BackdoorHelpers.BypassLoginScreen(app);

			app.WaitForElement("First Page");
		}

		[TestCase(true)]
		[TestCase(false)]
		[Test]
		public void EnterText(bool shouldUseKeyboardReturnButton)
		{
			//Arrange
			var textInput = "Hello World";

			//Act
			switch (shouldUseKeyboardReturnButton)
			{
				case true:
					FirstPage.EnterTextAndPressEnter(textInput);
					break;
				case false:
					FirstPage.EnterText(textInput);
					FirstPage.ClickGo();
					break;
			}

			FirstPage.WaitForNoActivityIndicator();

			//Assert
			Assert.AreEqual(FirstPage.GetEntryFieldText(), textInput);
		}

		[Test]
		public void SelectItemOnListView()
		{
			//Arrange
			var listItemNumber = 9;
			var expectedAlertString = $"You Selected Number {listItemNumber}";

			//Act
			BackdoorHelpers.OpenListViewPage(app);

			ListPage.WaitForNoActivityIndicator();
			ListPage.TapListItemNumber(listItemNumber);

			//Assert
			Assert.AreEqual(expectedAlertString, ListPage.GetAlertText(listItemNumber));
			Assert.IsTrue(BackdoorHelpers.GetListPageData(app).Count > 30);
		}
	}
}

