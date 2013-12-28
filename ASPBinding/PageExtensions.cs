using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using ASPBinding.Services;
using Binding.Services;
using Binding.ControlWrappers;
using ASPBinding.ControlWrappers;
using System.Reflection;
using System.Web.Configuration;
using Binding.Interfaces;
using Binding;

namespace Binding
{

    //TODO: These extensions are useful across platforms and shouldn't be asp specific. Going with this approach temporarily
    //whilst I sort out the service injection to the binders. 
    public static class BindingContainerExtensions
    {
        /// <summary>
        /// Create and execute a programmatic binding
        /// </summary>
        /// <param name="bindingContainer"></param>
        /// <param name="control">The control that should be bound</param>
        /// <param name="targetProperty">The property on the control to which to bind</param>
        /// <param name="path">Path to the source data</param>
        public static void CreateBinding(this IBindingContainer bindingContainer, Control control, string targetProperty, string path)
        {
            CreateBinding(bindingContainer, control, targetProperty, new Options { Path = path });
        }

        /// <summary>
        /// Create and execute a programmatic binding
        /// </summary>
        /// <param name="bindingContainer"></param>
        /// <param name="control">The control that should be bound</param>
        /// <param name="targetProperty">The property on the control to which to bind</param>
        /// <param name="bindingOptions">Binding options including the path to the data source</param>
        public static void CreateBinding(this IBindingContainer bindingContainer, Control control, string targetProperty, Options bindingOptions)
        {
            object value = BindingHelpers.Bind(control, bindingOptions, targetProperty);
            PropertyInfo pInfo = control.GetType().GetProperty(targetProperty);
            pInfo.SetValue(control, value, null);
        }

        public static StateBag GetStateBag(this IBindingContainer bindingContainer)
        {
            StateBag viewState = (StateBag)bindingContainer.GetType().GetProperty("ViewState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(bindingContainer, null);
            return viewState;
        }

        public static void RegisterForBinding(this IBindingContainer bindingContainer, StateMode stateMode)
        {
            BinderBase binder = BindingHelpers.CreateBinder(bindingContainer, stateMode);
        }

        public static void RegisterForBinding(this IBindingContainer bindingContainer)
        {
            BinderBase binder = BindingHelpers.CreateBinder(bindingContainer);
        }

        /// <summary>
        /// Call this to initiate Unbind
        /// </summary>
        /// <remarks>
        /// Only required if UpdateSourceTrigger is Explicit.
        /// DO NOT CALL BEFORE LOAD in the PLC
        /// </remarks>
        /// <param name="bindingContainer"></param>
        public static void Unbind(this IBindingContainer bindingContainer)
        {
            BinderBase binder = BindingHelpers.CreateBinder(bindingContainer);
            binder.ExecuteUnbind();
        }
    }

    public static class BindingHelpers
    {

        private const string STATE_MODE_KEY = "BindingStateMode";
        private const string UPDATE_SOURCE_TRIGGER_KEY = "UpdateSourceTrigger";

        private const StateMode DEFAULT_STATE_MODE = StateMode.Persist;
        private const string NEED_BINDING_OPTIONS_CONTROL = "BindR requires a BindingOptionsControl on the page. The BindingOptions must appear before any controls that need it.";
        private const string NEED_BINDING_OPTIONS_RESOURCES = "BindR requires a BindingOptionsControl on the page and requires the binding options to have a Resources collection. The BindingOptions must appear before any controls that need it.";
        private const string NEED_BINDING_RESOURCE = "BindR requires a BindingOptionsControl on the page and requires the binding options to have a Resources collection with a Resource that matches the ID specified by BindR. The BindingOptions must appear before any controls that need it.";


        #region Commands

        #region Shortcuts 

        /// <summary>
        /// Shorthand call for BindCommand
        /// </summary>
        /// <param name="control"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static string BindC(this object control, string sourcePath)
        {
            return BindCommand(control,sourcePath);
        }

