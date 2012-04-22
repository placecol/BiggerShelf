using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kaiser.BiggerShelf.Web.Infrastructure.Raven
{
    public  class RavenId
    {
        private readonly string prefix;
        private readonly string hilo;

        public RavenId(string prefix, string hilo)
        {
            this.hilo = hilo;
            this.prefix = prefix;
        }

        public string HiLo
        {
            get { return hilo; }
        }

        public string Prefix
        {
            get { return prefix; }
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Prefix, HiLo);
        }
    }

    public class RavenId<TEntity> : RavenId where TEntity : class, new()
    {
        public RavenId(string prefix, string hilo) : base(prefix, hilo)
        {
        }
    }
}