<h2>Simple Example</h2>

<p>The best place to start is with the creation of a ViewModel.</p>

<pre lang="cs">[Serializable]
public class ViewModel
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; }

    public ClickCommand OnClick
    {
        get
        {
            return new ClickCommand();
        }
    }

    public ViewModel()
    {
        //Just some default values so we see something on the screen.
        //In a real world scenerio, these would be loaded from the model.
        ID = 1;
        FirstName = &quot;Dave&quot;;
        LastName = &quot;Smith&quot;;
        CreatedDate = new DateTime(1983, 07, 01);
    }
}</pre>

<p>Nothing complicated or scary about that. Just a simple class which exposes some properties to which we will bind. Note the <code>SerializableAttribute</code>, this is required only if we intend to use <code>StateMode.Persist</code> which stores the ViewModel in the View State. Another thing that probably stands out is the <code>ClickCommand</code>. This is simply an implementation of <a href="http://msdn.microsoft.com/en-us/library/system.windows.input.icommand.aspx"><code>ICommand</code></a>, as follows:</p>

<pre lang="cs">public class ClickCommand : ICommand
{
    #region ICommand Members

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        
    }

    #endregion
}</pre>

<p>The above <code>ICommand</code> performs no action, but demonstrates how to wire a command to a method defined via the ViewModel.</p>

<p>Next, we set this ViewModel as the data context of the page. We do this by implementing the <code>IBindingContainer</code> interface and by calling an (extension) method from the overridden <code>onload</code> event of the page's code-behind.</p>

<pre lang="cs">//two using statements
using Binding.Interfaces;
using Binding;

namespace Barebones
{
    /*Implement one interface, with one property only*/
    public partial class BindingExample : System.Web.UI.Page, IBindingContainer
    {

        #region IBindingContainer Members

        private object dataContext = new ViewModel();
        public object DataContext
        {
            get { return dataContext; }
            set { dataContext = value; }
        }

        #endregion

        //override one method
        protected override void OnLoad(EventArgs e)
        {
            //call one method
            this.RegisterForBinding();
            base.OnLoad(e);
        }
       
    }
}</pre>

<p>The extremely straightforward code above sets the data context of the page by returning our ViewModel as the value of the <code>DataContext</code> property, and registers the page as a binding container by calling <code>RegisterForBinding()</code>.</p>

<p>That's all the plumbing required in order to start developing using MVVM and to start harnessing two-way data binding.</p>

<p>The next step is to create our view (i.e., controls and some bindings).</p>

<pre lang="aspnet">&lt;%--Import 1 namespace and as far as markup is concerned you're ready to go! --%&gt;
&lt;%@ Import Namespace=&quot;Binding&quot; %&gt;


&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
&lt;head runat=&quot;server&quot;&gt;
    &lt;title&gt;&lt;/title&gt;
&lt;/head&gt;
&lt;body&gt;
    &lt;form id=&quot;form1&quot; runat=&quot;server&quot;&gt;
    &lt;div&gt;
        &lt;!--Two-way (default) data bindings--&gt;
        ID: &lt;asp:Label ID=&quot;lbID&quot; runat=&quot;server&quot; 
                Text='&lt;%# sender.Bind(&quot;ID&quot;) %&gt;'&gt;&lt;/asp:Label&gt;
        &lt;br /&gt;
        First Name: &lt;asp:TextBox ID=&quot;tbFirstName&quot; runat=&quot;server&quot; 
                        Text='&lt;%# sender.Bind(&quot;FirstName&quot;) %&gt;'&gt;&lt;/asp:TextBox&gt;
        Last Name: &lt;asp:TextBox ID=&quot;tbSecondName&quot; runat=&quot;server&quot; 
                       Text='&lt;%# sender.Bind(&quot;LastName&quot;) %&gt;'&gt;&lt;/asp:TextBox&gt;
        Date: &lt;asp:TextBox ID=&quot;tbCreatedDate&quot; runat=&quot;server&quot; 
                   Text='&lt;%# sender.Bind(&quot;CreatedDate&quot;) %&gt;'&gt;&lt;/asp:TextBox&gt;
        &lt;!-- Command Binding --&gt;
        &lt;asp:Button ID=&quot;btnSubmit&quot; runat=&quot;server&quot; 
            OnClick='&lt;%# sender.BindC(&quot;OnClick&quot;) %&gt;' Text=&quot;Submit&quot;/&gt;
    &lt;/div&gt;
    &lt;/form&gt;
