﻿namespace UniModules.UniGame.Context.Runtime.Connections 
{
    using System;
    using Context;
    using Core.Runtime.Interfaces;
    using Core.Runtime.Rx;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class UniqueConnector<TData> : 
        TypeDataConnector<IObservable<TData>> 
    {

    }
}