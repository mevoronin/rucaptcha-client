using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mevoronin.RuCaptchaNETClient
{
    /// <summary>
    /// Исключение генерируемое при неправильном ответе сервера
    /// </summary>
    public class RuCaptchaException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RuCaptchaException()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public RuCaptchaException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public RuCaptchaException(string message, Exception inner)
            : base(message, inner)
        {

        }

    }
}
