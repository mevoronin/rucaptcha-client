using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace mevoronin.RuCaptchaNETClient
{
    /// <summary>
    /// Конфиг капчи
    /// </summary>
    public class CaptchaConfig
    {
        private Dictionary<string, string> parameters;

        /// <summary>
        /// Перечень установленных параметров
        /// </summary>
        public Dictionary<string, string> Parameters { get { return parameters; } }

        /// <summary>
        /// Получить список параметров
        /// </summary>
        public NameValueCollection GetParameters()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (var item in parameters)
                nvc.Add(item.Key, item.Value);
            return nvc;
        }

        public CaptchaConfig()
        {
            parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Капча имеет два слова
        /// </summary>
        public CaptchaConfig SetIsPhrase(bool value)
        {
            parameters["phrase"] = value ? "1" : "0";
            return this;
        }

        /// <summary>
        /// Капча регистр ответа имеет значения
        /// </summary>
        public CaptchaConfig SetRegisterSensitive(bool value)
        {
            parameters["regsense"] = value ? "1" : "0";
            return this;
        }

        /// <summary>
        /// Работнику нужно совершить математическое действие с капчей
        /// </summary>
        public CaptchaConfig SetNeedCalc(bool value)
        {
            parameters["calc"] = value ? "1" : "0";
            return this;
        }

        /// <summary>
        /// Из чего состоит капча
        /// </summary>
        public CaptchaConfig SetCharType(CaptchaCharTypeEnum value)
        {
            if (value == CaptchaCharTypeEnum.Default)
                parameters.Remove("numeric");
            else
                parameters["numeric"] = ((int)value).ToString();
            return this;
        }

        /// <summary>
        /// Минимальное количество знаков в ответе
        /// </summary>
        public CaptchaConfig SetMinLen(int? minLen)
        {
            if (minLen.HasValue)
            {
                if (minLen.Value < 1 || minLen > 20)
                    throw new ArgumentOutOfRangeException("minLen", minLen.Value, "Количество знаков в отчете может быть в диапазоне от 1 до 20 символов.");
                parameters["min_len"] = minLen.Value.ToString();
            }
            else
                parameters.Remove("min_len");
            return this;
        }

        /// <summary>
        /// Максимальное количество знаков в ответе
        /// </summary>
        public CaptchaConfig SetMaxLen(int? maxLen)
        {
            if (maxLen.HasValue)
            {
                if (maxLen.Value < 1 || maxLen > 20)
                    throw new ArgumentOutOfRangeException("maxLen", maxLen.Value, "Количество знаков в отчете может быть в диапазоне от 1 до 20 символов.");
                parameters["max_len"] = maxLen.Value.ToString();
            }
            else
                parameters.Remove("max_len");
            return this;
        }

        /// <summary>
        /// Язык капчи
        /// </summary>
        public CaptchaConfig SetLanguage(CaptchaLanguageEnum lang)
        {
            if (lang == CaptchaLanguageEnum.Default)
                parameters.Remove("language");
            else
                parameters["language"] = ((int)lang).ToString();
            return this;
        }

        /// <summary>
        /// ID разработчика приложения. Разработчику приложения отчисляется 10% от всех капч, пришедших из его приложения.
        /// </summary>
        public CaptchaConfig SetSoftId(string softId)
        {
            parameters["soft_id"] = softId;
            return this;
        }

    }
}
