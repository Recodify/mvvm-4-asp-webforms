<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistrationFormExample.aspx.cs" Inherits="SimpleBinding.Advanced.RegistrationFormExample" %>
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
      
        <fieldset>
            <legend>Registration Form</legend>
            <div>
                <asp:Label ID="userMessage" Text='<%# sender.Bind("UserMessage") %>' runat="server"></asp:Label>
                <asp:Panel ID="inputForm" Visible='<%# sender.Bind("ShowInputForm")%>' runat="server">
                       <ol>
                            <li><label>Name:</label> <asp:TextBox ID="tbFirstName" runat="server" Text='<%# sender.Bind("FirstName") %>'></asp:TextBox></li>
                            <li><label>Middle Name:</label> <asp:TextBox ID="TextBox1" runat="server" Text='<%# sender.Bind("MiddleName") %>'></asp:TextBox> </li>
                            <li><label>Last Name:</label> <asp:TextBox ID="TextBox2" runat="server" Text='<%# sender.Bind("LastName") %>'></asp:TextBox> </li>
                            <li><hr /></li>
                            <li><label>Email Address:</label> <asp:TextBox ID="TextBox3" runat="server" Text='<%# sender.Bind("EmailAddress") %>'></asp:TextBox> </li>
                            <li><hr /></li>
                            <li><label>Password:</label> <asp:TextBox ID="TextBox4" runat="server" Text='<%# sender.Bind("Password") %>'></asp:TextBox> </li>
                            <li><label>Confirm Password:</label> <asp:TextBox ID="TextBox5" runat="server" Text='<%# sender.Bind("ConfirmedPassword") %>'></asp:TextBox> </li>
                       </ol>
                       <asp:Button ID="btnSubmit" runat="server" OnClick='<%# sender.BindC("SubmitCommand") %>' Text="Submit" />     
                       
                </asp:Panel>
            </div>
        </fieldset>
    </form>
</body>
</html>