        /// <summary>
        /// Shorthand call for BindCommand
        /// </summary>
        /// <param name="control"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static string BindC(this object control, object bindingOptions)
        {
            return BindCommand(control, bindingOptions);
        }

        /// <summary>
        /// Shorthand call for BindCommand
        /// </summary>
        /// <param name="control"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static string BindC(this object control, Options bindingOptions)
        {
            return BindCommand(control, bindingOptions.Path);
        }

        #endregion 

        /// <summary>
        /// Bind to a command
        /// <remarks>Allows the use of the slower but more succinct binding syntax</remarks>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="bindingOptions">Options for binding. Only the Path property is utilised for command bindings</param>
        public static string BindCommand(this object control, object bindingOptions)
        {
            Options options = new Options();
            options.Load(bindingOptions);

            return BindCommand(control, options.Path);
        }

        /// <summary>
        /// Bind to a command
        /// </summary>
        /// <param name="control"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static string BindCommand(this object control, string sourcePath)
        {
            return BindCommand(control, new Options { Path = sourcePath });
        }

        /// <summary>
        /// Bind to a command
        /// </summary>
        /// <remarks>
        /// Binding via an options object is supported to allow for a more natural syntax between data and command bindings
        /// </remarks>
        /// <param name="control"></param>
        /// <param name="bindingOptions">Options for binding. Only the Path property is utilised for command bindings.
        /// </param>
        public static string BindCommand(this object control, Options bindingOptions)
        {
            
            Control ctrl = control as Control;
            IBindingContainer bindingContainer = ctrl.Page as IBindingContainer;
            BinderBase binder = CreateBinder(bindingContainer);

            binder.ExecuteCommandBind(new WebformControl(ctrl), bindingOptions);

            return "return true";
        }

        #endregion

        #region Resource Binding

        /// <summary>
        /// Shorthand call for BindResource
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public static object BindR(this object control, string resourceID)
        {
            return BindResource(control, resourceID);
        }

        /// <summary>
        /// Bind using a global Resource binding
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceID">The id of the binding which should be used</param>
        /// <returns></returns>
        public static object BindResource(this object control, string resourceID)
        {
            Control ctrl = control as Control;

            IControlService service = new WebformsControlService();
            IBindingTarget target = new WebformControl(ctrl.Page);

            WebformControl webFormControl = service.FindControlRecursive(target, typeof(BindingOptionsControl)) as WebformControl;
            BindingOptionsControl bindingOptions = webFormControl.Control as BindingOptionsControl;
            if (bindingOptions == null)
                throw new InvalidOperationException(NEED_BINDING_OPTIONS_CONTROL);

            if (bindingOptions.Resources == null)
                throw new InvalidOperationException(NEED_BINDING_OPTIONS_RESOURCES);

            Options options = bindingOptions.Resources.Where(r => r.ID == resourceID).FirstOrDefault();
            if (options == null)
                if (bindingOptions.Resources == null)
                    throw new InvalidOperationException(NEED_BINDING_RESOURCE);

            if (options.Mode == BindingMode.Command)
            {
                BindCommand(control, options);
                return null;
            }
            else
                return Bind(control, options);
        }

        #endregion 

        #region Data Binding

        /// <summary>
        /// Bind with the default options
        /// </summary>
        /// <param name="control"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static object Bind(this object control, string sourcePath)
        {
            return Bind(control, new Options { Path=sourcePath });
        }

        /// <summary>
        /// Bind and specify options for the bind. Allows for a cleaner binding syntax by taking advantages of anonymous types and reflection
        /// </summary>
        /// <remarks>
        /// Slower than explicitly creating a Options object or using a binding resource
        /// </remarks>
        /// <param name="control"></param>
        /// <param name="bindingOptions"></param>
        /// <returns></returns>
        public static object Bind(this object control, object bindingOptions)
        {
            Options options = new Options();
            options.Load(bindingOptions);

