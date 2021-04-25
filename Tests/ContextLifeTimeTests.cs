using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UniModules.UniGame.Context.Runtime.Context;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UnityEngine;

namespace UniModules.UniGame.Context.Tests 
{

    public class ContextLifeTimeTests
    {
        [Test]
        public void LifeTimePublishingTest()
        {
            //info
            var contextContainer = new EntityContext();
            var childContext1 = new EntityContext();
            var childContext2 = new EntityContext();

            //action
            contextContainer.Publish<IContext>(childContext1);
            contextContainer.Publish<IContext>(childContext2);

            childContext1.Release();

            //check
            var value = contextContainer.Get<IContext>();
            Assert.That(Equals(value, childContext2));
        }
        
        [Test]
        public void LifeTimeContextTerminateTest()
        {
            //info
            var contextContainer = new EntityContext();
            var childContext1 = new EntityContext();

            //action
            contextContainer.Publish<IContext>(childContext1);
            childContext1.Release();

            //check
            var value = contextContainer.Get<IContext>();
            Assert.That(value == null);
        }
        
        [Test]
        public void LifeTimeContextTerminateAfterReleaseTest()
        {
            //info
            var contextContainer = new EntityContext();
            var childContext1 = new EntityContext();

            //action
            contextContainer.Publish<IContext>(childContext1);
            contextContainer.Release();
            childContext1.Release();

            //check
            var value = contextContainer.Get<IContext>();
            Assert.That(value == null);
        }

    }
}
