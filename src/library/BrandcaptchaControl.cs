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
// Copyright (c) 2007 Adrian Godong, Ben Maurer, Mike Hatalski
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
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using Brandcaptcha.Design;

namespace Brandcaptcha
{
    /// <summary>
    /// This class encapsulates Brandcaptcha UI and logic into an ASP.NET server control.
    /// </summary>
    [ToolboxData("<{0}:BrandcaptchaControl runat=\"server\" />")]
    [Designer(typeof(BrandcaptchaControlDesigner))]
    public class BrandcaptchaControl : WebControl, IValidator
    {
        #region Private Fields

        private const string BRANDCAPTCHA_CHALLENGE_FIELD = "brand_cap_challenge";
        private const string BRANDCAPTCHA_RESPONSE_FIELD = "brand_cap_answer";

        private const string BRANDCAPTCHA_SECURE_HOST = "https://api.pontamedia.net/";
        private const string BRANDCAPTCHA_HOST = "http://api.pontamedia.net/";

        private BrandcaptchaResponse _brandcaptchaResponse;

        private string publicKey;
        private string privateKey;
        private bool overrideSecureMode;
        private IWebProxy proxy;

        #endregion

        #region Public Properties

        [Category("Settings")]
        [Description("The public set using BrandcaptchaPublicKey in AppSettings.")]
        public string PublicKey
        {
            get { return this.publicKey; }
            set { this.publicKey = value; }
        }

        [Category("Settings")]
        [Description("The private set using BrandcaptchaPrivateKey in AppSettings.")]
        public string PrivateKey
        {
            get { return this.privateKey; }
            set { this.privateKey = value; }
        }

        [Category("Settings")]
        [DefaultValue(false)]
        [Description("Set this to true to override Brandcaptcha usage of Secure API.")]
        public bool OverrideSecureMode
        {
            get { return this.overrideSecureMode; }
            set { this.overrideSecureMode = value; }
        }

        [Category("Settings")]
        [Description("Set this to override proxy used to validate Brandcaptcha.")]
        public IWebProxy Proxy
        {
            get { return this.proxy; }
            set { this.proxy = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandcaptchaControl"/> class.
        /// </summary>
        public BrandcaptchaControl()
        {
            this.publicKey = ConfigurationManager.AppSettings["BrandcaptchaPublicKey"];
            this.privateKey = ConfigurationManager.AppSettings["BrandcaptchaPrivateKey"];
        }

        #region Overriden Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (string.IsNullOrEmpty(this.PublicKey) || string.IsNullOrEmpty(this.PrivateKey))
            {
                throw new ApplicationException("Brandcaptcha needs to be configured with a public & private key.");
            }

            if (!this.CheckIfBrandcaptchaExists())
            {
                Page.Validators.Add(this);
            }
        }

        /// <summary>
        /// Iterates through the Page.Validators property and look for registered instance of <see cref="BrandcaptchaControl"/>.
        /// </summary>
        /// <returns>True if an instance is found, False otherwise.</returns>
        private bool CheckIfBrandcaptchaExists()
        {
            foreach (var validator in Page.Validators)
            {
                if (validator is BrandcaptchaControl)
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderContents(writer);
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            // <script> display
            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.AddAttribute(HtmlTextWriterAttribute.Src, this.GenerateChallengeUrl(), false);
            output.RenderBeginTag(HtmlTextWriterTag.Script);
            output.RenderEndTag();
        }

        #endregion

        #region IValidator Members

        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public string ErrorMessage
        {
            get
            {
                return (this._brandcaptchaResponse != null) ?
                    this._brandcaptchaResponse.ErrorMessage :
                    null;
            }

            set
            {
                throw new NotImplementedException("ErrorMessage property is not settable.");
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool IsValid
        {
            get
            {
                return this._brandcaptchaResponse == null || this._brandcaptchaResponse.IsValid;
            }

            set
            {
                throw new NotImplementedException("IsValid property is not settable. This property is populated automatically.");
            }
        }

        /// <summary>
        /// Perform validation of Brancaptcha.
        /// </summary>
        public void Validate()
        {
            if (Page.IsPostBack && Visible && Enabled)
            {
                if (this._brandcaptchaResponse == null)
                {
                    if (Visible && Enabled)
                    {
                        BrandcaptchaValidator validator = new BrandcaptchaValidator();
                        validator.PrivateKey = this.PrivateKey;
                        validator.RemoteIP = Page.Request.UserHostAddress;
                        validator.Challenge = Context.Request.Form[BRANDCAPTCHA_CHALLENGE_FIELD];
                        validator.Response = Context.Request.Form[BRANDCAPTCHA_RESPONSE_FIELD];
                        validator.Proxy = this.proxy;

                        if (validator.Challenge == null)
                        {
                            this._brandcaptchaResponse = BrandcaptchaResponse.InvalidChallenge;
                        }
                        else if (validator.Response == null)
                        {
                            this._brandcaptchaResponse = BrandcaptchaResponse.InvalidResponse;
                        }
                        else
                        {
                            this._brandcaptchaResponse = validator.Validate();
                        }
                    }
                }
            }
            else
            {
                this._brandcaptchaResponse = BrandcaptchaResponse.Valid;
            }
        }

        #endregion

        /// <summary>
        /// This function generates challenge URL.
        /// </summary>
        private string GenerateChallengeUrl()
        {
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(Context.Request.IsSecureConnection || this.overrideSecureMode ? BRANDCAPTCHA_SECURE_HOST : BRANDCAPTCHA_HOST);
            urlBuilder.Append("/challenge.php?");
            urlBuilder.AppendFormat("k={0}&", this.PublicKey);
            return urlBuilder.ToString();
        }
    }
}