// Copyright (c) 2014 by Zarego & PontaMedia
// Author: Carlos A. Bellucci, Daniel G. Gomez 
//
// This is a .NET library that handles calling BrandCaptcha.
//    - Documentation and latest version
//          http://www.pontamedia.com/
//
// This code is based on code from,
// and copied, modified and distributed with permission in accordance with its terms:
//
// Copyright (c) 2007 Adrian Godong, Ben Maurer, Mike Hatalski, Derik Whittaker, Steven Carta
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.UI;
using System.IO;

namespace Brandcaptcha
{
    public static class BrandcaptchaControlMvc
    {
        private static string publicKey;
        private static string privateKey;
        private static bool overrideSecureMode;
        private static IWebProxy proxy;

        public static string PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }

        public static string PrivateKey
        {
            get { return privateKey; }
            set { privateKey = value; }
        }

        public static bool OverrideSecureMode
        {
            get { return overrideSecureMode; }
            set { overrideSecureMode = value; }
        }

        public static IWebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        static BrandcaptchaControlMvc()
        {
            publicKey = ConfigurationManager.AppSettings["BrandcaptchaPublicKey"];
            privateKey = ConfigurationManager.AppSettings["BrandcaptchaPrivateKey"];
        }

        public class CaptchaValidatorAttribute : ActionFilterAttribute
        {
            private const string CHALLENGE_FIELD_KEY = "brand_cap_challenge";
            private const string RESPONSE_FIELD_KEY = "brand_cap_answer";

            private BrandcaptchaResponse _brandcaptchaResponse;

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {

                BrandcaptchaValidator validator = new BrandcaptchaValidator();
                validator.PrivateKey = BrandcaptchaControlMvc.PrivateKey;
                validator.RemoteIP = filterContext.HttpContext.Request.UserHostAddress;
                validator.Challenge = filterContext.HttpContext.Request.Form[CHALLENGE_FIELD_KEY];
                validator.Response = filterContext.HttpContext.Request.Form[RESPONSE_FIELD_KEY];
                validator.Proxy = proxy;

                if (string.IsNullOrEmpty(validator.Challenge))
                {
                    this._brandcaptchaResponse = BrandcaptchaResponse.InvalidChallenge;
                }
                else if (string.IsNullOrEmpty(validator.Response))
                {
                    this._brandcaptchaResponse = BrandcaptchaResponse.InvalidResponse;
                }
                else
                {
                    this._brandcaptchaResponse = validator.Validate();
                }

                // this will push the result values into a parameter in our Action
                filterContext.ActionParameters["captchaValid"] = this._brandcaptchaResponse.IsValid;
                filterContext.ActionParameters["captchaErrorMessage"] = this._brandcaptchaResponse.ErrorMessage;

                base.OnActionExecuting(filterContext);
            }
        }

        public static String GenerateCaptcha(this HtmlHelper helper)
        {
            return GenerateCaptcha(helper, "brandcaptcha", null);
        }

        public static String GenerateCaptcha(this HtmlHelper helper, string id, short? tabIndex)
        {
            if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
            {
                throw new ApplicationException("Brandcaptcha needs to be configured with a public & private key.");
            }

            var captchaControl = new BrandcaptchaControl();
            captchaControl.ID = id;
            if (tabIndex.HasValue) captchaControl.TabIndex = tabIndex.Value;
            captchaControl.PublicKey = publicKey;
            captchaControl.PrivateKey = privateKey;
            captchaControl.OverrideSecureMode = overrideSecureMode;

            var htmlWriter = new HtmlTextWriter(new StringWriter());

            captchaControl.RenderControl(htmlWriter);

            return htmlWriter.InnerWriter.ToString();
        }

    }
}