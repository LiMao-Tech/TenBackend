using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TenBackend.Models;

namespace TenBackend.Infrastructure
{
    public class TenMsgMediaFormatter : MediaTypeFormatter
    {
        private string controllerName;

        public TenMsgMediaFormatter() {
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x.product"));
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedEncodings.Add(Encoding.UTF8);
            MediaTypeMappings.Add(new TenMsgMediaMapping());
        }

        public TenMsgMediaFormatter(string controllerArg)
            : this() {
            controllerName = controllerArg;
        }

        public override bool CanReadType(Type type) {
            return false;
        }

        public override bool CanWriteType(Type type) {
            return type == typeof(TenMsg) || type == typeof(IEnumerable<TenMsg>);
        }

        public override void SetDefaultContentHeaders(Type type,
                HttpContentHeaders headers, MediaTypeHeaderValue mediaType) {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.Add("X-ModelType",
                type == typeof(IEnumerable<TenMsg>)
                    ? "IEnumerable<TenMsg>" : "TenMsg");
            headers.Add("X-MediaType", mediaType.MediaType);
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type,
                HttpRequestMessage request, MediaTypeHeaderValue mediaType) {
                    return new TenMsgMediaFormatter(
                request.GetRouteData().Values["controller"].ToString());
        }

        public override async Task WriteToStreamAsync(Type type, object value,
                Stream writeStream, HttpContent content,
                TransportContext transportContext) {

            List<string> productStrings = new List<string>();
            IEnumerable<TenMsg> msgs = value is TenMsg ? new TenMsg[] { (TenMsg)value } : (IEnumerable<TenMsg>)value;

            foreach (TenMsg msg in msgs)
            {
                productStrings.Add(string.Format("{0},{1},{2}",
                    product.ProductID,
                    controllerName == null ? product.Name :
                        string.Format("{0} ({1})", product.Name, controllerName),
                    product.Price));
            }

            Encoding enc = SelectCharacterEncoding(content.Headers);
            StreamWriter writer = new StreamWriter(writeStream, enc ?? Encoding.Unicode);
            await writer.WriteAsync(string.Join(",", productStrings));
            writer.Flush();
        }
    }
}