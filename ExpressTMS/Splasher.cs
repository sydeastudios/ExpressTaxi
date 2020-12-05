using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

namespace ExpressTMS
{
    /// <summary>
    /// </summary>
    public static class Splasher
    {
        /// <summary>
        /// </summary>
        private static Window mSplash;

        /// <summary>
        /// </summary>
        public static Window Splash
        {
            get{return mSplash;}
            set{mSplash = value;}
        }

        /// <summary>
        /// </summary>
        public static void ShowSplash()
        {
            if (mSplash != null)
            {
                mSplash.Show();
                for (int i = 0; i < 250; i++)
                {
                    Thread.Sleep(1);
                }
                mSplash.Close();
            }
        }
        /// <summary>
        /// </summary>
        public static void CloseSplash()
        {
            if ( mSplash != null )            
            {                    
                mSplash.Close ( );                    
                mSplash.Dispatcher.InvokeShutdown();                
                if ( mSplash is IDisposable )                    
                    ( mSplash as IDisposable ).Dispose ( );            
            }        
        }
    }
}