            return Bind(control, options);
        }

        /// <summary>
        /// Bind and specify options for the bind.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="bindingOptions"></param>
        /// <returns></returns>
        public static object Bind(object control, Options bindingOptions)
        {
            return Bind(control, bindingOptions, null);
        }

        /// <summary>
        /// Bind and specify options for the bind.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="bindingOptions"></param>
        /// <returns></returns>
        public static object Bind(object control, Options bindingOptions, string propertyName)
        {
            Control ctrl = control as Control;
            WebformControl wrappedControl = new WebformControl(ctrl);

            IBindingContainer bindingContainer = ctrl.Page as IBindingContainer;
            BinderBase binder = CreateBinder(bindingContainer);

            object bindingResult = null;

            if (ctrl != null && bindingContainer != null)
            {
                IDataItemContainer dataItemContainer = ctrl.NamingContainer as IDataItemContainer;

                if (dataItemContainer != null)
                {
                    WebformControl<IDataItemContainer> wrappedContainer = new WebformControl<IDataItemContainer>((Control)dataItemContainer);

                    if (string.IsNullOrEmpty(propertyName))
                        bindingResult = binder.ExecuteBind(wrappedControl, wrappedContainer, bindingOptions);
                    else
                        bindingResult = binder.ExecuteBind(wrappedControl, wrappedContainer, bindingOptions, propertyName);
                }
                else
                {
                    if (string.IsNullOrEmpty(propertyName))
                        bindingResult = binder.ExecuteBind(wrappedControl, bindingOptions);
                    else
                        bindingResult = binder.ExecuteBind(wrappedControl, bindingOptions, propertyName);
                }
            }

            return bindingResult;
        }

        #endregion 

        /// <summary>
        /// Create a Binder 
        /// </summary>
        /// <remarks>
        /// Specifying which binder to use can be acheived by placing a BindingOptionsControl on the page or
        /// using the BindingStateMode config key. BindingOptionsControl takes precedence. A default ViewStateBinder
        /// is used in the absence of the above. 
        /// </remarks>
        /// <param name="bindingContainer"></param>
        /// <returns></returns>
        public static BinderBase CreateBinder(IBindingContainer bindingContainer)
        {
            //Create a binder based on:
            //A) The binder specified by a binding options control
            BinderBase binder = null;
            if (!TryGetBinderFromOptionsControl(bindingContainer, out binder))
            {
                //B) The binder specified in the config
                if (!TryGetBinderFromConfig(bindingContainer, out binder))
                {
                    //C) the default binder is created.
                    binder = CreateBinder(bindingContainer, DEFAULT_STATE_MODE);                
                }
            }

            //Set the update source trigger on:
            UpdateSourceTrigger updateSourceTrigger =  UpdateSourceTrigger.PostBack;
            
            //A) The updateSourceTrigger specified by a binding options control
            if (!TryGetUpdateSourceTriggerOptionsControl(bindingContainer, out updateSourceTrigger))
            {
                //B) The updateSourceTrigger specified in the config
                TryGetUpdateSourceTriggerFromConfig(bindingContainer, out updateSourceTrigger);
            }
            
            binder.UpdateSourceTrigger = updateSourceTrigger;

            return binder;
        }

        /// <summary>
        /// Create a binder and explicitly specify the StateMode
        /// </summary>
        /// <param name="bindingContainer"></param>
        /// <param name="stateMode"></param>
        /// <returns></returns>
        public static BinderBase CreateBinder(IBindingContainer bindingContainer, StateMode stateMode)
        {

            BinderBase binder = null;
            IDataStorageService dataStorageService = GetDataStorageService(bindingContainer);

            if (stateMode == StateMode.Recreate)
                binder = new StatelessBinder(bindingContainer, dataStorageService, new WebformsControlService());
            else
                binder = new ViewStateBinder(bindingContainer, new ViewStateStorageService(bindingContainer.GetStateBag()), new WebformsControlService());

            return binder;
        }

