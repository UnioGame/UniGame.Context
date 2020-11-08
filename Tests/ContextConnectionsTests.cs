namespace UniModules.UniGame.Context.Tests {
    using Core.Runtime.Rx;
    using NUnit.Framework;
    using Runtime.Connections;
    using Runtime.Context;
    using UniRx;

    public class ContextConnectionsTests 
    {

        [Test]
        public void ContextConnectorValueTest() {
            
            //info
            var connector = new ContextConnector();
            var context1 = new EntityContext();
            var context2 = new EntityContext();
            
            //action

            connector.Bind(context1);
            connector.Bind(context2);

            context1.Publish(10);
            context2.Publish(20);
            
            //check
            connector.Receive<int>().Subscribe(x => Assert.That(x == 20));
        }
        
        [Test]
        public void ContextReceiveTest() {
            
            //info
            var context1  = new EntityContext();
            var testValue = 100;
            //action

            context1.Publish(testValue);
            
            //check
            context1.Receive<int>().
                First().
                Subscribe(x => Assert.That(x == testValue));
            context1.Release();
        }
        
        [Test]
        public void ContextReceiveFirstTest() {
            
            //info
            var context1       = new EntityContext();
            var textValueFirst = 200;
            var resultValue    = 0;
            
            //action
            context1.Receive<int>().
                First().
                Subscribe(x => resultValue = x);
            
            context1.Publish(textValueFirst);
            context1.Release();
            
            //check
            Assert.That(resultValue == textValueFirst);
        }
        
        [Test]
        public void ContextConnectorRelease() {
            
            //info
            var connector = new ContextConnector();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            connector.Bind(context1);
            connector.Bind(context2);

            context1.Publish(10);
            context2.Publish(20);
            connector.Release();
            context2.Publish(30);
            
            //check
            connector.Receive<int>().Subscribe(x => Assert.That(x == 20));

            Assert.That(connector.ConnectionsCount == 0);
        }
        
        [Test]
        public void ContextConnectorContextRelease() {
            
            //info
            var connector = new ContextConnector();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            connector.Bind(context1);
            connector.Bind(context2);

            context1.Publish(10);
            context2.Publish(20);
            
            context2.Release();
            
            //check
            connector.Receive<int>().Subscribe(x => Assert.That(x == 10));

            Assert.That(connector.ConnectionsCount == 1,$"connector.ConnectionsCount == {connector.ConnectionsCount}");
        }
        
        [Test]
        public void ContextConnectorPublishAndReleaseBeforeConnect() {
            
            //info
            var connector = new ContextConnector();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            context1.Publish(10);
            context2.Publish(20);
            
            connector.Bind(context1);
            connector.Bind(context2);
            
            context2.Release();
            
            //check
            connector.Receive<int>().
                Subscribe(x => Assert.That(x == 10,$"X value {x}"));

            Assert.That(connector.ConnectionsCount == 1,$"connectors count {connector.ConnectionsCount}");
        }
        
        [Test]
        public void ContextConnectorPublishBeforeConnect() {
            
            //info
            var connector = new ContextConnector();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            context1.Publish(10);
            context2.Publish(20);
            
            connector.Bind(context1);
            connector.Bind(context2);

            //check
            connector.Receive<int>().
                Subscribe(x => Assert.That(x == 20));

            Assert.That(connector.ConnectionsCount == 2);
        }
        
        [Test]
        public void ContextConnectorValueAfterRelease() {
            
            //info
            var connector = new ContextConnector();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action
            context1.Publish(10);
            context2.Publish(20);
            
            connector.Bind(context1);
            connector.Bind(context2);

            //check
            var disposable = connector.
                Receive<int>().
                Subscribe(x => Assert.That(x == 20,$"Connector value is {x}"));
            Assert.That(connector.ConnectionsCount == 2);
            
            disposable.Dispose();
            context2.Release();
            
            connector.Receive<int>().Subscribe(x => Assert.That(x == 10));
            Assert.That(connector.ConnectionsCount == 1);
        }

        [Test]
        public void ReactivePropertyDisposeTest()
        {
            //info
            var value = new RecycleReactiveProperty<int>();
            
            //action
            var disposable = value.Subscribe(x => Assert.That(x == int.MaxValue));
            disposable.Dispose();
            
            value.Value = 0;
            Assert.True(true);
        }
        
    }
}
