#Using BrandCAPTCHA with ASP.NET

The BrandCAPTCHA ASP.NET Library provides a simple way to place a Brand[CAPTCHA](http://en.wikipedia.org/wiki/Captcha) on your ASP.NET website, helping you stop bots from abusing it. The library wraps the BrandCAPTCHA API.

After you've obtained your API keys, you can continue with the setup of the BrandCAPTCHA library in your website.

##ASP.NET

Add a reference to the library Brandcaptcha.dll in your project.

Insert the BrandCAPTCHA control inside the form you want to protect:

At the beginning of your aspx file, insert the code below:

```
<%@ Register TagPrefix="brandcaptcha" Namespace="Brandcaptcha" Assembly="Brandcaptcha" %>
```

Then insert the BrandCAPTCHA control inside the `<form runat="server">` tag:

```html
<brandcaptcha:BrandCaptchaControl
    ID="brandcaptcha"
    runat="server"
    PublicKey="your_public_key"
    PrivateKey="your_private_key"
    />
```

You will need to replace `your_public_key` and `your_private_key` with your public key and private key, or you can set them inside your configuration file.

Be sure to use the ASP.NET validations to validate your form (check `Page.IsValid` on submit).

##ASP.NET MVC

Add a reference to the library Brandcaptcha.dll in your project.

Insert the BrandCAPTCHA control inside the form you want to protect:

At the beginning of your view, insert the code below:

```
@using Brandcaptcha;
```

Then insert the BrandCAPTCHA control inside the `<form>`:

```
@Html.Raw(Html.GenerateCaptcha())
```

You will need to set the `PublicKey` and `PrivateKey` properties of the BrandcaptchaControlMvc class with your public key and private key, or you can set them inside your configuration file.

To validate the BrandCAPTCHA, you will need to add the `BrandcaptchaControlMvc.CaptchaValidator` attribute and the `captchaValid` and `captchaErrorMessage` parameters to the action you want to protect. Below is an example code in c#:

```c#
[HttpPost]
[BrandcaptchaControlMvc.CaptchaValidator]
public ActionResult FormPostAction(bool captchaValid, string captchaErrorMessage)
{
	
}
```	

##Configuration file

You can set your public and private key inside your application's configuration file, using BrandCaptchaPublicKey and BrandCaptchaPrivateKey inside the `appSettings` section:

```xml
<appSettings>
	<add key="BrandcaptchaPublicKey" value="your_public_key" />
	<add key="BrandcaptchaPrivateKey" value="your_private_key"/>
</appSettings>
```


