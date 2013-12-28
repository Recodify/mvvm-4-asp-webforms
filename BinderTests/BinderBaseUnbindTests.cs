using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinderTests.MockingInterfaces;
using Binding.Services;
using Binding;
using Rhino.Mocks;
using System.Web.UI.WebControls;
using ASPBinding.ControlWrappers;
using System.Web.UI;
using Binding.ControlWrappers;
using BinderTests.MockHelpers;
using BinderTests.Utils;

namespace BinderTests
{
    [TestClass]
    public class BinderBaseUnbindTests
    {

        //Used internally be the binder - cheating slightly but nevermind
        private const string DATA_BINDING_KEY = "DATA_BINDINGS";
        private const string COMMAND_BINDING_KEY = "COMMAND_BINDINGS";
        private const string BINDING_SEQUENCE_MANAGER = "BINDING_SEQUENCE_MANAGER";


        private IServiceProviderBindingContainer mockContainer = null;
        private IDataStorageService dataStorageService = null;
        private IControlService controlService = null;
        private SimpleViewModel viewModel = null;


        //object under test
        private ViewStateBinder binder = null;
        //used by the mock data service as a backing store
        private Binding.BindingCollection collection = null;

        [TestInitialize()]
        public void SetupForTest()
        {
       
            mockContainer = MockRepository.GenerateMock<IServiceProviderBindingContainer>();
            dataStorageService = MockRepository.GenerateMock<IDataStorageService>();
            controlService = MockRepository.GenerateMock<IControlService>();
            viewModel = new SimpleViewModel();
         
            //object under test
            binder = new ViewStateBinder(mockContainer, dataStorageService, controlService);

            mockContainer.Expect(mc => mc.IsPostBack).IgnoreArguments().Return(true);
            mockContainer.Expect(mc => mc.DataContext).IgnoreArguments().Return(viewModel);
           
        }

        [TestMethod]
        public void TextBoxUnbindTest()
        {
            /*Arrange*/
            TextBox tb1 = SetupTextboxTest(BindingMode.TwoWay);

            /*Act*/
            binder.ExecuteUnbind();

            /*Assert*/
            Assert.AreEqual(tb1.Text, viewModel.EmployeeName);
        }

        /// <summary>
        /// This tests a  oneway unbinding
        /// <remarks>The view model should not be updated!</remarks>
        /// </summary>
        [TestMethod]
        public void TextBoxOneWayUnbindTest()
        {
            /*Arrange*/
            TextBox tb1 = SetupTextboxTest(BindingMode.OneWay);

            /*Act*/
            binder.ExecuteUnbind();

            /*Assert*/
            Assert.AreNotEqual(tb1.Text, viewModel.EmployeeName);
        }

       

        /*If multiple controls share the same binding source, the one added last should by default be authorative*/
        [TestMethod]
        public void DefaultAuthorityIsLastBindingTest()
        {
            Tuple<TextBox, TextBox> tbs = SetupMultipleTextboxTest(new Options { Mode = BindingMode.TwoWay }, new Options { Mode = BindingMode.TwoWay });

            binder.ExecuteUnbind();

            Assert.AreEqual(tbs.Item2.Text, viewModel.EmployeeName);
        }

        [TestMethod]
        /*If multiple controls share the same binding source, and the authorative flag is set on one of the bindings, its value is used*/
        public void ExplicitAuthorityTest()
        {
            Tuple<TextBox, TextBox> tbs = SetupMultipleTextboxTest(new Options { Mode = BindingMode.TwoWay, IsAuthorative = true }, new Options { Mode = BindingMode.TwoWay });

            binder.ExecuteUnbind();

            Assert.AreEqual(tbs.Item1.Text, viewModel.EmployeeName);
        }

        [TestMethod]
        /*If multiple controls share the same binding source, and the authorative flag is set on more than one of the bindings, the first one wins*/
        public void ExplicitAuthorityLastAuthorativeFlagWinsTest()
        {
            Tuple<TextBox, TextBox> tbs = SetupMultipleTextboxTest(new Options { Mode = BindingMode.TwoWay, IsAuthorative = true }, new Options { Mode = BindingMode.TwoWay, IsAuthorative = true });

            binder.ExecuteUnbind();

            Assert.AreEqual(tbs.Item1.Text, viewModel.EmployeeName);
        }


        [TestMethod]
        public void CollectionTwoWayTest()
        {
            /*Arrange*/
            List<Control> controls = SetupCollectionUnbindTest(BindingMode.TwoWay);

            //update the value of the controls
            for (int i = 0; i < controls.Count; i++)
            {
                ((TextBox)controls[i]).Text = i.ToString();
            }

            /*Act*/
            binder.ExecuteUnbind();

            /*Assert*/
            for (int i = 0; i < controls.Count; i++)
            {
                TextBox tb = controls[i] as TextBox;
                Assert.AreEqual(tb.Text, viewModel.ChildrensNames[i].Name);
            }
        
        }
        

        [TestMethod]
        public void CollectionOneWayTest()
        {
            /*Arrange*/
            List<Control> controls = SetupCollectionUnbindTest(BindingMode.OneWay);

            //update the value of the controls
            for (int i = 0; i < controls.Count; i++)
            {
                ((TextBox)controls[i]).Text = i.ToString();
            }

            /*Act*/
            binder.ExecuteUnbind();

            /*Assert*/
            for (int i = 0; i < controls.Count; i++)
            {
                TextBox tb = controls[i] as TextBox;
                Assert.AreNotEqual(tb.Text, viewModel.ChildrensNames[i].Name);
            }
        }


        private TextBox SetupTextboxTest(BindingMode direction)
        {
            return SetupTextboxTest(new Options { Mode = direction });
        }


