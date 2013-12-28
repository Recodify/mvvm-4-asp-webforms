using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Binding.Services;
using BinderTests.MockingInterfaces;
using Binding;
using Rhino.Mocks;
using ASPBinding.Services;
using System.Web.UI;
using Binding.Interfaces;

namespace BinderTests
{
    [TestClass]
    public class ContextRetrievalTests
    {

        private const string VIEW_MODEL_KEY = "VIEW_MODEL";

        private IBindingContainer mockContainer = null;
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
            dataStorageService = new ViewStateStorageService(new StateBag());
            controlService = MockRepository.GenerateMock<IControlService>();
           
            //object under test
            binder = new ViewStateBinder(mockContainer, dataStorageService, controlService);

        }

        [TestMethod]
        public void ContextIsConsistentBetweenGetsTest()
        {
            viewModel = new SimpleViewModel() { EmployeeName = "Dave Smith" };
            //view supplies initial state
            mockContainer.Expect(mc => mc.DataContext).IgnoreArguments().Return(viewModel);

            //will store in datastorage
            object binderContext = binder.DataContext;
            Assert.IsNotNull(dataStorageService.Retrieve<object>(VIEW_MODEL_KEY));

            //set is post back to true
            mockContainer.Expect(mc => mc.IsPostBack).IgnoreArguments().Return(true);

            //retreive = should get the vm from storage
            SimpleViewModel vm = binder.DataContext as SimpleViewModel;
            vm.EmployeeName = "Sam Shiles";

            SimpleViewModel vm2 = binder.DataContext as SimpleViewModel;
         
            Assert.AreEqual(vm.EmployeeName, vm2.EmployeeName);
        }
    
    }
}
