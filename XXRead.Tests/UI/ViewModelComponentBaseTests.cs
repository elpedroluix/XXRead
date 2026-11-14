using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using XXRead.UI.Components;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class ViewModelComponentBaseTests
	{
		[TestMethod]
		public void Render_ShowsViewModelValue()
		{
			using var context = new BunitContext();
			var viewModel = new FakeViewModel() { Value = "first" };
			context.Services.AddSingleton(viewModel);

			var component = context.Render<FakeComponent>();

			Assert.AreEqual("first", component.Markup);
		}

		[TestMethod]
		public void PropertyChanged_RendersAgain()
		{
			using var context = new BunitContext();
			var viewModel = new FakeViewModel() { Value = "first" };
			context.Services.AddSingleton(viewModel);
			var component = context.Render<FakeComponent>();

			viewModel.Value = "second";

			component.WaitForAssertion(() => Assert.AreEqual("second", component.Markup));
		}

		[TestMethod]
		public void PropertyChanged_FromAnotherThread_RendersAgain()
		{
			using var context = new BunitContext();
			var viewModel = new FakeViewModel() { Value = "first" };
			context.Services.AddSingleton(viewModel);
			var component = context.Render<FakeComponent>();

			Task.Run(() => viewModel.Value = "second").Wait();

			component.WaitForAssertion(() => Assert.AreEqual("second", component.Markup));
		}

		[TestMethod]
		public void Dispose_UnsubscribesFromViewModel()
		{
			using var context = new BunitContext();
			var viewModel = new FakeViewModel();
			context.Services.AddSingleton(viewModel);
			var component = context.Render<FakeComponent>();
			Assert.AreEqual(1, viewModel.SubscriberCount);

			component.Instance.Dispose();

			Assert.AreEqual(0, viewModel.SubscriberCount);
		}

		public class FakeViewModel : INotifyPropertyChanged
		{
			private PropertyChangedEventHandler _propertyChanged;
			private string _value;

			public event PropertyChangedEventHandler PropertyChanged
			{
				add => _propertyChanged += value;
				remove => _propertyChanged -= value;
			}

			public int SubscriberCount => _propertyChanged?.GetInvocationList().Length ?? 0;

			public string Value
			{
				get => _value;
				set
				{
					_value = value;
					OnPropertyChanged();
				}
			}

			private void OnPropertyChanged([CallerMemberName] string propertyName = "")
			{
				_propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public class FakeComponent : ViewModelComponentBase<FakeViewModel>
		{
			protected override void BuildRenderTree(RenderTreeBuilder builder)
			{
				builder.AddContent(0, ViewModel.Value);
			}
		}
	}
}
