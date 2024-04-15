using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;
using HeartDisease.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Routing;


namespace HeartDisease
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start(Object sender, EventArgs e)
        {
           

            //--Captcha-- 
            var captchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            //-- this will generate  alphanumeric string------------------
            captchaManager.CharactersFactory = () => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            captchaManager.PlainCaptchaPairFactory = length =>
            {
                string randomText = RandomText.Generate(captchaManager.CharactersFactory(), length);
                bool ignoreCase = false;//This parameter is responsible for ignoring case.
                return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                    new StringCaptchaValue(randomText, randomText, ignoreCase));
            };

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        
    }
}
