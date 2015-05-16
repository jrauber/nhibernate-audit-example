using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Type;

namespace nHibernate4.Mapping
{
    public class NoRefIntModelMapper : ConventionModelMapper
    {
        public NoRefIntModelMapper()
        {
            this.BeforeMapManyToOne += (inspector, member, customizer) =>
            {
                customizer.ForeignKey("none");
            };

            this.BeforeMapSet += (inspector, member, customizer) =>
            {
                customizer.Key(k => k.ForeignKey("none"));
            };

            this.BeforeMapBag += (inspector, member, customizer) =>
            {
                customizer.Key(k => k.ForeignKey("none"));
            };

            this.BeforeMapList += (inspector, member, customizer) =>
            {
                customizer.Key(k => k.ForeignKey("none"));
            };

            this.BeforeMapIdBag += (inspector, member, customizer) =>
            {
                customizer.Key(k => k.ForeignKey("none"));
            };
        }
    }
}