        private static IDataStorageService GetDataStorageService(IBindingContainer bindingContainer)
        {
            IDataStorageService dataStorageService = null;
            IBindingServicesProvider serviceProvider = bindingContainer as IBindingServicesProvider;
            if (serviceProvider != null)
                dataStorageService = serviceProvider.DataStorageService;
            else
                dataStorageService = new ViewStateStorageService(bindingContainer.GetStateBag());

            return dataStorageService;
        }

        private static IControlService GetControlService(IBindingContainer bindingContainer)
        {
            IControlService controlService = null;
            IBindingServicesProvider serviceProvider = bindingContainer as IBindingServicesProvider;
            if (serviceProvider != null)
                controlService = serviceProvider.ControlService;
            else
                controlService = new WebformsControlService();

            return controlService;
        }

        private static bool TryGetBinderFromConfig(IBindingContainer bindingContainer, out BinderBase binder)
        {

            bool result = false;
            binder = null;
            string stateModeStr = WebConfigurationManager.AppSettings[STATE_MODE_KEY];
            
            if (!string.IsNullOrEmpty(stateModeStr))
            {
                StateMode stateModeEnum = StateMode.Persist;
                if (stateModeEnum.TryParse(stateModeStr))
                {
                    binder = CreateBinder(bindingContainer, stateModeEnum);                    
                    result = true;
                }

            }

            return result;
        }

        private static bool TryGetUpdateSourceTriggerFromConfig(IBindingContainer bindingContainer, out UpdateSourceTrigger updateSourceTrigger)
        {
            bool result = false;
            updateSourceTrigger = UpdateSourceTrigger.PostBack;
            string updateSourceTriggerStr = WebConfigurationManager.AppSettings[UPDATE_SOURCE_TRIGGER_KEY];

            if (!string.IsNullOrEmpty(updateSourceTriggerStr))
            {

                if (updateSourceTrigger.TryParse(updateSourceTriggerStr))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool TryGetUpdateSourceTriggerOptionsControl(IBindingContainer bindingContainer, out UpdateSourceTrigger updateSourceTrigger)
        {
            bool result = false;
            updateSourceTrigger = UpdateSourceTrigger.PostBack;

            BindingOptionsControl bindingOptionsControl = null;

            if (TryGetBindingOptionsControl(bindingContainer, out bindingOptionsControl))
            {
                updateSourceTrigger = bindingOptionsControl.UpdateSourceTrigger;
                result = true;
            }

            return result; 
        }
        
        private static bool TryGetBinderFromOptionsControl(IBindingContainer bindingContainer, out BinderBase binder)
        {
            bool result = false;
            binder = null;

            BindingOptionsControl bindingOptionsControl = null;

            if (TryGetBindingOptionsControl(bindingContainer, out bindingOptionsControl))
            {
                binder = CreateBinder(bindingContainer, bindingOptionsControl.StateMode);
                result = true;
            }

            return result; 
        }

        private static bool TryGetBindingOptionsControl(IBindingContainer bindingContainer, out BindingOptionsControl bindingOptionsControl)
        {
            bool result = false;
            bindingOptionsControl = null;

            Page page = bindingContainer as Page;
            if (page == null)
                throw new InvalidOperationException("This method binding extension can only be used within the context of an asp.net page");

            IBindingTarget target = new WebformControl(page);
            IControlService controlService = GetControlService(bindingContainer);
            WebformControl wrappedOptionsControl = controlService.FindControlRecursive(target, typeof(BindingOptionsControl)) as WebformControl;

            if (wrappedOptionsControl != null)
            {
                bindingOptionsControl = wrappedOptionsControl.Control as BindingOptionsControl;

                if (bindingOptionsControl != null)
                {
                    result = true;
                }
            }

            return result;
        }

        
    }
}