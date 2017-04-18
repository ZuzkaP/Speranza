using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Speranza.Controllers;
using System.Collections.Specialized;
using System.Collections;
using Moq;

namespace Speranza.Tests.Controllers
{
    internal class FakeControllerContext : ControllerContext
    {
        
        public FakeControllerContext(Controller controller, SessionStateItemCollection sessionItems, HttpCookieCollection cookies = null)
        {
            Controller = controller;
            HttpContext = new FakeHttpContext(sessionItems,cookies);
        }

      

        internal class FakeHttpContext : HttpContextBase
        {
            private SessionStateItemCollection sessionItems;
            private HttpCookieCollection cookies;

            public Mock<HttpRequestBase> RequestMock { get; }

            public FakeHttpContext(SessionStateItemCollection sessionItems, HttpCookieCollection cookies)
            {
                this.sessionItems = sessionItems;
                RequestMock = new Mock<HttpRequestBase>();
                RequestMock.SetupGet(r => r.Cookies).Returns(cookies);
                this.cookies = cookies;
            }

            public override HttpRequestBase Request
            {
                get
                {
                    return RequestMock.Object; 
                }
            }
            public override HttpSessionStateBase Session
            {
                get
                {
                    return new FakeSessionState(sessionItems);
                }
            }
            public override HttpResponseBase Response { get { return new FakeHttpResponse(cookies); } }

            private class FakeSessionState : HttpSessionStateBase
            {
                private readonly SessionStateItemCollection sessionItems;

                
                public FakeSessionState(SessionStateItemCollection sessionItems)

                {
                    this.sessionItems = sessionItems;
                }



                public override int Count
                {
                    get
                    {
                        return this.sessionItems.Count;
                    }
                }

                public override NameObjectCollectionBase.KeysCollection Keys
                {
                    get
                    {
                        return this.sessionItems.Keys;
                    }
                }
                
                public override object this[string name]
                {
                    get
                    {
                        return this.sessionItems[name];
                    }
                    set
                    {
                        this.sessionItems[name] = value;
                    }
                }

                public override object this[int index]
                {
                    get
                    {
                        return this.sessionItems[index];
                    }
                    set
                    {
                        this.sessionItems[index] = value;
                    }

                }

                public override void Add(string name, object value)

                {
                    this.sessionItems[name] = value;
                }

                public override void Remove(string name)

                {
                    this.sessionItems.Remove(name);
                }

                
                public override IEnumerator GetEnumerator()

                {
                    return this.sessionItems.GetEnumerator();
                }

            }
        }

        internal class FakeHttpResponse : HttpResponseBase
        {
            HttpCookieCollection cookies;

            public FakeHttpResponse(HttpCookieCollection cookies)
            {
                this.cookies = cookies;
            }

            public override HttpCookieCollection Cookies { get { return cookies; } }
        }
    }

    
}
