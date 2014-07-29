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
// Copyright (c) 2007 Adrian Godong, Ben Maurer
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

namespace Brandcaptcha
{
    /// <summary>
    /// Encapsulates a response from Brandcaptcha web service.
    /// </summary>
    public class BrandcaptchaResponse
    {
        public static readonly BrandcaptchaResponse Valid = new BrandcaptchaResponse(true, string.Empty);
        public static readonly BrandcaptchaResponse InvalidChallenge = new BrandcaptchaResponse(false, "Invalid Brandcaptcha request. Missing challenge value.");
        public static readonly BrandcaptchaResponse InvalidResponse = new BrandcaptchaResponse(false, "Invalid Brandcaptcha request. Missing response value.");
        public static readonly BrandcaptchaResponse InvalidSolution = new BrandcaptchaResponse(false, "The verification words are incorrect.");
        public static readonly BrandcaptchaResponse BrandcaptchaNotReachable = new BrandcaptchaResponse(false, "The Brandcaptcha server is unavailable.");

        private bool isValid;
        private string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandcaptchaResponse"/> class.
        /// </summary>
        /// <param name="isValid">Value indicates whether submitted Brandcaptcha is valid.</param>
        /// <param name="errorCode">Error code returned from Brandcaptcha web service.</param>
        internal BrandcaptchaResponse(bool isValid, string errorMessage)
        {
            BrandcaptchaResponse templateResponse = null;

            if (IsValid)
            {
                templateResponse = BrandcaptchaResponse.Valid;
            }
            else
            {
                switch (errorMessage)
                {
                    case "incorrect-captcha-sol":
                        templateResponse = BrandcaptchaResponse.InvalidSolution;
                        break;
                    case null:
                        throw new ArgumentNullException("errorMessage");
                }
            }

            if (templateResponse != null)
            {
                this.isValid = templateResponse.IsValid;
                this.errorMessage = templateResponse.ErrorMessage;
            }
            else
            {
                this.isValid = isValid;
                this.errorMessage = errorMessage;
            }
        }

        public bool IsValid
        {
            get { return this.isValid; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        public override bool Equals(object obj)
        {
            BrandcaptchaResponse other = (BrandcaptchaResponse)obj;
            if (other == null)
            {
                return false;
            }

            return other.IsValid == this.isValid && other.ErrorMessage == this.errorMessage;
        }

        public override int GetHashCode()
        {
            return this.isValid.GetHashCode() ^ this.errorMessage.GetHashCode();
        }
    }
}