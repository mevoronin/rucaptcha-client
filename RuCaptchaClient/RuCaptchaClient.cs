using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace mevoronin.RuCaptchaNETClient
{
    /// <summary>
    /// Клиент сервиса RuCaptcha
    /// </summary>
    public class RuCaptchaClient
    {
        private readonly string api_key;
        const string host = "http://rucaptcha.com";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="api_key">Ключ доступа к API</param>
        public RuCaptchaClient(string api_key)
        {
            this.api_key = api_key;
        }

        /// <summary>
        /// Получить расшифрованное значение капчи
        /// </summary>
        /// <param name="captchaId">Id капчи</param>
        /// <returns></returns>
        public string GetCaptcha(string captchaId)
        {
            string url = string.Format("{0}/res.php?key={1}&action=get&id={2}", host, api_key, captchaId);
            return GetAnswer(url);
        }

        public string UploadCaptchaFile(string fileName)
        {
            string url = string.Format("{0}/in.php", host);
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("key", api_key);

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream requestStream = request.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                requestStream.Write(formitembytes, 0, formitembytes.Length);
            }
            requestStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "file", fileName, "image/jpeg");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            requestStream.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(trailer, 0, trailer.Length);
            requestStream.Close();

            using(WebResponse response = request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                return responseReader.ReadToEnd();
            }

        }

        /// <summary>
        /// Получить текущий баланс аккаунта
        /// </summary>
        /// <returns></returns>
        public decimal GetBalance()
        {
            string url = string.Format("{0}/res.php?key={1}&action=getbalance", host, api_key);
            string string_balance = GetAnswer(url);
            decimal balance = decimal.Parse(string_balance, CultureInfo.InvariantCulture.NumberFormat);
            return balance;
        }
        /// <summary>
        /// Вернуть описание ответа сервера
        /// </summary>
        /// <param name="serviceAnswer">Ответ сервера</param>
        private string GetErrorMessage(string serviceAnswer)
        {
            string answer = null;
            switch (answer)
            {
                case "CAPCHA_NOT_READY":
                    answer = "Капча в работе, ещё не расшифрована, необходимо повтороить запрос через несколько секунд.";
                    break;
                case "ERROR_WRONG_ID_FORMAT":
                    answer = "Неверный формат ID капчи. ID должен содержать только цифры.";
                    break;
                case "ERROR_WRONG_CAPTCHA_ID":
                    answer = "Неверное значение ID капчи.";
                    break;
                case "ERROR_CAPTCHA_UNSOLVABLE":
                    answer = "Капчу не смогли разгадать 3 разных работника. Средства за эту капчу не списываются.";
                    break;
                case "ERROR_WRONG_USER_KEY":
                    answer = "Не верный формат параметра key, должно быть 32 символа.";
                    break;
                case "ERROR_KEY_DOES_NOT_EXIST":
                    answer = "Использован несуществующий key.";
                    break;
                case "ERROR_ZERO_BALANCE":
                    answer = "Баланс Вашего аккаунта нулевой.";
                    break;
                case "ERROR_NO_SLOT_AVAILABLE":
                    answer = "Текущая ставка распознования выше, чем максимально установленная в настройках Вашего аккаунта.";
                    break;
                case "ERROR_ZERO_CAPTCHA_FILESIZE":
                    answer = "Размер капчи меньше 100 Байт.";
                    break;
                case "ERROR_TOO_BIG_CAPTCHA_FILESIZE":
                    answer = "Размер капчи более 100 КБайт.";
                    break;
                case "ERROR_WRONG_FILE_EXTENSION":
                    answer = "Ваша капча имеет неверное расширение, допустимые расширения jpg,jpeg,gif,png.";
                    break;
                case "ERROR_IMAGE_TYPE_NOT_SUPPORTED":
                    answer = "Сервер не может определить тип файла капчи.";
                    break;
                case "ERROR_IP_NOT_ALLOWED":
                    answer = "В Вашем аккаунте настроено ограничения по IP с которых можно делать запросы. И IP, с которого пришёл данный запрос не входит в список разрешённых.";
                    break;
                default:
                    answer = "Неопознанный ответ сервера.";
                    break;
            }
            return string.Format("{0} ({1})", answer, serviceAnswer);
        }

        private string GetAnswer(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            string serviceAnswer = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                serviceAnswer = reader.ReadToEnd();
            }

            if (serviceAnswer.StartsWith("OK|"))
                return serviceAnswer.Substring(3);
            else
                throw new RuCaptchaException(GetErrorMessage(serviceAnswer));
        }
    }
}
