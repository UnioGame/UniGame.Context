using UnityEngine;

namespace UniModules.UniGame.Context.Tests {
    using NUnit.Framework;
    using Runtime.Connections;
    using Runtime.Context;
    using UniContextData.Runtime.Entities;
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
            connector.Receive<int>().Subscribe(x => Assert.That(x == 20));
            Assert.That(connector.ConnectionsCount == 2);
            context2.Release();
            
            connector.Receive<int>().Subscribe(x => Assert.That(x == 10));
            Assert.That(connector.ConnectionsCount == 1);
        }
        
    }
}
