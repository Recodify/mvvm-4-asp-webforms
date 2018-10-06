<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataBinding.aspx.cs" Inherits="SimpleBinding.Simple.TwoWayDataBinding" %>
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
            <legend>View model inspector</legend>
            <div>
                These fields are bound to the same properties as the text boxes below.
            </div>
            <asp:Label ID="Label2" runat="server" Text='<%# sender.Bind("SelectedAddress.HouseNameNumber") %>'></asp:Label>
            <asp:Label ID="Label3" runat="server" Text='<%# sender.Bind("SelectedAddress.Postcode") %>'></asp:Label>
        </fieldset>
        <fieldset>
            <legend>Simple Two-way Binding</legend>
            <div>
                These fields are bound to a view model using the default (two-way) binding. Update the values on the textboxes below 
                and cause a postback. The entered data should be reflected in the labels above.
            </div>
            <asp:TextBox ID="TextBox1" runat="server" Text='<%# sender.Bind("SelectedAddress.HouseNameNumber") %>'></asp:TextBox>
            <asp:TextBox ID="TextBox2" runat="server" Text='<%# sender.Bind("SelectedAddress.Postcode") %>'></asp:TextBox>
           
            <asp:Button ID="postback" runat="server" Text="Cause Postback"/>
        </fieldset>
        
    </div>
    </form>
</body>
</html>