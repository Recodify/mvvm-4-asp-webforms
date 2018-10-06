<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NestedStatelessBinding.aspx.cs"
    Inherits="SimpleBinding.NestedStatelessBinding" %>
<%@ Register  Assembly="ASPBinding" TagPrefix="Binding" Namespace="Binding" %>
<%@ Register  Assembly="Binding" TagPrefix="Binding" Namespace="Binding" %>
<%@ Import Namespace="Binding" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../styles.css" /> 
</head>
<body>
    <form id="form1" runat="server">
        <!--We set the StateMode to recreate so that the viewmodel will be created on each postback-->
        <Binding:BindingOptionsControl ID="bindingOptions" runat="server" StateMode="Recreate">
            <Resources>
                <Binding:Options ID="globalBinding" Path="SelectedAddress.PostCode" IsAuthorative="true" Mode="OneWay"></Binding:Options>
            </Resources>
        </Binding:BindingOptionsControl>
        <fieldset>
            <legend>Nested Stateless Binding</legend>
            <span>
                The below shows two repeaters, one as an ItemTemplate of the parent and demonstrates two-way databinding in a nested scenario.
                Notice that the binding expressions relative to the previous collection bind. If the expression was not a child of a collection binding
                then the path would be relative to the DataContext of the IBindingContainer (ie. The ViewModel).
                Because we use a BindingOptionsControl and speficy StateMode="Recreate", the ViewModel is recreated from scratch on each postback
            </span>
            <div>
                <ul>
                    <asp:Repeater ID="rptAddressList" runat="server" DataSource='<%# sender.Bind("AvailableAddresses") %>'>
                        <ItemTemplate>
                            <li>
                                <ul>
                                    <li>House Name/Number:
                                        <asp:TextBox ID="houseName" runat="server" Text='<%# sender.Bind("HouseNameNumber") %>'></asp:TextBox>
                                        <asp:Repeater ID="rptPhoneNumber" runat="server" DataSource='<%# sender.Bind("PhoneNumbers") %>'>
                                            <ItemTemplate>
                                                Phonenumbers :
                                                <asp:TextBox ID="houseName" runat="server" Text='<%# sender.Bind("Number") %>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </li>
                                </ul>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Button ID="postback" runat="server" Text="Cause Postback" />
            </div>
        </fieldset>
    </form>
</body>
</html>
