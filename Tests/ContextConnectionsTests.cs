using UniModules.UniGame.Core.Runtime.Interfaces;

namespace UniModules.UniGame.Context.Tests 
{
    using Core.Runtime.Rx;
    using NUnit.Framework;
    using Runtime.Connections;
    using Runtime.Context;
    using UniRx;

    public class ContextConnectionsTests 
    {
        [Test]
        public void ContextBindPublishTest()
        {
            //info
            var source1 = new EntityContext();
            var source2 = new EntityContext();
            var source3 = new EntityContext();
            var result  = 300;
            var source  = 1;
            
            //action
            source1.Publish(source);
            source1.Broadcast(source2);
            source2.Broadcast(source3);
            
            source1.Publish(result);
            
            //check
            Assert.That(source3.Get<int>() == result);
        }

        [Test]
        public void ContextConnectorDisconnectTest() 
        {
            //info
            var connector   = new ContextConnection();
            var context1    = new EntityContext();
            var context2    = new EntityContext();
            var startValue  = 10;
            var secondValue = 20;
            var lastValue   = 30;
            
            //action
            context1.Publish(startValue);
            
            connector.Connect(context1);
            connector.Connect(context2);

            connector.Receive<int>().First().Subscribe(x => Assert.That(x == startValue));
            
            context2.Publish(20);
            
            connector.Receive<int>().First().Subscribe(x => Assert.That(x == secondValue));
            
            connector.Disconnect(context1);
            connector.Connect(context1);

            connector.Receive<int>().First().Subscribe(x => Assert.That(x == startValue));
        }
        
        [Test]
        public void ConnectorDisconnectTest() 
        {
            //info
            var connector   = new ContextConnection();
            var context1    = new EntityContext();
            var context2    = new EntityContext();
            var startValue  = 10;
            var secondValue = 20;

            var resultValue  = 0;
            
            //action
            var disposable = connector.Receive<int>().
                Skip(1).
                Subscribe(x => resultValue = x);
            
            context1.Publish(startValue);
            
            connector.Connect(context1);
            connector.Connect(context2);

            connector.Disconnect(context1);
            context2.Publish(secondValue);
            
            disposable.Dispose();
            
            Assert.That(secondValue == resultValue);
        }
        
        [Test]
        public void ContextConnectorDisconnectMethodTest() 
        {
            //info
            var connector        = new ContextConnection();
            var context1         = new EntityContext();
            var context2         = new EntityContext();
            var startValue       = 10;
            var secondValue      = 20;
            var lastValue        = 30;
            
            //action
            context1.Publish(startValue);
            
            var disposable1 = connector.Connect(context1);
            var disposable2 = connector.Connect(context2);

            connector.Receive<int>().First().Subscribe(x => Assert.That(x == startValue));

            context2.Publish(20);
            
            connector.Receive<int>().First().Subscribe(x => Assert.That(x == secondValue));

            disposable1.Dispose();
            connector.Connect(context1);

            connector.Receive<int>().First().Subscribe(x => Assert.That(x == startValue));
        }

        [Test]
        public void ContextConnectorValueTest()
        {
            //info
            var connector = new ContextConnection();
            var context1 = new EntityContext();
            var context2 = new EntityContext();
            
            //action

            connector.Connect(context1);
            connector.Connect(context2);

            context1.Publish(10);
            context2.Publish(20);
            
            //check
            connector.Receive<int>().Subscribe(x => Assert.That(x == 20));
        }
        
        [Test]
        public void ReceiveAfterConnectTest()
        {
            //info
            var connector   = new ContextConnection();
            var context1    = new EntityContext();
            var resultValue = 555;
            
            //action
            connector.Connect(context1);
            var value = connector.Get<int>();
            Assert.That(value == 0);
            
            context1.Publish(resultValue);
            
            value = connector.Get<int>();
            Assert.That(value == resultValue);
        }
        
        [Test]
        public void ContextReceiveTest() 
        {
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
        public void ContextReceiveFirstTest() 
        {
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
        public void ContextConnectorRelease() 
        {
            //info
            var connector = new ContextConnection();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            connector.Connect(context1);
            connector.Connect(context2);

            context1.Publish(10);
            context2.Publish(20);
            connector.Release();
            context2.Publish(30);
            
            //check
            connector.Receive<int>().Subscribe(x => Assert.That(x == 20));

            Assert.That(connector.Count == 0);
        }
        
        [Test]
        public void ContextConnectorContextRelease() 
        {
            //info
            var connector = new ContextConnection();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            connector.Connect(context1);
            connector.Connect(context2);

            context1.Publish(10);
            context2.Publish(20);
            
            context2.Release();
            
            //check
            connector.Receive<int>().Subscribe(x => Assert.That(x == 10));

            Assert.That(connector.Count == 1,$"connector.ConnectionsCount == {connector.Count}");
        }
        
        [Test]
        public void ContextConnectorPublishAndReleaseBeforeConnect() 
        {
            //info
            var connector = new ContextConnection();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            context1.Publish(10);
            context2.Publish(20);
            
            connector.Connect(context1);
            connector.Connect(context2);
            
            context2.Release();
            
            //check
            connector.Receive<int>().
                Subscribe(x => Assert.That(x == 10,$"X value {x}"));

            Assert.That(connector.Count == 1,$"connectors count {connector.Count}");
        }
        
        [Test]
        public void ContextConnectorPublishBeforeConnect() 
        {
            //info
            var connector = new ContextConnection();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action

            context1.Publish(10);
            context2.Publish(20);
            
            connector.Connect(context1);
            connector.Connect(context2);

            //check
            connector.Receive<int>().
                Subscribe(x => Assert.That(x == 20));

            Assert.That(connector.Count == 2);
        }
        
        [Test]
        public void ContextConnectorValueAfterRelease() 
        {
            //info
            var connector = new ContextConnection();
            var context1  = new EntityContext();
            var context2  = new EntityContext();
            
            //action
            context1.Publish(10);
            context2.Publish(20);
            
            connector.Connect(context1);
            connector.Connect(context2);

            //check
            var disposable = connector.
                Receive<int>().
                Subscribe(x => Assert.That(x == 20,$"Connector value is {x}"));
            Assert.That(connector.Count == 2);
            
            disposable.Dispose();
            context2.Release();
            
            connector.Receive<int>().Subscribe(x => Assert.That(x == 10));
            Assert.That(connector.Count == 1);
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
