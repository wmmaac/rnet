﻿namespace Cogito.Application.Lifecycle
{

    /// <summary>
    /// To be invoked just after the application starts.
    /// </summary>
    public interface IOnAfterStart<T>
        where T : class
    {

        void OnAfterStart();

    }

}
