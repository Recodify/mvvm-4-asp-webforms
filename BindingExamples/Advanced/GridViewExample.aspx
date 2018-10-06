<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridViewExample.aspx.cs"
    Inherits="SimpleBinding.Advanced.GridViewExample" %>

<%@ Register Assembly="ASPBinding" TagPrefix="Binding" Namespace="Binding" %>
<%@ Register Assembly="Binding" TagPrefix="Binding" Namespace="Binding" %>
<%@ Import Namespace="Binding" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../styles.css" />
</head>
<body>
    <form id="form1" runat="server">
    <!--This example shows:
        A) using AbsolutePaths to bind (out-of-context) to a viewmodel property from within an item template.
        B) using a IValueConverter 
        C) binding via a globalBindingResource
        D) Binding with a GridView control
     -->
    <Binding:BindingOptionsControl ID="bindingOptions" runat="server" StateMode="Persist">
        <resources>
            <%-- A) --%>
            <%--
            <Binding:Options ID="submitAction"  Path="FormSubmit" Mode=Command PathMode=Absolute></Binding:Options>
             --%>
            <Binding:Options ID="employeeNameBinding"  Path="EmployeeName" Mode=TwoWay PathMode=Absolute></Binding:Options>
        </resources>
    </Binding:BindingOptionsControl>
    <fieldset>
        <legend>Grid view two-way databinding</legend>
        <div>
            <!--C)-->
            Showing data for:
            <asp:Label ID="lblEmployeeName" runat="server" Text='<%# sender.BindR("employeeNameBinding") %>'></asp:Label>
            <hr />
            <asp:GridView ID="gvEmployeeLeave" runat="server" AutoGenerateColumns="false" DataSource='<%# sender.Bind("SelectedEmployee.LeavePeriods") %>'>
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <!--A) and C) -->
                            <asp:TextBox ID="litName" runat="server" Text='<%# sender.BindR("employeeNameBinding")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Leave Type">
                        <ItemTemplate>
                            <!--B)-->
                            <asp:Literal ID="litLeaveType" runat="server" Text='<%# sender.Bind(new Options{Path="Type", Converter="LeaveTypeImageConverter", Mode=BindingMode.OneWay})%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Start Date">
                        <ItemTemplate>
                            <asp:TextBox ID="tbStartDate" runat="server" Text='<%# sender.Bind("StartDate")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="End Date">
                        <ItemTemplate>
                            <asp:TextBox ID="tbEndDate" runat="server" Text='<%# sender.Bind("EndDate")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="postback" runat="server" Text="Cause Postback" />
        </div>
    </fieldset>
    </form>
</body>
</html>
