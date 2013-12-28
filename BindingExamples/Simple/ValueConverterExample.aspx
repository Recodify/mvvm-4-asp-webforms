<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValueConverterExample.aspx.cs" Inherits="SimpleBinding.Simple.ValueConverterExample" %>
<%@ Register  Assembly="ASPBinding" TagPrefix="Binding" Namespace="Binding" %>
<%@ Import Namespace="Binding" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../styles.css" />  
</head>
<body>
    <form id="form1" runat="server">
    <Binding:BindingOptionsControl ID="bindingOptions" runat="server" StateMode="Persist">
    </Binding:BindingOptionsControl>
    <div>
        <fieldset>
            <legend>Oneway Value Converter Example</legend>
            <div>
                This field is bound to an object. An IValueConverter is used to converter the object to a string for presentation
            </div>
            <asp:Label ID="TextBox1" runat="server" Text='<%# sender.Bind(new Options{Path="SelectedAddress", Converter="AddressConverter", Mode=BindingMode.OneWay}) %>'></asp:Label>
        </fieldset>
          <fieldset>
            <legend>Twoway Value Converter Example</legend>
            <div>
                This field is bound to an object. An IValueConverter is used to converter the object to a string for presentation
            </div>
            <asp:TextBox ID="Label1" runat="server" Text='<%# sender.Bind(new Options{Path="UserName", Converter="NameConverter", Mode=BindingMode.TwoWay}) %>'></asp:TextBox>
        </fieldset>
        <asp:Button ID="postback" runat="server" Text="Cause Postback"/>
    </div>
    </form>
</body>
</html>