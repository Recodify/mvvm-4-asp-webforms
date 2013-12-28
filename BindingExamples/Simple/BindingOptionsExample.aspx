<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BindingOptionsExample.aspx.cs" Inherits="SimpleBinding.BindingOptionsExample" %>
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
    <Binding:BindingOptionsControl ID="bindingOptions" runat="server" StateMode=Persist UpdateSourceTrigger=PostBack>
        <Resources>
            <Binding:Options ID="globalBinding" Path="SelectedAddress.PostCODE" Mode=TwoWay></Binding:Options>
        </Resources>
    </Binding:BindingOptionsControl>
    <div>
          <fieldset>
              <legend>Global Binding Resource Example</legend>
              <span>
                    This example uses a BindingOptionsControl to specify a global binding resource which is used across both the following controls
              </span>
              <br />
              <br />
              <!-- 
                       This is an example of reusing a global binding across controls
              -->
              <div>Label:<asp:Label ID="Label1" runat="server" Text='<%# sender.BindR("globalBinding") %>'></asp:Label></div>
              <div><asp:Textbox ID="Textbox2" runat="server" Text='<%# sender.BindR("globalBinding") %>'></asp:Textbox></div>
          </fieldset>  
          <fieldset>
              <legend>Syntax options</legend>
              <span>
                    There are two syntax which can be used when binding. The following labels use differing syntax to produce the same result.
              </span>
              <div>
                    <!-- This is an example of using the long hand syntax to specify binding options  -->
                    Long hand syntax: <asp:Label ID="lbl0" runat="server" Text='<%# sender.Bind(new Options {Path = "SelectedAddress.HouseNameNumber", IsAuthorative = true, Mode=BindingMode.OneWay}) %>'></asp:Label>
              </div>
              <!-- 
                    This is an example of using the anonymous binding syntax to specify biniding options, this will be slower at bind than using the long hand but
                    does not effect unbind times and allows for a slightly cleaner syntax. 
                    Enums values must be specified as strings!
              -->
              <div>
                  Short hand syntax: <asp:Label ID="lbl1" runat="server" Text='<%# sender.Bind(new {Path = "SelectedAddress.HouseNameNumber", IsAuthorative = true, Mode="OneWay"}) %>'></asp:Label>
              </div>
         </fieldset>  
         <asp:Button ID="Button1" runat="server" Text="Cause Postback"/>
    </div>
    </form>
</body>
</html>
