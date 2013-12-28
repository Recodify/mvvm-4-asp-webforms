<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProgrammaticBinding.aspx.cs" Inherits="SimpleBinding.Simple.ProgrammaticBinding" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="Stylesheet" type="text/css" href="../styles.css" />  
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <fieldset>
        <legend>Programatic Data Binding</legend>
        <span>
            This field has been data bound from code
        </span>
        <div>
            <asp:Textbox ID="lblEmployeeName" runat="server"></asp:Textbox>
            <asp:Button ID="postback" runat="server" Text="Cause Postback"/>
        </div>
    </fieldset>
    </form>
</body>
</html>