&lt;/body&gt;
&lt;/html&gt;
--&gt;</pre>

<p>We're now ready to run the example. Doing so will result in the following:</p>

<p><img width="449" height="197" alt="a screenshot of the simple example running" src="WPFDataBindingInASPX/simpleExampleScreen.jpg" complete="true" /></p>

<p>To test the above, I suggest adding a <code>Page_PreRender</code> handler to your code-behind and setting a breakpoint. Also place a breakpoint in the <code>Execute</code> method of the <code>ClickCommand</code>. Modify the values of the textboxes and click Submit. First the breakpoint in the <code>Execute</code> method will be hit, next the <code>Page_PreRender</code> one. Examine the <code>DataContext</code> property (ViewModel) to see the unbound values from the textboxes. Using <code>Page_PreRender</code> in this manner is the recommended approach for ViewModel/DataContext verification with all supplied examples as by this point in the PLC, everything bind-y should have run.</p>

<p>Something that is worth explaining, which some of you will be scratching your heads over: <code>sender.Bind(&quot;...&quot;)</code> and <code>sender.BindC(&quot;...&quot;)</code>.</p>

<p><code>sender</code>? Where? What? Why? This is to do with taking advantage of the ASP.NET data binding lifecycle which unfortunately doesn't offer much in the way of extensibility, and so this is really a hack in order for us to get a hook-in from which we can dangle the rest of our framework. There are other ways of doing this, of course: static method calls from mark-up, page base classes, event handlers, protected code-behind methods - but all these require either more code-behind, or inheriting from a common super-class which we are trying to avoid. So, where does the &quot;<code>sender</code>&quot; come from? See the following dis-assembled method from the above example's page:</p>

<pre lang="cs">public void __DataBindingtbFirstName(object sender, EventArgs e)
{
    TextBox dataBindingExpressionBuilderTarget = (TextBox) sender;
    BindingExample Container = 
      (BindingExample) dataBindingExpressionBuilderTarget.BindingContainer;
    dataBindingExpressionBuilderTarget.Text = 
      Convert.ToString(BindingHelpers.Bind(sender, &quot;FirstName&quot;), 
      CultureInfo.CurrentCulture);
}</pre>

<p>The data-binding statement we author in mark-up is executed in the scope of the above method. We also have the following extension method defined (amongst others):</p>

<pre lang="cs">/// &lt;summary&gt;
/// Bind with the default options
/// &lt;/summary&gt;
/// &lt;param name=&quot;control&quot; /&gt;
/// &lt;param name=&quot;sourcePath&quot; /&gt;
/// &lt;returns&gt;
public static object Bind(this object control, string sourcePath)
{
    return Bind(control, new Options { Path=sourcePath });
}</pre>

<p>So what we're doing with <code>sender.Bind</code> is simply calling an extension method of <code>System.Object</code>.</p>

<p>This is, of course, a very simple example, but I hope it demonstrates the ease with which the supplied framework can be harnessed. Please refer to the <em>BindingExamples</em> project (and its menu - <em>MainMenu.aspx</em>) for further examples applicable to a number of disparate scenarios and for a demonstration of the features available for you to use.</p>

<h2>Features So Far...</h2>

<ul>
<li>One-way and two-way data binding</li>

<li>Tiny amounts of integration code</li>

<li><code>IValueConverter</code> support</li>

<li>Implicit and explicit precedence - If you bind multiple controls to a single source, you can control which control &quot;Wins&quot;/&quot;IsAuthorative&quot; on unbind</li>

<li>Global binding resources - Allow one binding declaration to be used across multiple controls</li>

<li>Stateful and stateless binding - Choose to persist the ViewModel in the View State between postbacks, or recreate it each time</li>

<li>Cascading updates (see section with same title for more information)</li>

<li>Automatic or explicit unbind - Choose to have the framework automatically unbind on each post back, or manually initiate the unbind operation when required</li>

<li>Fully unit testable - Dependency Injection/Inversion of Control utilised to allow easy mocking</li>

<li>Support deep binding paths - Will happily traverse and bind to child properties (no limit to the size of the object graphs)</li>

