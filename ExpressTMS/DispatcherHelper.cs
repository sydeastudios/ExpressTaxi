using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Windows.Threading;

namespace ExpressTMS
{
    /// <summary>
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// </summary>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }
        /// <summary>
        /// </summary>
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;

            return null;
        }
    }
}
