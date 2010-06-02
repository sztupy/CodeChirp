using NUnit.Framework;
using CodeChirp.Core;
using Shaml.Testing;
using Shaml.Testing.NUnit;

namespace Tests.Blog.Core
{
	[TestFixture]
    public class TagTests
    {
        [Test]
        public void CanCompareTags() {
            Tag instance = new Tag();
            EntityIdSetter.SetIdOf<int>(instance, 1);

            Tag instanceToCompareTo = new Tag();
            EntityIdSetter.SetIdOf<int>(instanceToCompareTo, 1);

            instance.ShouldEqual(instanceToCompareTo);
        }
    }
}
