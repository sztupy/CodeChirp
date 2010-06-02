using NUnit.Framework;
using CodeChirp.Core;
using Shaml.Testing;
using Shaml.Testing.NUnit;

namespace Tests.Blog.Core
{
	[TestFixture]
    public class SoulTests
    {
        [Test]
        public void CanCompareSouls() {
            Soul instance = new Soul();
            EntityIdSetter.SetIdOf<int>(instance, 1);

            Soul instanceToCompareTo = new Soul();
            EntityIdSetter.SetIdOf<int>(instanceToCompareTo, 1);

            instance.ShouldEqual(instanceToCompareTo);
        }
    }
}
