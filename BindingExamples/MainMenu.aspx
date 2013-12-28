<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.aspx.cs" Inherits="SimpleBinding.MainMenu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../styles.css" />  
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Main Menu</h1>
        <p>
            Please use the table below to select an example page to view. Refer to the source code for more explanation.
            The easiest way to starting digging into the examples is to place a break point in the supplied Page_PreRender method which
            will allow you to inspect the DataContext property to verify binding and unbinding.
        </p>
        <table>
            <tr>
                <th colspan="2">Simple</th>
            </tr>
            <tr>
                <td><a href="Simple/DataBinding.aspx">Two-way Data Binding</a></td>
                <td>Basic example of two-way data-binding.</td>
            </tr>
            <tr>
                <td><a href="Simple/CommandBinding.aspx">Command Binding</a></td>
                <td>Basic example of binding to an ICommand.</td>
            </tr>
            <tr>
                <td><a href="Simple/BindingOptionsExample.aspx">Binding Options Control</a></td>
                <td>Basic example of using a BindingOptionsControl</td>
            </tr>
            <tr>
                <td><a href="Simple/ProgrammaticBinding.aspx">Programatic Data Binding</a></td>
                <td>Basic example of programatic data binding.</td>
            </tr>
            <tr>
                <td><a href="Simple/ValueConverterExample.aspx">Value Converter</a></td>
                <td>Basic example of using an IValueConverter</td>
            </tr>
            <tr>
                <th colspan="2">Advanced</th>
            </tr>
             <tr>
                <td><a href="Advanced/NestedBindingExample.aspx">Nested Two-way Data Binding</a></td>
                <td>Shows using two-way binding in a nested scenario. This is useful for using binding within ItemTemplates or
                    in any parent/child scenario.
                </td>
            </tr>
            <tr>
                <td><a href="Advanced/NestedStatelessBinding.aspx">Nested Two-way Data Binding (Stateless)</a></td>
                <td>Shows using two-way binding in a nested scenario. This is useful for using binding within ItemTemplates or
                    in any parent/child scenario. This example enables stateless mode.
                </td>
            </tr>
            <tr>
                <td><a href="Advanced/GridViewExample.aspx">GridView Binding</a></td>
                <td>
                      This example shows:
                        <ul>
                            <li>using AbsolutePaths to bind to a viewmodel property from within an item template.</li>
                            <li>using a IValueConverter</li>
                            <li>binding via a globalBindingResource</li>
                            <li>Binding with a GridView control</li>
                        </ul>
                </td>
            </tr>
            <tr>
                <td><a href="Advanced/RegistrationFormExample.aspx">Registration Form Example</a></td>
                <td>
                      This example shows:
                        <ul>
                            <li>Using a ViewModel to control element visibility</li>
                            <li>Command Binding</li>
                            <li>PostBack Unbinding</li>
                        </ul>
                </td>
            </tr>
             <tr>
                <td><a href="Advanced/RegistrationFormExplicitExample.aspx">Explicit Unbind Registration Form Example</a></td>
                <td>
                      This example shows:
                        <ul>
                            <li>Using a ViewModel to control element visibility</li>
                            <li>Command Binding</li>
                            <li>Explicit Unbinding</li>
                        </ul>
                </td>
            </tr>
            
            <tr>
                <td><a href="Advanced/CalculatedCommandCanExecute.aspx">Calculated CanExecute Example</a></td>
                <td>
                    This Example shows how to control the state of input controls by utilising the command model.
                </td>
            </tr>
            <tr>
                <td><a href="Advanced/ContextExample.aspx">Context Examples</a></td>
                <td>
                    Shows a number of binding statements and how to determine/control the context to which they apply.
                    This example shows:
                        <ul>
                            <li>PathMode</li>
                            <li>Relative Binding</li>
                            <li>Absolute Binding</li>
                            <li>Binding within a template</li>
                            <li>Binding to a collection</li>
                        </ul>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
