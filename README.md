rucaptcha-client
================

.NET Client for RuCaptcha service

Небольшой клиент для сервиса RuCaptcha

Пример использования:

<pre style='color:#000020;background:#f6f8ff;'>Загрузка капчи и получение ответа<html><body style='color:#000020; background:#f6f8ff; '><pre>
RuCaptchaClient client <span style='color:#308080; '>=</span> <span style='color:#200080; font-weight:bold; '>new</span> RuCaptchaClient<span style='color:#308080; '>(</span>API_KEY<span style='color:#308080; '>)</span><span style='color:#406080; '>;</span>
<span style='color:#200080; font-weight:bold; '>string</span> captcha_id <span style='color:#308080; '>=</span> client<span style='color:#308080; '>.</span>UploadCaptchaFile<span style='color:#308080; '>(</span>filename<span style='color:#308080; '>)</span><span style='color:#406080; '>;</span>
<span style='color:#200080; font-weight:bold; '>string</span> answer <span style='color:#308080; '>=</span> <span style='color:#200080; font-weight:bold; '>null</span><span style='color:#406080; '>;</span>
<span style='color:#200080; font-weight:bold; '>while</span> <span style='color:#308080; '>(</span><span style='color:#200080; font-weight:bold; '>string</span><span style='color:#308080; '>.</span>IsNullOrEmpty<span style='color:#308080; '>(</span>answer<span style='color:#308080; '>)</span><span style='color:#308080; '>)</span>
<span style='color:#406080; '>{</span>
    Thread<span style='color:#308080; '>.</span>Sleep<span style='color:#308080; '>(</span><span style='color:#008c00; '>5000</span><span style='color:#308080; '>)</span><span style='color:#406080; '>;</span>
    <span style='color:#200080; font-weight:bold; '>try</span>
    <span style='color:#406080; '>{</span>
        answer <span style='color:#308080; '>=</span> client<span style='color:#308080; '>.</span>GetCaptcha<span style='color:#308080; '>(</span>captcha_id<span style='color:#308080; '>)</span><span style='color:#406080; '>;</span>
    <span style='color:#406080; '>}</span>
    <span style='color:#200080; font-weight:bold; '>catch</span> <span style='color:#308080; '>(</span>Exception ex<span style='color:#308080; '>)</span>
    <span style='color:#406080; '>{</span>
        Console<span style='color:#308080; '>.</span>WriteLine<span style='color:#308080; '>(</span>ex<span style='color:#308080; '>.</span>Message<span style='color:#308080; '>)</span><span style='color:#406080; '>;</span>
    <span style='color:#406080; '>}</span>
<span style='color:#406080; '>}</span>
</pre>

В данном примере ожидание ответа сделано в виде цикла с 5-секундной задержкой.
Если сайт вместо ожидаемого слова вернет статус CAPTCHA_NOT_READY и т.п. клиент создаст исключение с соответствующим текстом.
