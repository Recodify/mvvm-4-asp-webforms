<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BindingExample.aspx.cs" Inherits="Barebones.BindingExample" %>
<%--Import 1 namespace and as far as markup is concerned you're ready to go! --%>
<%@ Import Namespace="Binding" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <!--Two-way (default) data bindings-->
        ID: <asp:Label ID="lbID" runat="server" Text='<%# sender.Bind("ID") %>'></asp:Label>
        <br />
        First Name: <asp:TextBox ID="tbFirstName" runat="server" Text='<%# sender.Bind("FirstName") %>'></asp:TextBox>
        Last Name: <asp:TextBox ID="tbSecondName" runat="server" Text='<%# sender.Bind("LastName") %>'></asp:TextBox>
        Date: <asp:TextBox ID="tbCreatedDate" runat="server" Text='<%# sender.Bind("CreatedDate") %>'></asp:TextBox>
        <!-- Command Binding -->
        <asp:Button ID="btnSubmit" runat="server" OnClick='<%# sender.BindC("OnClick") %>' Text="Submit"/>
    </div>
    </form>
</body>
</html>
