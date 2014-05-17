using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mevoronin.RuCaptchaNETClient
{
    /// <summary>
    /// Из чего состоит капча
    /// </summary>
    public enum CaptchaCharTypeEnum
    {
        /// <summary>
        /// По-умолчанию
        /// </summary>
        Default = 0,
        /// <summary>
        /// Капча состоит только из цифр
        /// </summary>
        OnlyDigits = 1,
        /// <summary>
        /// капча состоит только из букв
        /// </summary>
        OnlyLetter = 2,
        /// <summary>
        /// Капча состоит либо только из цифр, либо только из букв
        /// </summary>
        OnlyDigitsOrOnlyLetter = 3
    }
}
