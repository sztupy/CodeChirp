using NUnit.Framework;
using CodeChirp.Core;
using Shaml.Testing;
using Shaml.Testing.NUnit;

namespace Tests.Blog.Core
{
	[TestFixture]
    public class BadgeTests
    {
        [Test]
        public void CanCompareBadges() {
            Badge instance = new Badge();
            EntityIdSetter.SetIdOf<int>(instance, 1);

            Badge instanceToCompareTo = new Badge();
            EntityIdSetter.SetIdOf<int>(instanceToCompareTo, 1);

            instance.ShouldEqual(instanceToCompareTo);
        }
    }
}
