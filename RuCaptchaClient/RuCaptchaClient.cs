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
        static Dictionary<string, string> errors;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="api_key">Ключ доступа к API</param>
        public RuCaptchaClient(string api_key)
        {
            this.api_key = api_key;
            if (errors == null)
            {
                errors = new Dictionary<string, string>();
                errors.Add("CAPCHA_NOT_READY", "Капча в работе, ещё не расшифрована, необходимо повтороить запрос через несколько секунд.");
                errors.Add("ERROR_WRONG_ID_FORMAT", "Неверный формат ID капчи. ID должен содержать только цифры.");
                errors.Add("ERROR_WRONG_CAPTCHA_ID", "Неверное значение ID капчи.");
                errors.Add("ERROR_CAPTCHA_UNSOLVABLE", "Капчу не смогли разгадать 3 разных работника. Средства за эту капчу не списываются.");
                errors.Add("ERROR_WRONG_USER_KEY", "Не верный формат параметра key, должно быть 32 символа.");
                errors.Add("ERROR_KEY_DOES_NOT_EXIST", "Использован несуществующий key.");
                errors.Add("ERROR_ZERO_BALANCE", "Баланс Вашего аккаунта нулевой.");
                errors.Add("ERROR_NO_SLOT_AVAILABLE", "Текущая ставка распознования выше, чем максимально установленная в настройках Вашего аккаунта.");
                errors.Add("ERROR_ZERO_CAPTCHA_FILESIZE", "Размер капчи меньше 100 Байт.");
                errors.Add("ERROR_TOO_BIG_CAPTCHA_FILESIZE", "Размер капчи более 100 КБайт.");
                errors.Add("ERROR_WRONG_FILE_EXTENSION", "Ваша капча имеет неверное расширение, допустимые расширения jpg,jpeg,gif,png.");
                errors.Add("ERROR_IMAGE_TYPE_NOT_SUPPORTED", "Сервер не может определить тип файла капчи.");
                errors.Add("ERROR_IP_NOT_ALLOWED", "В Вашем аккаунте настроено ограничения по IP с которых можно делать запросы. И IP, с которого пришёл данный запрос не входит в список разрешённых.");
            }
        }

        /// <summary>
        /// Получить расшифрованное значение капчи
        /// </summary>
        /// <param name="captchaId">Id капчи</param>
        /// <returns></returns>
        public string GetCaptcha(string captchaId)
        {
            string url = string.Format("{0}/res.php?key={1}&action=get&id={2}", host, api_key, captchaId);
            return MakeGetRequest(url);
        }

        /// <summary>
        /// Загрузить файл капчи
        /// </summary>
        /// <param name="fileName">уть к файлу с капчей</param>
        /// <returns></returns>
        public string UploadCaptchaFile(string fileName)
        {
            return UploadCaptchaFile(fileName, null);
        }
        /// <summary>
        /// Загрузить файл капчи
        /// </summary>
        /// <param name="fileName">уть к файлу с капчей</param>
        /// <param name="config">Параметры</param>
        /// <returns></returns>
        public string UploadCaptchaFile(string fileName, CaptchaConfig config)
        {
            string url = string.Format("{0}/in.php", host);
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("key", api_key);
            if (config != null)
                nvc.Add(config.GetParameters());

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

            using (WebResponse response = request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                return ParseAnswer(responseReader.ReadToEnd());
            }

        }

        /// <summary>
        /// Получить текущий баланс аккаунта
        /// </summary>
        /// <returns></returns>
        public decimal GetBalance()
        {
            string url = string.Format("{0}/res.php?key={1}&action=getbalance", host, api_key);
            string string_balance = MakeGetRequest(url);
            decimal balance = decimal.Parse(string_balance, CultureInfo.InvariantCulture.NumberFormat);
            return balance;
        }

        /// <summary>
        /// Выполнение Get запроса по указанному URL
        /// </summary>
        private string MakeGetRequest(string url)
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
            return ParseAnswer(serviceAnswer);
        }

        /// <summary>
        /// Разбор ответа
        /// </summary>
        private string ParseAnswer(string serviceAnswer)
        {
            if (errors.Keys.Contains(serviceAnswer))
                throw new RuCaptchaException(string.Format("{0} ({1})", errors[serviceAnswer], serviceAnswer));
            else if (serviceAnswer.StartsWith("OK|"))
                return serviceAnswer.Substring(3);
            else
                return serviceAnswer;

        }
    }
}
