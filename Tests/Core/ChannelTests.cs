using NUnit.Framework;
using CodeChirp.Core;
using Shaml.Testing;
using Shaml.Testing.NUnit;

namespace Tests.Blog.Core
{
	[TestFixture]
    public class ChannelTests
    {
        [Test]
        public void CanCompareChannels() {
            Channel instance = new Channel();
            EntityIdSetter.SetIdOf<int>(instance, 1);

            Channel instanceToCompareTo = new Channel();
            EntityIdSetter.SetIdOf<int>(instanceToCompareTo, 1);

            instance.ShouldEqual(instanceToCompareTo);
        }
    }
}