<li>Full support for declarative binding</li>

<li>Partial support for programmatic binding</li>

<li>No base classes required - Allows easy integration with existing frameworks</li>

<li>Full support for static or dynamic binding - Use predefined data bindings or generate them based on application flow/data flow</li>

<li>Relative or absolute binding paths - Bind &quot;out of context&quot; using absolute binding expressions or as part of a nested hierarchy using relative binding paths</li>

<li>Works entirely in nested scenarios - Data contexts are inherited from the page or parent binding</li>
</ul>

<h2>...and Some Yet to Come</h2>

<ul>
<li>Property coercion</li>

<li>Built-in validation</li>

<li>Cascading updates without <code>INotifyPropertyChanged</code> - possible as we <strong>know</strong> when values have changed</li>

<li>UI Element binding - The ability to bind to the properties of other controls (ala WPF)</li>

<li>Ancestor binding - The ability to bind to Ancestors of a certain type</li>

<li>Custom View State serializer - avoid the need for <code>[field: NonSerialized]</code> when using <code>INotifyPropertyChanged</code></li>

<li>Support for <a href="http://msdn.microsoft.com/en-us/library/system.componentmodel.idataerrorinfo.aspx"><code>IDataErrorInfo</code></a></li>

<li>The ability to have multiple models/contexts - Utilise multiple View Models/data contexts via a dictionary based view data context collection</li>
</ul>

<h3>Exploring the Syntax</h3>

<p>There are three types of binding (inline)<strong>Data</strong>, (inline)<strong>Command</strong>, and (global)<strong>Resource</strong>. These are utilised by the following binding methods:</p>

<ul>
<li><strong>Data</strong>: <code>sender.Bind()</code></li>

<li><strong>Command</strong>: <code>sender.BindC()</code>/<code>sender.BindCommand()</code></li>

<li><strong>Resource</strong>: <code>sender.BindR()</code>/<code>sender.BindResource()</code></li>
</ul>

<p>(Where two methods are specified, one is simply a shorthand syntax of the longer version to reduce mark-up verbosity.)</p>

<p>Command and Databinding are self explanatory, one is for data and supports two-way, one is for commands and is one-way only. The resource binding is offered as a stand-in to WPF resources, enabling you to specify a binding once and use it across many controls. Resource bindings can be either Command or Data bindings; specify which via the <code>Mode</code> property when declaring global resources - see <em>BindingExamples/Advanced/GridViewExample.aspx</em> for an example of this in action.</p>

<p>Simple bindings are created simply with:</p>

<pre lang="aspnet">Text='&lt;%# Bind(&quot;Expression&quot;) %&gt;'
Text='&lt;%# BindC(&quot;Expression&quot;) %&gt;'
Text='&lt;%# BindR(&quot;ResourceID&quot;) %&gt;'</pre>

<p>There is also an extended syntax that allows a greater level of control over the binding:</p>

<pre lang="aspnet">Text='&lt;%# sender.Bind(new Options{Path=&quot;Expression&quot;, 
          Converter=&quot;LeaveTypeImageConverter&quot;, 
          Mode=BindingMode.OneWay, IsAuthorative=true})%&gt;'
Text='&lt;%# sender.BindC(new Options{Path=&quot;Expression&quot;, 
          Converter=&quot;LeaveTypeImageConverter&quot;, 
          Mode=BindingMode.Command, IsAuthorative=true})%&gt;'
Text='&lt;%# BindR(&quot;ResourceID&quot;) %&gt;'</pre>

<p>As you can see, the extended syntax is only available for Command and Data bindings as Resource bindings are controlled via their declaration. When specifying a <code>Converter</code>, this should be the fully-qualified type name of the converter.</p>

<p>A third syntax is also available, a shorthand version of the extended syntax above (see <em>BindingExamples/Simple/BindingOptionsExample.aspx</em> for a demonstration):</p>

<pre lang="aspnet">Text='&lt;%# sender.Bind(new {Path = &quot;Type&quot;, 
          Converter=&quot;LeaveTypeImageConverter&quot;, 
          Mode=&quot;OneWay&quot;, IsAuthorative=true}) %&gt;'
