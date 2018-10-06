using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Binding;
using Binding.Services;
using Rhino.Mocks;
using System.Web.UI.WebControls;
using ASPBinding.ControlWrappers;
using Binding.ControlWrappers;

namespace BinderTests
{
    /// <summary>
    /// Path storage, nesting and object resolution tests
    /// </summary>
    [TestClass]
    public class BindingPathTests
    {

        private IControlService controlService = null;
        
        [TestInitialize()]
        public void SetupForTest()
        {
           
            controlService = MockRepository.GenerateMock<IControlService>();
        }

        [TestMethod]
        public void SimplePathStorageTest()
        {
            string rawPath = "SelectedEmployee.Name";
            BindingPath path = new BindingPath(rawPath);

            Assert.AreEqual(rawPath, path.Raw);
            Assert.AreEqual(path.Raw, path.FullyQualified);
            Assert.AreEqual(path.Raw, path.Indexed);

            //Default mode, non other specfied
            Assert.AreEqual(PathMode.Relative, path.Mode);
        }

        [TestMethod]
        public void SimplePathSourceResolutionTest()
        {
            string rawPath = "SelectedEmployee.Name";
            BindingPath path = new BindingPath(rawPath);

            object selectedEmployee = new { Name = "Sam" };
            SourceProperty sourceProperty = path.ResolveAsSource(new { SelectedEmployee =  selectedEmployee});

            Assert.IsNotNull(sourceProperty);
            Assert.IsNotNull(sourceProperty.Descriptor);
            Assert.IsNotNull(sourceProperty.OwningInstance);
            Assert.IsNotNull(sourceProperty.Value);
            Assert.AreEqual("Sam", sourceProperty.Value);
            Assert.AreEqual("Name", sourceProperty.Descriptor.Name);
            Assert.AreEqual(selectedEmployee, sourceProperty.OwningInstance);
        }

        [TestMethod]
        public void IndexedPathStorageTest()
        {
            string rawPath = "AvailableEmployees.Name";
            BindingPath path = new BindingPath(rawPath, 1);

            Assert.AreEqual(rawPath, path.Raw);
            Assert.AreNotEqual(path.Raw, path.FullyQualified);
            Assert.AreEqual(path.Indexed, path.FullyQualified);
        }

        [TestMethod]
        public void IndexedPathSourceResolutionTest()
        {
            string rawPath = "AvailableEmployees.Name";
            BindingPath path = new BindingPath(rawPath, 1);

            var employee1 = new { Name = "Sam" };
            var employee2 = new { Name = "Ben" };
            var employee3 = new { Name = "Ali" };

            var list = new[] { employee1, employee2, employee3};
            var viewModel = new { AvailableEmployees = list };
            
            SourceProperty sourceProperty = path.ResolveAsSource(viewModel);

            Assert.IsNotNull(sourceProperty);
            Assert.IsNotNull(sourceProperty.Descriptor);
            Assert.IsNotNull(sourceProperty.OwningInstance);
            Assert.IsNotNull(sourceProperty.Value);
            Assert.AreEqual("Ben", sourceProperty.Value);
            Assert.AreEqual("Name", sourceProperty.Descriptor.Name);
            
            //empoyee at the index of 1
            Assert.AreEqual(employee2 , sourceProperty.OwningInstance);
            Assert.AreEqual(employee2.Name, sourceProperty.Value);
            Assert.AreEqual("Name", sourceProperty.Descriptor.Name);
        }

        [TestMethod]
        public void SimplePathTargetResolutionTest()
        {
            string rawPath = "tb1.Text";
            BindingPath path = new BindingPath(rawPath);

            TextBox tb = new TextBox();
            tb.ID = "tb1";
            tb.Text = "hello";

            //tell the control service to return our control when asked
            controlService.Expect(cs => cs.FindControlUnique(null, null)).IgnoreArguments()
                .Do(new Func<IBindingTarget, string, IBindingTarget>((s, e) => new WebformControl(tb)));

            //and unwrap
            controlService.Expect(cs => cs.Unwrap(null)).IgnoreArguments().Do(new Func<IBindingTarget,object>(b => tb));

            TargetProperty targetProperty = path.ResolveAsTarget(new WebformControl(tb), this.controlService);
            Assert.IsNotNull(targetProperty);
            Assert.IsNotNull(targetProperty.Descriptor);
            Assert.IsNotNull(targetProperty.OwningControlRaw);
            Assert.IsNotNull(targetProperty.Value);

            Assert.AreEqual("hello", targetProperty.Value);
            Assert.AreEqual("Text", targetProperty.Descriptor.Name);
            Assert.AreEqual(tb, targetProperty.OwningControlRaw);
        }

