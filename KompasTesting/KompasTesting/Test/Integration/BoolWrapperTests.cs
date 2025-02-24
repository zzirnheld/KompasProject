namespace Kompas.Test.Integration;

public class BoolWrapperTests
{

	private class BoolWrapper(Func<bool> fallback)
	{
		public bool? overrideBool = null;

		private readonly Func<bool> fallback = fallback;

		public bool Value => overrideBool ?? fallback();
	}

	private class WrapperWrapper(Func<bool> input)
	{
		private readonly Func<bool> input = input;

		public bool Value => input();
	}


	[Fact]
	public void BoolWrapper_WhenOverrideChanged_ChangesValueOfValue()
	{
		var wrapper = new BoolWrapper(() => false);
		var wrapperWrapper = new WrapperWrapper(() => wrapper.Value);

		Assert.False(wrapperWrapper.Value);

		wrapper.overrideBool = true;
		Assert.True(wrapperWrapper.Value);

		wrapper.overrideBool = null;
		Assert.False(wrapperWrapper.Value);
	}
}