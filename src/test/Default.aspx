<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BrandcaptchaTest._Default" %>
<%@ Register TagPrefix="brandcaptcha" Namespace="Brandcaptcha" Assembly="Brandcaptcha" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <brandcaptcha:BrandcaptchaControl ID="BrandcaptchaControl" 
            
            PublicKey="__your_public_key__" 
            PrivateKey="__your_private_key__" runat="server" 
         />
            
         <br />
        <asp:Label ID="BrandcaptchaResult" runat="server" /><br />
        <asp:Button ID="BrandcaptchaButton" Text="Submit" runat="server" onclick="BrandcaptchaButton_Click" />
    </div>
    </form>
</body>
</html>