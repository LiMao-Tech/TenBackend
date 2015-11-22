using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
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
            
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedEncodings.Add(Encoding.UTF8);
            // Debug.WriteLine("Formatter Constructed");
            // MediaTypeMappings.Add(new TenMsgMediaMapping());
        }

        public TenMsgMediaFormatter(string controllerArg)
            : this() {
            controllerName = controllerArg;
        }

        // Serialization Support
        public override bool CanReadType(Type type) {
            return type == typeof(TenMsg);
        }

        public override bool CanWriteType(Type type) {
            return false;
            // return type == typeof(TenMsg);
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream stream,
                                                        HttpContent httpContent,
                                                        IFormatterLogger iFormatterLogger)
        {
            MultipartStreamProvider parts = await httpContent.ReadAsMultipartAsync();
            IEnumerable<HttpContent> contents = parts.Contents;

            HttpContent content = contents.FirstOrDefault();
            foreach (HttpContent c in contents ) {
                if (SupportedMediaTypes.Contains(c.Headers.ContentType)) {
                    content = c;
                    break;
                }
            }

            using (var msgStream = await content.ReadAsStreamAsync())
            {
                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(TenMsg));
                TenMsg msg = (TenMsg)js.ReadObject(msgStream);
                Debug.WriteLine("msgString: " + msgStream.ToString());

                int sender = msg.Sender;
                int receiver = msg.Receiver;
                byte phoneType = msg.PhoneType;
                bool isLocked = msg.IsLocked;
                DateTime msgTime = msg.MsgTime;
                string msgContent = msg.MsgContent;
                Debug.WriteLine("Msg Content: " + msg.MsgContent);
                
                return new TenMsg(sender, receiver, phoneType, isLocked, msgTime, msgContent);
            }
        }
    }
}