Text='&lt;%# sender.BindC(new {Path = &quot;Type&quot;, 
          Converter=&quot;LeaveTypeImageConverter&quot;, 
          Mode=&quot;Command&quot;, IsAuthorative=true}) %&gt;'
Text='&lt;%# BindR(&quot;ResourceID&quot;) %&gt;'</pre>

<p>The supplied examples are really the best place to start seeing this syntax in action. I've tried to make it natural, intuitive, and WPF-like as possible within the constraints of ASP.NET.</p>

<h3>StateMode - Persist or to Not Persist</h3>

<p>When utilising this framework, you have a choice between two <code>StateMode</code>s: <code>Persist</code> and <code>Recreate</code>.</p>

<p>With <code>StateMode.Persist</code>, the <code>DataContext</code> of the page will be stored in View State between page requests so that you can work against a stateful ViewModel. This is the default mode and has been implemented to mimic WPF-centric MVVM as closely as possible. To use <code>StateMode.Persist</code>, the ViewModel must be <code>[Serializable]</code>.</p>

<p>With <code>StateMode.Recreate</code>, the <code>DataContext</code> of the page must be recreated on each postback and as such is the responsibility of the page developer. This could be as simple as instantiating a new instance of the ViewModel and returning it via the getter of the <code>DataContext</code> property of the page.</p>

<p>I feel that choosing the correct <code>StateMode</code> will depend on the scenario, and so I have left it down to the consumer to choose the mode most suitable for the situation.</p>

<p>Controlling <code>StateMode</code> can be done on a page by page or site-wide basis. If the mode is specified for a page, it will override any site-wide settings.</p>

<p>To set the <code>StateMode</code> for the entire site, use the following in <em>web.config</em>:</p>

<pre lang="xml">&lt;appSettings&gt;
    &lt;add key=&quot;BindingStateMode&quot; value=&quot;Persist&quot;/&gt;
&lt;/appSettings&gt;</pre>

<p>Or:</p>

<pre lang="xml">&lt;appSettings&gt;
    &lt;add key=&quot;BindingStateMode&quot; value=&quot;Recreate&quot;/&gt;
&lt;/appSettings&gt;</pre>

<p>To set on a page by page basis, use a <code>BindingOptionsControl</code> (see: <em>BindingExamples/Advanced/NestedStatelessBinding.aspx</em>).</p>

<pre lang="aspnet">&lt;Binding:BindingOptionsControl ID=&quot;bindingOptions&quot; runat=&quot;server&quot; StateMode=&quot;Persist&quot; /&gt;</pre>

<p>Or:</p>

<pre lang="aspnet">&lt;Binding:BindingOptionsControl ID=&quot;bindingOptions&quot; runat=&quot;server&quot; StateMode=&quot;Recreate&quot; /&gt;</pre>

<h3>Controlling the Unbind</h3>

<p>You can choose between simple and automated - <code>UpdateSourceTrigger.PostBack</code> - and complete control - <code>UpdateSourceTrigger.Explicit</code> - when it comes to initiating the unbind operation.</p>

<p><code>UpdateSourceTrigger.PostBack</code> will unbind the View to the ViewModel on each postback so that the latest UI data is available anytime after <code>Load</code> on every postback.</p>

<p><code>UpdateSourceTrigger.Explicit</code> will only unbind the View when you tell it to. In order to initiate an unbind with this mode set, you should call <code>this.Unbind()</code> from the page, or <code>BinderBase.ExecuteUnbind()</code> from the ViewModel or <code>ICommand</code>.</p>

<p>See <em>DomainModel\ViewModels\RegistrationFormExample.aspx</em> and <em>DomainModel\ViewModels\RegistrationFormExplicitExample.aspx</em> for examples of using these two approaches.</p>

<p>As with <code>StateMode</code>, the <code>UpdateSourceTrigger</code> can be set either site-wide via <em>web.config</em> or on a page by page basis using a <code>BindingOptionsControl</code>.</p>

<p>To set <code>UpdateSourceTrigger</code> for the entire site user:</p>

<pre lang="xml">&lt;appSettings&gt;
    &lt;add key=&quot;UpdateSourceTrigger&quot; value=&quot;PostBack&quot;/&gt;
&lt;/appSettings&gt;</pre>

<p>Or:</p>

<pre lang="xml">&lt;appSettings&gt;
    &lt;add key=&quot;UpdateSourceTrigger&quot; value=&quot;Explicit&quot;/&gt;
