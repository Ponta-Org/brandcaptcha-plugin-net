using System;

namespace BrandcaptchaTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void BrandcaptchaButton_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                this.BrandcaptchaResult.Text = "Success";
            }
            else
            {
                this.BrandcaptchaResult.Text = this.BrandcaptchaControl.ErrorMessage;

            }
        }
    }
}