<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CascadingExample.aspx.cs" Inherits="SimpleBinding.CascadingExample" %>
<%@ Register  Assembly="ASPBinding" TagPrefix="Binding" Namespace="Binding" %>
<%@ Import Namespace="Binding" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../styles.css" />  
</head>
<body>
    <form id="form1" runat="server">
    <Binding:BindingOptionsControl ID="bindingOptions" runat="server" StateMode="Persist">
    </Binding:BindingOptionsControl>
    <div>
        <fieldset>
            <legend>Default bind authority</legend>
            <div>
                These fields are all bound to the same source. By default, the last binding is considered authorative - try updating the 
                last textbox and causing a postback, you will see that the previous two textboxes are also updated.
                Changing the value of the first two textboxes and then causing a postback will result in these values being lost as they 
                are reseting to the value of the athorative binding.
            </div>
            <asp:TextBox ID="Textbox0" runat="server" Text='<%# sender.Bind("SelectedAddress.HouseNameNumber") %>'></asp:TextBox>
            <asp:TextBox ID="TextBox1" runat="server" Text='<%# sender.Bind("SelectedAddress.HouseNameNumber") %>'></asp:TextBox>
            <asp:TextBox ID="TextBox2" runat="server" Text='<%# sender.Bind("SelectedAddress.HouseNameNumber") %>'></asp:TextBox>
            <asp:Button ID="postback" runat="server" Text="Cause Postback"/>
        </fieldset>
        <fieldset>
            <legend>Overriden bind authority</legend>
            <div>
                These fields are also all bound to the same source. In this instance, the default behavior has been overriden so that the
                first textbox is considered authorative
            </div>
            <asp:TextBox ID="Textbox3" runat="server" Text='<%# sender.Bind(new Options{Path="SelectedAddress.Postcode", IsAuthorative=true}) %>'></asp:TextBox>
            <asp:TextBox ID="TextBox4" runat="server" Text='<%# sender.Bind("SelectedAddress.Postcode") %>'></asp:TextBox>
            <asp:TextBox ID="TextBox5" runat="server" Text='<%# sender.Bind("SelectedAddress.Postcode") %>'></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Cause Postback"/>
        </fieldset>
    </div>
    </form>
</body>
</html>
