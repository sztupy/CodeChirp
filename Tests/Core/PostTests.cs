using NUnit.Framework;
using CodeChirp.Core;
using Shaml.Testing;
using Shaml.Testing.NUnit;

namespace Tests.Blog.Core
{
	[TestFixture]
    public class PostTests
    {
        [Test]
        public void CanComparePosts() {
            Post instance = new Post();
            EntityIdSetter.SetIdOf<int>(instance, 1);

            Post instanceToCompareTo = new Post();
            EntityIdSetter.SetIdOf<int>(instanceToCompareTo, 1);

            instance.ShouldEqual(instanceToCompareTo);
        }
    }
}