&lt;/appSettings&gt;</pre>

<p>To set on a page by page basis, use:</p>

<pre lang="aspnet">&lt;Binding:BindingOptionsControl ID=&quot;BindingOptionsControl1&quot; 
         runat=&quot;server&quot; UpdateSourceTrigger=&quot;PostBack&quot; /&gt;</pre>

<p>Or:</p>

<pre lang="aspnet">&lt;Binding:BindingOptionsControl ID=&quot;BindingOptionsControl1&quot; 
         runat=&quot;server&quot; UpdateSourceTrigger=&quot;Explicit&quot; /&gt;</pre>

<h3>Diving Deeper</h3>

<p>I will not try and provide a blow by blow of exactly how this framework has been constructed because this article is already getting long and my goal here is to provide a starting point, some background information, and offer some insight to some aspects I feel are less than obvious and which might bite you during your explanation/utilisation of this framework. This isn't to say I don't think there's some value in an explanation of how the framework is implemented, if people would like a follow-up article - a deep dive - then speak up and I'll get to work.</p>

<h2>Points of Interest</h2>

<h3>Notes on Context</h3>

<p>When creating a binding expression, the most important piece of information to have in mind is the context to which the statement will apply. In other words, which object's properties am I targeting when I write: <code>Employee.FirstName</code>. In straight-forward cases, the answer to this is simple: the <code>DataContext</code> of the page (your ViewModel). Life isn't always simple though, as this is not always the case. The more precise answer to <strong>&quot;what is the context?&quot;</strong> is the <code>DataContext</code> of the page <strong><em>or</em></strong> the object returned by a parent binding, whichever is closest (most recent ancestor). The &quot;parent binding&quot; is only applicable if it is bound via the framework; standard ASP.NET data binding or programmatic assignment of data sources do not count as parent bindings, in which case the context would still be the <code>DataContext</code> of the page. The packaged source contains an example showing this concept at work. Please refer to <em>BindingExamples/Advanced/ContextExample.aspx</em>.</p>

<h3>Testing</h3>

<p>One of the key motivations behind the adoption of the MVVM architecture (as well as MVP and MVC implementations) is a separation of responsibility that allows for business and application logic to be placed in classes that are inherently unit testable. The draw backs of the standard WebForms postback/code-behind model are well documented and have led to the evolution of WCSF and ASP.NET MVC. Using the proposed binding framework along with an MVVM pattern will allow this same level of testability. I might even argue that it leads to greater test coverage than WCSF/MVP because of the ability to almost completely dispose of code in the code-behind, but I won't take that argument further as they both allow for very testable code.</p>

<p>The use of the MVVM pattern (especially when coupled with a DI container and IOC) means that you can write ViewModels that are totally decoupled from the rest of the application and as such should be trivial to write unit tests for. For more information on unit testing View Models, see: <a href="http://msdn.microsoft.com/en-us/magazine/dd419663.aspx">Unit testing View Models</a></p>

<p>The supplied solution contains a number of tests that are written to test the framework itself. I'm under no illusions about the coverage of these tests, I know that they're fairly limited, but I hope that they show that the framework itself, as well as the code written to utilise it, has been written in a manner that allows for the code to be fully exercised via unit tests. As this project moves forward, the expansion of this test suite is something that I plan to tackle as a priority.</p>

<h3>Case Sensitivity</h3>

<p>The standard <code>Databinder.Eval</code> is used during bind. <code>Databinder.Eval</code> ignores the case of the properties to which you bind, binding to the first property it finds with the specified name, regardless of case. Personally, I don't like this very much and would rather have the extra control afforded by explicitly matching case, but in order to preserve consistency, this case insensitivity when matching properties has been replicated for unbind operations in this framework.</p>

<h3>Change Notification and Cascading Updates</h3>

<p>Cascading updates are defined as: If multiple controls are bound to the same ViewModel property and the value of that property is modified by an unbind operation, then all bound controls are updated with the new value.</p>

<p>For example: I have a textbox and label bound to the <code>EmployeeName</code> property on the ViewModel. I enter a new name into the textbox and initiate a postback. When the page is rendered back to the client, I would also expect the label to reflect the new value.</p>

