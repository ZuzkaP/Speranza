using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Speranza.Controllers;
using System.Collections.Specialized;
using System.Collections;

namespace Speranza.Tests.Controllers
{
    internal class FakeControllerContext : ControllerContext
    {
        
        public FakeControllerContext(AccountsController controller, SessionStateItemCollection sessionItems)
        {
            Controller = controller;
            HttpContext = new FakeHttpContext(sessionItems);
           
        }


        private class FakeHttpContext : HttpContextBase
        {
            private SessionStateItemCollection sessionItems;

            public FakeHttpContext(SessionStateItemCollection sessionItems)
            {
                this.sessionItems = sessionItems;       
            }


            public override HttpSessionStateBase Session
            {
                get
                {
                    return new FakeSessionState(sessionItems);
                }
            }


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
    }
}
