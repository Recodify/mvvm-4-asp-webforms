<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalculatedCommandCanExecute.aspx.cs" Inherits="SimpleBinding.Advanced.CalculatedCommandCanExecute" %>
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
            <legend>Calculated CanExecute Command Binding</legend>
            <span>
                The following button is bound to a command on the view model. When the button is clicked it changes the result of the
                ICommand.CanExecute so that the button is disabled.
            </span>
            <div>
                <asp:Button ID="btnDoSomething" runat="server" Text="Command Bound" OnClick='<%# sender.BindC("OnClick") %>' />
            </div>
        </fieldset>        
    </div>
    </form>
</body>
</html>