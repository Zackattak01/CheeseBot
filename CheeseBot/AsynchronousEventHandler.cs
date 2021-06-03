using System;
using System.Threading.Tasks;

namespace CheeseBot
{
    public delegate Task AsynchronousEventHandler<in TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;
    
    public delegate Task AsynchronousEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e) where TEventArgs : EventArgs;
}