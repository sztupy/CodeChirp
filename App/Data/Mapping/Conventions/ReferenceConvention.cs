using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace  CodeChirp.Data.Mapping.Conventions {
    public class ReferenceConvention : IReferenceConvention {
        public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance)
        {
            instance.Column(instance.Property.Name + "Fk");
        }
    }
}
