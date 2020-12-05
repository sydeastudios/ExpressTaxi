using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace ExpressTMS
{
    /// <summary>
    /// </summary>
    public class MessageListener : DependencyObject
    {
        /// <summary>
        /// </summary>
        private static MessageListener mInstance;

        /// <summary>
        /// </summary>
        private MessageListener()
        {

        }

        /// <summary>
        /// </summary>
        public static MessageListener Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new MessageListener();
                return mInstance;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage(string message)
        {
            Message = message;
            Debug.WriteLine(Message);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageListener), new UIPropertyMetadata(null));

    }
}
