using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace CodeChirp.Data.Mapping.Conventions {
    public class HasManyToManyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance) {   
            instance.Cascade.SaveUpdate();
        }
    }
}
