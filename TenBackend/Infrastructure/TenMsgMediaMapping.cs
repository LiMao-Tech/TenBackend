using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;

namespace TenBackend.Infrastructure
{
    public class TenMsgMediaMapping : MediaTypeMapping
    {
        public TenMsgMediaMapping() : base("application/x.tenmsg")
        {

        }

        public override double TryMatchMediaType(HttpRequestMessage request)
        {
            IEnumerable<string> values;

            return request.Headers.TryGetValues("X-UseTenMsgFormat", out values)
                && values.Where(x => x == "true").Count() > 0 ? 1 : 0;
        }
    }
}