<p>In order to support cascading updates, the ViewModel must implement <code>INotifyPropertyChanged</code> or <code>INotifyCollectionChanged</code> (for <code>IEnumerable</code>). If binding via deep paths (properties of objects exposed via the ViewModel's own properties), then the underlying objects must also implement <code>INotifyPropertyChanged</code> and the events must propagate to the parent (ViewModel) which in turn must raise the <code>PropertyChanged</code> event. The framework also includes a custom collection <code>NotifyPropertyCollection&lt;t&gt;</code> which should ease development when exposing collections of <code>INotifyPropertyChanged</code> objects via the View Model. This should not be confused with <code>System.Collections.ObjectModel.ObservableCollection</code> or <code>System.Collections.Specialized.INotifyCollectionChanged</code> which are instead used for monitoring the state of a collection, not the properties which the items of that collection expose.</p>

<p>For more information on <code>INotifyPropertyChanged</code> and event propagation, see:</p>

<ul>
<li><a href="http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.85).aspx">http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.85).aspx</a></li>

<li><a href="http://blogs.imeta.co.uk/jyoung/archive/2010/04/06/848.aspx">http://blogs.imeta.co.uk/jyoung/archive/2010/04/06/848.aspx</a></li>

<li><a href="http://www.codeproject.com/KB/cs/PropertyNotifyPart2.aspx">http://www.codeproject.com/KB/cs/PropertyNotifyPart2.aspx</a></li>
</ul>

<p><strong>N.B.</strong>: When implementing <code>INotifyPropertyChanged</code> on objects marked as <code>serializable</code>, you must apply <code>[field: NonSerialized]</code> to the <code>PropertyChanged</code> event declaration in order to avoid the <code>BinaryFormater</code> trying to serialize the methods (and their parent objects!) that are subscribed to this event. For example:</p>

<pre lang="cs">[field: NonSerialized]
public event PropertyChangedEventHandler PropertyChanged;</pre>

<h3>ViewModel Integrity</h3>

<p>In order to avoid issues with data integrity, it is important to understand the mechanism used for locating the object and property (read - original source) during the unbind operation. The binding system does not use keys (IDs) to resolve a binding to an object at unbind time, instead relys on an indexed path which is stored in the View State between postbacks. The following is an example of a path: <code>AvailableAddresses[1].PhoneNumbers[3].AreaCode</code>.</p>

<p>This method is different to the mechanism used by stateful environments like Silverlight and WPF which have the luxury of being able to map back to the same object from which the data was initially retrieved and as such can rely on object equality, the ideal scenario. Due to the stateless nature of the web, we do not have this option in ASP.NET and so we must store a path (including the indexers if binding to collections) so that we may traverse this path at unbind time in order to ascertain where to write back the value retrieved from the View. This method, whilst effective, does introduce a responsibility which must be appreciated in order to avoid data corruption in the ViewModel. It is important that no changes are made directly to the ViewModel on a postback <em>before</em> the unbind is initiated. If changes are made to the ViewModel, say setting the value of a property, this value will be overwritten when you unbind. More importantly still, ensure that any collections are presented in the same order and contain the same number of items as they did when the initial bind took place. If you modified the ViewModel so that a collection contains less items then at initial bind, you are likely to receive <em>index out of range</em> exceptions. If the items are in a different order, then you may end up with corrupt and invalid data as values are mapped back on to the incorrect objects.</p>

<p>This caveat emptor applies to both <code>Persist</code> (stateful) and <code>Recreate</code> (stateless) state modes, but there is a greater risk with stateless binding as the responsibility for recreation of the ViewModel is placed in the hands of the page designer, whereas with stateful binding, some of this responsibility is assumed by the binder framework as it serializes the ViewModel to View State and deserializes it into an object on each post back. Although many will feel that stateful binding is not worth the sacrifice due to View State size considerations and risk of concurrency issues (in a lot of situations, I would agree), it does minimise exposure to the potential issues described here.</p>

<h2>Background</h2>

<p>From the very outset, two-way databinding support in ASP.NET Web Forms has been poor. Over time, a number of solutions have evolved (keep reading to find out about some of these). Unfortunately, all of them have had significant limitations or have only worked when utilised in a proscribed manner.</p>

