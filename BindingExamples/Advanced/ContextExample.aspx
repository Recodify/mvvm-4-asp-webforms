<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContextExample.aspx.cs" Inherits="SimpleBinding.Advanced.ContextExample" %>
<%@ Import Namespace="Binding" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../styles.css" />  
</head>
<body>
    <form id="form1" runat="server">
    <div>
          <fieldset>
            <legend>Determining the Context</legend>
            <span>
               Below shows a number of data bindings with comments to explain what the context is and why it is such.
            </span>
            <div>
                <!--Context is the DataContext of the Page(ViewModel) as we have specified PathMode=Absolute -->
                <asp:TextBox ID="TextBox0" runat="server" Text='<%# sender.Bind("AvailableAddresses[0].HouseNameNumber") %>'></asp:TextBox>
                <!--Context is the DataContext of the Page(ViewModel) -->
                <ul>
                    <asp:Repeater ID="rptAddressList" runat="server" DataSource='<%# sender.Bind("AvailableAddresses") %>'>
                        <ItemTemplate>
                            <li>
                                <ul>
                                    <li>House Name/Number:
                                        <!--Context is the DataItem of the RepeaterItem (we have a parent binding that has supplied an IEnumerable)-->
                                        <asp:TextBox ID="houseName" runat="server" Text='<%# sender.Bind("HouseNameNumber") %>'></asp:TextBox>                                    
                                        <!--Context is the DataContext of the Page(ViewModel) as we have specified PathMode=Absolute -->
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# sender.Bind(new Options{Path="AvailableAddresses[0].HouseNameNumber",PathMode=PathMode.Absolute}) %>'></asp:TextBox>
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