        [TestMethod]
        public void NestedRelativePathTest()
        {
            /*Arrange*/
            Binding.BindingDef parent = new Binding.BindingDef(
                new WebformControl(new TextBox()),
                "Text", 
                new Options() { Path = "Company" },
                controlService);

            string rawPath = "AvailableEmployees.Name";
            
            BindingPath path = new BindingPath(rawPath,2,parent.SourceExpression,PathMode.Relative);

            var employee1 = new { Name = "Sam" };
            var employee2 = new { Name = "Ben" };
            var employee3 = new { Name = "Ali" };

            var list = new[] { employee1, employee2, employee3};
            var company = new { AvailableEmployees = list };

            var viewModel = new { Company = company};

            /*Act*/
            SourceProperty sourceProperty = path.ResolveAsSource(viewModel);

            /*Assert*/
            Assert.IsNotNull(sourceProperty);
            Assert.IsNotNull(sourceProperty.Descriptor);
            Assert.IsNotNull(sourceProperty.OwningInstance);
            Assert.IsNotNull(sourceProperty.Value);
            Assert.AreEqual("Ali", sourceProperty.Value);
            Assert.AreEqual("Name", sourceProperty.Descriptor.Name);

            //empoyee at the index of 1
            Assert.AreEqual(employee3, sourceProperty.OwningInstance);
            Assert.AreEqual(employee3.Name, sourceProperty.Value);
            Assert.AreEqual("Name", sourceProperty.Descriptor.Name);
        }

        /*This is example of the work that still needs to be done on programatic binding. Currently when creating a binding object for an item in collection 
         a DataItemIndex is passed in. This index is actually for the collection, not the child. So for example if I create a binding for a Bike and I want to bind to the
         * wheelsize property I will pass in a DataItemIndex of 1. This will lead to the path BikeList[1].WheelSize.
         * I will not pass in the index when creating the binding on the parent (collection). This is due to the way the binding framework hooks into the asp.net binding cycle.
         * Unfortunatly this goes against the intuitive (shown below). As a result, this test will fail. Programtic binding is supported in this version but not fully.
         */
       // [ExpectedException(typeof)
        [TestMethod]
        public void NestedCollectionRelativePathTest()
        {
            /*Arrange*/
            
            //create a parent binding.
            Binding.BindingDef parent = new Binding.BindingDef(
                new WebformControl(new TextBox())
                , "Text", 0
                , new BindingCollection()
                , new Options { Path = "Companies" }
                , true, controlService);


            string rawPath = "AvailableEmployees.Name";

            BindingPath path = new BindingPath(rawPath, 2, parent.SourceExpression, PathMode.Relative);

            var employee1 = new { Name = "Sam" };
            var employee2 = new { Name = "Ben" };
            var employee3 = new { Name = "Ali" };

            var list = new[] { employee1, employee2, employee3 };
            var company = new { AvailableEmployees = list };

            var companyList = new[] { company };

            var viewModel = new { Companies = companyList };

            /*Act*/
            SourceProperty sourceProperty = path.ResolveAsSource(viewModel);

            /*Assert*/
            Assert.IsNotNull(sourceProperty.Descriptor);
         
        }

        [TestMethod]
        public void NestedAbsolutePathTest()
        {
            /*Arrange*/

            //create a parent binding.
            Binding.BindingDef parent = new Binding.BindingDef(
                new WebformControl(new TextBox())
                , "Text", 0
                , new BindingCollection()
                , new Options { Path = "Companies" }
                , true, controlService);


            string rawPath = "CompanyName";

            BindingPath path = new BindingPath(rawPath, parent.SourceExpression, PathMode.Absolute);

            var employee1 = new { Name = "Sam" };
            var employee2 = new { Name = "Ben" };
            var employee3 = new { Name = "Ali" };

            var list = new[] { employee1, employee2, employee3 };
            var company = new {  AvailableEmployees = list };

            var companyList = new[] { company };

            var viewModel = new { CompanyName = "Tesco Ltd.", Companies = companyList };

            /*Act*/
            SourceProperty sourceProperty = path.ResolveAsSource(viewModel);

            /*Assert*/
            Assert.IsNotNull(sourceProperty);
            Assert.IsNotNull(sourceProperty.Descriptor);
            Assert.IsNotNull(sourceProperty.OwningInstance);
            Assert.IsNotNull(sourceProperty.Value);
            Assert.AreEqual("CompanyName", sourceProperty.Descriptor.Name);

            Assert.AreEqual(viewModel, sourceProperty.OwningInstance);
            Assert.AreEqual(viewModel.CompanyName, sourceProperty.Value);
            
        }

        [TestMethod]
        public void NestedCollectionAbsolutePathTest()
        {
            /*Arrange*/

            //create a parent binding.
            /*Arrange*/
            Binding.BindingDef parent = new Binding.BindingDef(new WebformControl(new TextBox()), "", new Options { Path = "" }, controlService);
            string rawPath = "ViewModelID";

            BindingPath path = new BindingPath(rawPath, parent.SourceExpression, PathMode.Absolute);
            var viewModel = new {ViewModelID = "viewmodel1"};

            /*Act*/
            SourceProperty sourceProperty = path.ResolveAsSource(viewModel);

            /*Assert*/
            Assert.IsNotNull(sourceProperty);
            Assert.IsNotNull(sourceProperty.Descriptor);
            Assert.IsNotNull(sourceProperty.OwningInstance);
            Assert.IsNotNull(sourceProperty.Value);
            Assert.AreEqual("ViewModelID", sourceProperty.Descriptor.Name);
            Assert.AreEqual(viewModel, sourceProperty.OwningInstance);
            Assert.AreEqual(viewModel.ViewModelID, sourceProperty.Value);

        }
        
    }
}