<p>Anyone moving from a Silverlight or WPF project to Web Forms will find themselves having to re-adjust their thinking somewhat from a stateful world to a stateless one. This transition forces a number of concessions, one of the most glaring of which is this absence of a rich and flexible two-way databinding model as supplied by the former frameworks. The powerful data binding support supplied by WPF also compliments using the MVVM pattern to such a degree that the combination of Databinding + MVVM has become the de facto pattern used to develop UI applications with WPF and Silverlight. Many people will agree that the inability to use this pattern when designing Web Forms pages feels like a real step backwards after spending anytime with WPF or Silverlight.</p>

<p>The aim of the proposed framework is to address the lack of flexible and powerful two-way data binding in ASP.NET Web Forms by allowing for a WPF-esque declarative syntax to be used which at the same time allows UI development using the MVVM pattern.</p>

<p>Please be aware that throughout this article the term &quot;bind&quot; is used to describe the action of displaying data from ViewModel/source on the screen. The term &quot;unbind&quot; is used to describe the reverse of this process: extracting the user input data from the control and mapping it back to the ViewModel.</p>

<h3>References/Concepts</h3>

<p>Some of the concepts I have assumed you are familiar with:</p>

<ul>
<li><a href="http://www.codeproject.com/KB/WPF/BeginWPF5.aspx">WPF Data Binding</a></li>

<li><a href="http://msdn.microsoft.com/en-us/magazine/dd419663.aspx">MVVM</a></li>

<li><a href="http://www.codeproject.com/KB/silverlight/mvvm-explained.aspx">More MVVM</a><a></a><a> </a></li>

<li><a></a><a href="Mastering_DataBinding.aspx">Mastering ASP.NET DataBinding</a></li>

<li><a href="http://blogs.imeta.co.uk/jyoung/archive/2010/04/06/848.aspx">INotifyPropertyChanged</a></li>

<li><a href="http://msdn.microsoft.com/en-us/library/system.windows.input.icommand.aspx">ICommand</a></li>

<li><a href="http://consultingblogs.emc.com/dariuscollins/archive/2009/11/23/using-delegate-commands-with-wpf-views.aspx">DelegateCommand</a></li>
</ul>

<h3>Current Solutions</h3>

<table class="ArticleTable">
<thead>
<tr>
<th>Solution</th>

<th>Main Drawback</th>
</tr>
</thead>

<tbody>
<tr>
<td><a href="http://msdn.microsoft.com/en-us/magazine/cc163505.aspx">Subclass all controls</a></td>

<td>You have to subclass every control which needs to support two-way binding!</td>
</tr>

<tr>
<td><a href="http://davidhayden.com/blog/dave/archive/2005/05/25/1051.aspx">Data Source controls and GridView, FormView, DetailsView</a></td>

<td>You're limited to using the listed controls. Factory methods/CRUD methods and parameter mappings required.</td>
</tr>

<tr>
<td><a href="manubindingmanager.aspx">Use Visual Studio Designer to create bindings at design time</a></td>

<td>No runtime support. You must use the Visual Studio designers.</td>
</tr>

<tr>
<td><a href="http://www.nesterovsky-bros.com/weblog/CategoryView,category,ASPNET.aspx">Extender Controls for each binding</a></td>

<td>Each binding requires a binding extension control which can lead to bloated ASPX files.</td>
</tr>

<tr>
<td><a href="ASPNetTwoWayDataBinding.aspx">Parsing ASP.NET source files at runtime</a></td>

<td>Limited when binding across MasterPages/ContentPages and UserControls as you are reading the source from the file system.</td>
</tr>

<tr>
<td><a href="http://www.sourcebank.com/DevX/Article/35058">Binding Manager</a></td>

<td>No inline (ASPX) declarative binding.</td>
</tr>

<tr>
<td><a href="http://www.sourcebank.com/DevX/Article/35058">By hand</a></td>

<td>Labour intensive, verbose, accident prone, poor maintainability, code bloat.</td>
</tr>
</tbody>
</table>

<p><strong>N.B.</strong> The above list of solutions for providing Web Forms with two-way data binding support is by no means exhaustive, but I do feel that it covers some of the more common methods used.</p>

