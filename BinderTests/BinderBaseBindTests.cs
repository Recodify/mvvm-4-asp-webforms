using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Binding;
using Rhino.Mocks;
using BinderTests.MockingInterfaces;
using Binding.Services;
using System.Web.UI.WebControls;
using ASPBinding.ControlWrappers;
using BinderTests.MockHelpers;
using System.Web.UI;
using Binding.ControlWrappers;

namespace BinderTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class BinderBaseBindTests
    {
        private IServiceProviderBindingContainer mockContainer = null;
        private IDataStorageService dataStorageService = null;
        private IControlService controlService = null;
        private ISimpleViewModel viewModel = null;
        
        
        //object under test
        private ViewStateBinder binder = null;
        //used by the mock data service as a backing store
        private Binding.BindingCollection collection = null;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
       

        [TestInitialize()]
        public void SetupForTest()
        {
            mockContainer = MockRepository.GenerateMock<IServiceProviderBindingContainer>();
            dataStorageService = MockRepository.GenerateMock<IDataStorageService>();
            controlService = MockRepository.GenerateMock<IControlService>();
            viewModel = MockRepository.GenerateMock<ISimpleViewModel>();
            mockContainer.Expect(c => c.DataContext).IgnoreArguments().Return(viewModel);
            
            //object under test
            binder = new ViewStateBinder(mockContainer, dataStorageService, controlService);

            //setup mock services
            collection = new Binding.BindingCollection();
            dataStorageService.Expect(ds => ds.RetrieveOrCreate<Binding.BindingCollection>(null)).IgnoreArguments().Return(collection);

            //dataStorageService.Expect(ds => ds.Retrieve<object>(null)).IgnoreArguments()Return(viewModel);
            
        }

        /// <summary>
        /// This test the creation of a binding when used against a collection
        /// </summary>
        [TestMethod]
        public void VerifyRepeaterProgrammaticBindCreatesCorrectBindingObject()
        {
            /*Arrange*/
            List<Child> children = new List<Child> { new Child { Name = "Ben" }, new Child { Name = "Sam" }, new Child { Name = "Dave" }, new Child { Name = "Ali" } };
            this.viewModel.Expect(vm => vm.ChildrensNames).Return(children).Repeat.Once();

            Repeater rpt = new Repeater();
            Options options = new Options { Path = "ChildrensNames" };

            /*Act*/
            object resolvedValue = this.binder.ExecuteBind(new WebformControl(rpt), options, "DataSource");
            
            /*Assert*/
            Assert.AreEqual(children, resolvedValue);

            Binding.BindingDef binding = this.collection[0];
            Assert.IsTrue(binding.IsSourceEnumerable);

            //collection bindings should always be oneway.
            Assert.AreEqual(BindingMode.OneWay, binding.BindingOptions.Mode);

            //Assert that control service has been passed to the binding
            Assert.IsNotNull(binding.ControlService);
            Assert.AreEqual(this.controlService, binding.ControlService);

            //Assert that the binding has created target and source expression objects
            Assert.IsNotNull(binding.SourceExpression);
            Assert.IsNotNull(binding.TargetExpression);

            Assert.IsFalse(binding.HasValueConverter);
        }

        /// <summary>
        /// This test the creation of a binding when used against a non collection
        /// </summary>
        [TestMethod]
        public void VerifyTextboxTextProgrammaticBindCreatesCorrectBindingObject()
        {
            /*Arrange*/
            //We expect at least one call
            this.viewModel.Expect(vm => vm.EmployeeName).Return("Sam Shiles").Repeat.Once();

            TextBox tb = new TextBox();
            Options options = new Options { Path = "EmployeeName" };

            /*Act*/
            object resolvedValue = this.binder.ExecuteBind(new WebformControl(tb), options, "Text");

            /*Assert*/
            //Assert on the binding collection to make sure that the binding has been created successfully
            Assert.IsNotNull(collection);
            Assert.AreEqual(1, collection.Count);

            Binding.BindingDef binding = this.collection[0];
            //Assert the options have been stored on the binding correctly
            Assert.AreEqual(binding.BindingOptions, options);
            Assert.IsTrue(!binding.IsSourceEnumerable);
            
            //Assert that control service has been passed to the binding
            Assert.IsNotNull(binding.ControlService);
            Assert.AreEqual(this.controlService, binding.ControlService);

            //Assert that the binding has created target and source expression objects
            Assert.IsNotNull(binding.SourceExpression);
            Assert.IsNotNull(binding.TargetExpression);
            
            Assert.IsFalse(binding.HasValueConverter);

        }

        [TestMethod]
        public void VerifyTextboxTextProgrammaticBindReturnsCorrectValue()
        {

            /*Arrange*/
            //We expect at least one call
            this.viewModel.Expect(vm => vm.EmployeeName).Return("Sam Shiles").Repeat.Once();

            TextBox tb = new TextBox();
            Options options = new Options { Path = "EmployeeName" };

            /*Act*/
            object resolvedValue = this.binder.ExecuteBind(new WebformControl(tb), options, "Text");

            /*Assert*/
            //Assert on the return value to verify the correct value has been retreived from the model.
            Assert.IsNotNull(resolvedValue);
            Assert.IsTrue(resolvedValue.GetType() == typeof(string));
            Assert.AreEqual("Sam Shiles", (string)resolvedValue);
            
        }

        /// <summary>
        /// Getting this sort of thing to work in isolation and with programatically created controls
        /// requires a bit of work but this should do it. Its much easier in a real world scenario with declartive binding
        /// and asp.net taking care of calling the databind methods.
        /// </summary>
        [TestMethod]
        public void VerifyTextboxInRepeaterProgrammaticBindReturnsCorrectValue()
        {
            /*Arrange*/
            List<Child> children = new List<Child> { new Child{Name="Ben"}, new Child{Name="Sam"}, new Child{Name="Dave"}, new Child{Name="Ali"} };
            this.viewModel.Expect(vm => vm.ChildrensNames).Return(children).Repeat.Once();

            //tell the mock conntrol service to return the collection result when required.
            this.controlService.Expect(cs => cs.FindControlUnique(null, null)).IgnoreArguments()
                .Do(
                    new Func<IBindingTarget,string,IBindingTarget>((b,s) =>
                    {
                        if (s.EndsWith("tb1"))
                        {
                            //we only need it return a non-null
                            return new WebformControl(new  TextBox());
                        }
                        return null;
                    }));

            
            //create a repeater and set its item template
            Repeater rpt = new Repeater();
            rpt.ItemTemplate = new DynamicItemTemplate(new List<Func<Control>>
                    {
                        () => new TextBox(){ ID = "tb1" }
                    });
            
            
            /*Act*/
            Options options = new Options { Path = "ChildrensNames" };
            object resolvedValue = this.binder.ExecuteBind(new WebformControl(rpt), options, "DataSource");
            rpt.DataSource = resolvedValue;
            
            //When the repeater item is created bind up child controls and test the values returned from the binder
            rpt.ItemDataBound += (s, e) =>
                {
                    RepeaterItem rptItem = e.Item;
                    TextBox rptItemControl = rptItem.FindControl("tb1") as TextBox;
                    Options textBoxBindOptions = new Options { Path = "Name" };
                    object tbResolvedValue = this.binder.ExecuteBind(new WebformControl(rptItemControl), new WebformControl<IDataItemContainer>(rptItem), textBoxBindOptions, "Text");
                    Assert.AreEqual(children[rptItem.ItemIndex].Name, tbResolvedValue);
                };

            rpt.DataBind();

            /*Assert*/
            //verify we have an repeater item for each item passed in on the datasource
            Assert.AreEqual(children.Count, rpt.Items.Count);
            
            //we should have one binding for each child control (4) plus the binding the on the parent(rpt)
            Assert.AreEqual((children.Count + 1),this.collection.Count);

            Binding.BindingDef parentBinding = collection[0];
            Assert.IsTrue(parentBinding.IsSourceEnumerable);
            Assert.IsNotNull(parentBinding.Container);
            Assert.AreEqual(rpt, ((WebformControl)parentBinding.Container).Control);

            //test the child bindings
            for (int i = 1; i < collection.Count; i++)
            {
                
                Binding.BindingDef childBinding = collection[i];
                Assert.IsTrue(childBinding.Parent == parentBinding);
                Assert.AreEqual(i-1, childBinding.DataItemIndex);
                Assert.IsFalse(childBinding.IsSourceEnumerable);
                
            }

        }

        [TestCleanup()]
        public void TestCleanup()
        {
            viewModel.VerifyAllExpectations();
            mockContainer.VerifyAllExpectations();
            dataStorageService.VerifyAllExpectations();
            controlService.VerifyAllExpectations();
        }
    }
}