        private Tuple<TextBox,TextBox> SetupMultipleTextboxTest(Options options1, Options options2)
        {
            /*Arrange*/ 
            TextBox tb1 = new TextBox();
            tb1.Text = "Data From tb1";
            tb1.ID = "tb1";
            WebformControl wrappedTb = new WebformControl(tb1);

            TextBox tb2 = new TextBox();
            tb2.Text = "Data From tb2";
            tb2.ID = "tb2";
            WebformControl wrappedTb2 = new WebformControl(tb2);

            //Create a binding collection, add a binding
            options1.Path = "EmployeeName";
            collection = new Binding.BindingCollection() { new Binding.BindingDef(wrappedTb, "Text", options1, false, controlService) };

            options2.Path = "EmployeeName";
            collection.Add(new Binding.BindingDef(wrappedTb2, "Text", options2, false, controlService) );


            //and setup service to return it
            int count = 0;
            controlService.Expect(cs => cs.Unwrap(null)).Do(new Func<IBindingTarget, object>((t) =>
                {
                    if (count == 0)
                    {
                        count++;
                        return tb1;
                    }
                    else
                        return tb2;
                }));

            //setup the data storage service to return the collection
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(DATA_BINDING_KEY)).Return(collection);
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(COMMAND_BINDING_KEY)).Return(new Binding.BindingCollection());

            //and the viewmodel (we are using the view state binder which will try to resolve the service from storage
            dataStorageService.Expect(ds => ds.Retrieve<object>(null)).IgnoreArguments().Return(viewModel);

            return new Tuple<TextBox, TextBox> { Item1 = tb1, Item2 = tb2 };
        }

        private TextBox SetupTextboxTest(Options options)
        {
            /*Arrange*/
            TextBox tb1 = new TextBox();
            tb1.Text = "Data From view";
            tb1.ID = "tb1";
            WebformControl wrappedTb = new WebformControl(tb1);

            //Create a binding collection, add a binding
            options.Path = "EmployeeName";
            collection = new Binding.BindingCollection() { new Binding.BindingDef(wrappedTb, "Text", options, false, controlService) };

            //and setup service to return it
            controlService.Expect(cs => cs.Unwrap(null)).Do(new Func<IBindingTarget, object>((t) => tb1));

            //setup the data storage service to return the collection
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(DATA_BINDING_KEY)).Return(collection);
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(COMMAND_BINDING_KEY)).Return(new Binding.BindingCollection());

            //and the viewmodel (we are using the view state binder which will try to resolve the service from storage
            dataStorageService.Expect(ds => ds.Retrieve<object>(null)).IgnoreArguments().Return(viewModel);

            return tb1;
        }

        private List<Control> SetupCollectionUnbindTest(BindingMode direction)
        {
            return SetupCollectionUnbindTest(new Options { Mode = direction });
        }
        
        private List<Control> SetupCollectionUnbindTest(Options options)
        {

            List<Control> createdControls = new List<Control>();
            
            //Create a repeater from which we will read data
            Repeater rpt = new Repeater();
            rpt.ItemTemplate = new DynamicItemTemplate(new List<Func<Control>>{()=> new TextBox(){ID = "tb"}});
            var item1 = new Child{ Name = "Sam"};
            var item2 = new Child{ Name = "Ben"};
            var item3 = new Child{ Name = "Ali"};
            var list = new List<Child>{ item1, item2, item3};

            viewModel.ChildrensNames = list;

            //Create a binding collection
            collection = new Binding.BindingCollection();

            //add in the parent binding, this will not be used in the bind but is required for unbind to work within a collection
            collection.Add(new Binding.BindingDef(new WebformControl(rpt),"DataSource", new Options{Path="ChildrensNames"},true, controlService));

            //When the item is created, create a binding for it and add to the binding collection.
            //We will also had the itemtemplate create control to a collection so that we can stick some values
            //in to simulate user input before unbinding to the model.
            int index = 0;
            rpt.ItemDataBound += (s, e) =>
                {
                   TextBox tb = e.Item.FindControl("tb") as TextBox;
                   WebformControl wrappedTb = new WebformControl(tb);
                   options.Path = "Name";
                   Binding.BindingDef binding = new Binding.BindingDef(wrappedTb, "Text",index,collection, options,false, controlService);
                   index++;
                   collection.Add(binding);
                   createdControls.Add(tb);
                };

            rpt.DataSource = list;
            

            WebformControl dummyControl = new WebformControl(new TextBox());

            //tell the mock conntrol service to return the collection result when required.
            this.controlService.Expect(cs => cs.FindControlUnique(null, null)).IgnoreArguments()
                .Do(
                    new Func<IBindingTarget, string, IBindingTarget>((b, s) =>
                    {
                         //we only need it return a non-null
                         return dummyControl;
                    }));


            int counter = 0;

            //and setup service to return it
            controlService.Expect(cs => cs.Unwrap(null)).IgnoreArguments().Do(new Func<IBindingTarget, object>((t) => 
            {
                Control ctrl = createdControls[counter];
                counter++;
                return ctrl;
            }));


            //setup viewmodel with default values
            viewModel.ChildrensNames = new List<Child> { new Child(), new Child(), new Child() };

            //setup the data storage service to return the collection
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(DATA_BINDING_KEY)).Return(collection);
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(COMMAND_BINDING_KEY)).Return(new Binding.BindingCollection());

            //and the viewmodel (we are using the view state binder which will try to resolve the service from storage
            dataStorageService.Expect(ds => ds.Retrieve<object>(null)).IgnoreArguments().Return(viewModel);

            rpt.DataBind();

            return createdControls;
            
        }

    }
}