<p>You may decide you prefer one of the above options to the proposed framework. I've listed them for this very reason, different scenarios call for different solutions and it pays to be aware of what's around. All of the above will work fine and will fit into various architectural designs, but I feel that the proposed framework offers some benefits. I hope that by explaining the approach I have taken with this framework, I will convince you of the same. If you feel that a deeper examination of the available two-way databinding methods would be of benefit, then please leave a comment and I will consider expanding on the merits and drawbacks of the methods listed above, but I do feel confident that if you've spent time doing databinding the WPF way, then you'll immediately understand how the approach I've taken can be of benefit.</p>

<h2>Design Tenants</h2>

<p>The following are a list of ideals which I have tried to adhere to whilst developing this framework:</p>

<ul>
<li>No page base class - In order to allow easy integration of this framework with existing frameworks, a number of which require you to inherit your Pages from a base class, it was decided that we would not require this.</li>

<li>Minimise wire-up code - A key goal was to keep the amount of wiring required to a minimum. Taken hand-in-hand with the &quot;no page base class&quot; goal, this required careful design and implementation.</li>

<li>Minimise code-behind - Eliminate, as much as possible, the need for any code-behind. The ability to implement the UI entirely declaratively was a key goal.</li>

<li>Facilitate MVVM in ASP.NET - Full support for command binding and two-way databinding.</li>

<li>Mimic WPF - Allow the use of WPF binding features such as <code>IValueConverter</code>s, Binding Modes, Resources, and expressive, declarative binding statements.</li>

<li>Suppress Exceptions - As with WPF, databinding errors should not cause your application to throw an Exception (partialy realised, see: &quot;How far along are we?&quot;).</li>
</ul>

<h3>Solution Overview</h3>

<table class="ArticleTable">
<thead>
<tr>
<th>Project</th>

<th>Description</th>
</tr>
</thead>

<tbody>
<tr>
<td>Framework/Binding</td>

<td>This is the core framework assembly. It contains all the essential non-platform specific framework components.</td>
</tr>

<tr>
<td>Framework/ASPBinding</td>

<td>ASP.NET specific framework components.</td>
</tr>

<tr>
<td>FrameworkTests</td>

<td>Unit tests.</td>
</tr>

<tr>
<td>DomainModel</td>

<td>Business objects used by the examples and demos.</td>
</tr>

<tr>
<td>BindingExamples</td>

<td>Example and demo library - this contains a fair number of simple and more advanced examples, and should be considered the main reference for further exploration of this framework.</td>
</tr>

<tr>
<td>Barebones</td>

<td>The bare-minimum &quot;hello world&quot; example of using this framework.</td>
</tr>
</tbody>
</table>

<h3>Notes on Reusing WPF Assemblies</h3>

<p>I had to make a decision on whether to reuse the classes supplied by WPF or recreate them from scratch. This mainly applies to <code>ICommand</code> and <code>ObservableCollection</code> but is likely to involve others as the framework expands. The disadvantage of reuse is a dependency on the WPF assemblies. The disadvantage of not using them is the duplication of objects with the same purpose, and more importantly with issues regarding portability of code (such as Command and ViewModel code) between platforms. I decided to reuse in the end because I couldn't see any practical harm, although purists may argue this point.</p>

<h2>How Far Along Are We?</h2>

<p>I'd like to stress that the code supplied isn't a finished product. It's probably a version 0.3 at most, but I've got to the stage where before I invest any more time I feel it would be useful to generate some feedback. Maybe (although I hope not) I'm totally missing the point and there's a reason why this approach hasn't been tried (or shared) before. Perhaps there are things you don't like, some things you do. <strong>Let me know!</strong></p>

<p>In addition to the features listed in <strong>...and some that didn't</strong>, there are a number of areas that require further development:</p>

<ul>
<li>The performance could be better, it's not telling currently, but once scaled, might be. I am aware of quite a few areas where performance of the framework could be improved.</li>

<li>There is currently a limitation when binding programmatically, which really is best demonstrated by example. There is a test which demonstrates the issue in the test suite: <code>NestedCollectionRelativePathTest</code>.</li>

<li>The test coverage of the framework needs to be expanded significantly.</li>

<li>Error handling needs improving as well, the design goal of: bindings not throwing exceptions under any circumstances, hasn't been implemented, but my excuse is that with exceptions, the code is easier to debug whilst the framework is still in a development stage....although this could be (and probably is) a cop out.</li>
</ul>
