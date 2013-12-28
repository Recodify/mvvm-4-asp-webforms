using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Collections;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Input;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;
using Binding.Services;
using Binding.ControlWrappers;
using Binding.Helpers;
using Binding.Interfaces;

namespace Binding
{
    /// <summary>
    /// Provides Binding Services to an IBindingContainer
    /// </summary>
    public abstract class BinderBase
    {

        #region Internal Data

        private const string DATA_BINDING_KEY = "DATA_BINDINGS";
        private const string COMMAND_BINDING_KEY = "COMMAND_BINDINGS";
        private const string BINDING_SEQUENCE_MANAGER = "BINDING_SEQUENCE_MANAGER";

        private bool performBind = false;
        private bool isPushing = false;

        private object dataContext;
        public virtual object DataContext
        {
            get { return dataContext; }
            protected set { dataContext = value; }
        }

        private UpdateSourceTrigger updateSourceTrigger;
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return updateSourceTrigger; }
            set { updateSourceTrigger = value; }
        }
        
        private IBindingContainer bindingContainer;
        public IBindingContainer BindingContainer
        {
            get { return bindingContainer; }
        }


        private IDataStorageService dataStorageService;
        protected IDataStorageService DataStorageService
        {
            get { return dataStorageService; }
        }

        private IControlService controlService;
        protected IControlService ControlService
        {
            get { return controlService; }
        }

        private BindingCollection CommandStorage
        {
            get
            {
                return DataStorageService.RetrieveOrCreate<BindingCollection>(COMMAND_BINDING_KEY);
            }
        }

        private BindingCollection DataStorage
        {
            get
            {
                return DataStorageService.RetrieveOrCreate<BindingCollection>(DATA_BINDING_KEY);
            }
        }

        #endregion 

        public BinderBase(IBindingContainer bindingContainer, IDataStorageService dataStorageService, IControlService controlService)
        {
            this.bindingContainer = bindingContainer;
            this.dataStorageService = dataStorageService;
            this.controlService = controlService;

            bindingContainer.Load += new EventHandler(bindingContainer_Load);
        }

        #region Bind

        /// <summary>
        /// Execute a bind and return the result
        /// </summary>
        /// <remarks>
        /// This method will attempt to auto resolve the property of the target control to which to 
        /// bind. The implementation of this resolution can be control via IControlService.TryGetTargetPropertyName.
        /// </remarks>
        /// <param name="control">The target control</param>
        /// <param name="bindingOptions"></param>
        /// <returns>The result (value) of the bind</returns>
        public object ExecuteBind(IBindingTarget control, Options bindingOptions)
        {

            string propertyName = string.Empty;
            object value = null;
            
            if (controlService.TryGetTargetPropertyName(control, bindingOptions, out propertyName))
                value= ExecuteBind(control, bindingOptions, propertyName);

            return value;
        }

        /// <summary>
        /// Execute a bind and return the result
        /// </summary>
        /// <param name="controlID">The target control</param>
        /// <param name="bindingOptions"></param>
        /// <param name="targetPropertyName">The property on the target control which you wish to bind</param>
        /// <returns></returns>
        public object ExecuteBind(IBindingTarget control, Options bindingOptions, string targetPropertyName)
        {
            return ExecuteBind(control, null, bindingOptions, targetPropertyName);
        }

        /// <summary>
        /// Execute a bind in the context of a collection container. 
        /// </summary>
        /// <remarks>
        /// This method will attempt to auto resolve the property of the target control to which to 
        /// bind. The implementation of this resolution can be control via IControlService.TryGetTargetPropertyName.
        /// </remarks>
        /// <param name="control">The target control</param>
        /// <param name="collectionItemContainer">
        /// The collection item container. E.g. In the case of a repeater a RepeaterItem.
        /// This is available from the ItemTemplate control via (IDataItemContainer)container.NamingContainer. 
        /// </param>
        /// <param name="bindingOptions"></param>
        /// <returns>The result (value) of the bind</returns>
        public object ExecuteBind(IBindingTarget control, IBindingTarget<IDataItemContainer> collectionItemContainer, Options bindingOptions)
        {
            string propertyName = string.Empty;
            object value = null;
            
            if (controlService.TryGetTargetPropertyName(control, bindingOptions, out propertyName))
              value = ExecuteBind(control, collectionItemContainer, bindingOptions, propertyName);
            
            return value;
        }
        
        /// <summary>
        /// Execute a bind in the context of a collection container. 
        /// </summary>
        /// <remarks>
        /// Specficy the target control directly.
        /// </remarks>
        /// <param name="control">The target control</param>
        /// <param name="collectionItemContainer">
        /// The collection item container. E.g. In the case of a repeater a RepeaterItem.
        /// This is available from the ItemTemplate control via (IDataItemContainer)container.NamingContainer. 
        /// </param>
        ///<param name="targetPropertyName">The property on the targetcontrol.</param> 
        /// <param name="bindingOptions"></param>
        /// <returns>The result (value) of the bind</returns>
        public object ExecuteBind(IBindingTarget control, IBindingTarget<IDataItemContainer> collectionItemContainer, Options bindingOptions, string targetPropertyName)
        {
            if (control == null || bindingContainer == null ||  bindingOptions == null || string.IsNullOrEmpty(bindingOptions.Path))
                return null;

            object dataValue = null;
            BindingDef binding = null;
            
            IBindingTarget genericContainer = null;
            object baseContext = this.DataContext;

            if (bindingOptions.PathMode == PathMode.Relative && collectionItemContainer != null)
            {
                IDataItemContainer containerControl = collectionItemContainer.TargetInstance;
                dataValue = DataBinder.Eval(containerControl.DataItem, bindingOptions.Path);
                bool isCollectionBinding = DetermineIfCollection(control, dataValue);
                binding = new BindingDef(control, targetPropertyName, containerControl.DataItemIndex, DataStorage, bindingOptions, isCollectionBinding, controlService);
                genericContainer = collectionItemContainer;
            }
            else
            {
                dataValue = DataBinder.Eval(baseContext, bindingOptions.Path);
                bool isCollectionBinding = DetermineIfCollection(control, dataValue);
                binding = new BindingDef(control, targetPropertyName, bindingOptions, isCollectionBinding, controlService);
                genericContainer = controlService.GetParent(control);
            }

            //determine binding direction is not explicitly set based on the value and contorl
            if (bindingOptions.Mode == BindingMode.Unset)
                bindingOptions.Mode = DetermineBindingDirection(control, dataValue);

            //store the binding
            DataStorage.Add(binding);

            return AttemptConvert(binding, dataValue, genericContainer);
        }

        /// <summary>
        /// Bind to a command with auto resolution of the TargetProperty
        /// </summary>
        /// <param name="sourceExpression"></param>
        /// <param name="targetExpression"></param>
        public void ExecuteCommandBind(IBindingTarget control, Options bindingOptions)
        {
            string propertyName = string.Empty;

            if (controlService.TryGetTargetPropertyName(control, bindingOptions, out propertyName))
                ExecuteCommandBind(control, bindingOptions, propertyName);
           
        }

        /// <summary>
        /// Bind to a command when the TargetProperty is known
        /// </summary>
        /// <param name="control"></param>
        /// <param name="bindingOptions"></param>
        /// <param name="targetPropertyName"></param>
        public void ExecuteCommandBind(IBindingTarget control, Options bindingOptions, string targetPropertyName)
        {
            BindingDef binding = new BindingDef(control, targetPropertyName, bindingOptions, controlService);
            CommandStorage.Add(binding);
            ExecuteCommandBind(control, binding);
        }

        private void ExecuteCommandBind(BindingDef binding)
        {
            TargetProperty target = binding.ResolveAsTarget(this.BindingContainer);
            ExecuteCommandBind(target.OwningControl, binding);
        }
        
        private void ExecuteCommandBind(IBindingTarget control, BindingDef binding)
        {

            //Get the ICommandObject from the source
            SourceProperty commandInstance = binding.SourceExpression.ResolveAsSource(this.DataContext);
            object commandObject = commandInstance.Value;
            ICommand iCommand = commandObject as ICommand;

            //Get the property setter for the event to which we will bind            
            TargetEvent evt = binding.ResolveAsTargetEvent(this.bindingContainer);

            //bind
            Type eventDelegateType = evt.Descriptor.EventHandlerType;
            Delegate handler = new EventHandler((s, e) => iCommand.Execute(this));
            evt.Descriptor.GetAddMethod().Invoke(evt.OwningControl, new object[] { handler });

            control.Visible = iCommand.CanExecute(this);

            //listen to canExecuteChanged
            iCommand.CanExecuteChanged += new EventHandler((s,e) => control.Visible = iCommand.CanExecute(this));
        }

        private void RebindCommands()
        {
            if (CommandStorage != null)
            {
                foreach (BindingDef binding in CommandStorage)
                {
                    ExecuteCommandBind(binding);
                }
            }
        }
        
        private bool DetermineIfCollection(IBindingTarget control, object value)
        {
            if (value is IEnumerable && !(value is string))
                return true;

            return false;
        }

        private BindingMode DetermineBindingDirection(IBindingTarget control, object value)
        {
            if (control == null)
                return BindingMode.Unsupported;

            //String are IEnumerable!
            if (DetermineIfCollection(control, value))
                return BindingMode.OneWay;

            return BindingMode.TwoWay;
        }

        #endregion Bind

        #region Unbind


        /// <summary>
        /// Called by the binder to attempt the bind/unbind process
        /// </summary>
        private void ExecuteStagedUnbind()
        {
            try
            {
                if (!bindingContainer.IsPostBack)
                {
                    //If it's NOT a postback, always attempt a bind
                    performBind = true;
                }
                else
                {
                    //unbind if
                    if (this.updateSourceTrigger == UpdateSourceTrigger.PostBack)
                    {
                        ExecuteUnbind();
                    }
                }

                AttemptBind();
                RebindCommands();
                
            }
            finally
            {
                performBind = false;
            }

        }

        /// <summary>
        /// Call this method to initiate an explicit manual unbind
        /// </summary>
        public void ExecuteUnbind()
        {
            EnsureBindingSerivces();
            PushDataToModel();
            AttemptBind();
        }

        private void AttemptBind()
        { 
            
            //(Re)binds will occur on the intial load and if changes are detected
            // on the underlying model either as a result of an unbind operation (to support, cascading)
            // or as a direct model update.
            if (performBind)
            {
                performBind = false;
                this.DataStorage.Clear();
                this.CommandStorage.Clear();
                //will cause commands to be rebound as well
                this.bindingContainer.DataBind();
            }
        
        }

        private void bindingContainer_Load(object sender, EventArgs e)
        {
            EnsureBindingSerivces();
            ExecuteStagedUnbind();
        }

        private void PushDataToModel()
        {
            isPushing = true;
            try
            {

               
                //Listen for changes if the context implements INotifyPropertyChanged
                INotifyPropertyChanged notifyContext = this.DataContext as INotifyPropertyChanged;
                if (notifyContext != null)
                    notifyContext.PropertyChanged += new PropertyChangedEventHandler(DataItem_PropertyChanged);

                //Load all data items 
                //this.DataStorage.LoadDataItems(this.bindingContainer);

                //this works fine if all bindings are relative to the view model (ie, we only 
                //ever have one data context....if multiple contexts are to be supported
                //then we will first have to group by data context and then by source expression
                if (this.DataStorage != null)
                {
                    var groupBindings = this.DataStorage
                                .Where(x => x.BindingOptions.Mode == BindingMode.TwoWay)
                                .GroupBy(x => x.SourceExpression.FullyQualified);

                    foreach (var grouping in groupBindings)
                    {
                        var authorative = grouping.Where(x => x.BindingOptions.IsAuthorative).FirstOrDefault();
                        if (authorative != null)
                        {
                            ApplyBinding(authorative, this.bindingContainer);
                        }
                        else
                        {
                            //applying bindings in the order they were created
                            //the last bindings value will be the ultimate value of the source 
                            //unless the source implements a precedence/validation mechanism
                            List<BindingDef> nonAuthorative = grouping.ToList();
                            foreach (BindingDef binding in nonAuthorative)
                            {
                                ApplyBinding(binding, this.bindingContainer);
                            }
                        }
                    }
                }

            }
            finally
            {
                isPushing = false;
            }
        }

        private void ApplyBinding(BindingDef binding, IBindingContainer bindingContainer)
        {
            TargetProperty property = binding.ResolveAsTarget(this.bindingContainer);
            if (property != null)
            {
                object value = property.Value;
                SetProperty(binding, value);
            }
        }

        //TODO: Should probably be a member of binding.
        private void SetProperty(BindingDef binding, object value)
        {

            SourceProperty property = binding.SourceExpression.ResolveAsSource(this.DataContext);

            if (property != null && property.Descriptor != null && !property.Descriptor.IsReadOnly)
            {
                object convertedValue = null;
                Type destinationType = property.Descriptor.PropertyType;

                if (binding.HasValueConverter)
                {
                    IValueConverter valueConverter = binding.GetValueConverterInstance();

                    convertedValue = valueConverter.ConvertBack(value, property.Descriptor.PropertyType, null, null);
                }
                else
                {
                    if (TypeHelper.IsSimpleType(value.GetType()) && TypeHelper.IsSimpleType(destinationType))
                        convertedValue = Convert.ChangeType(value, destinationType);

                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
                        if (converter != null)
                            convertedValue = converter.ConvertFrom(value);
                        else
                        {
                            converter = TypeDescriptor.GetConverter(value.GetType());

                            if (converter != null)
                                convertedValue = converter.ConvertTo(value, destinationType);
                        }
                    }

                }

                property.Descriptor.SetValue(property.OwningInstance, convertedValue);
            }
        }

        private object AttemptConvert(BindingDef binding, object value, IBindingTarget container)
        {
            if (binding.HasValueConverter)
            {
                IValueConverter converter = binding.GetValueConverterInstance();
                value = converter.Convert(value, binding.ResolveAsTarget(container).Descriptor.PropertyType, binding, CultureInfo.CurrentCulture);
            }

            return value;

        }

        #endregion 

        #region Virtual / Abstract / Misc

        protected virtual void EnsureBindingSerivces()
        {
            foreach (BindingDef binding in this.DataStorage.Union(this.CommandStorage))
                binding.ControlService = controlService;
        }

        private void DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isPushing)
                performBind = true;
            else
                bindingContainer.DataBind();
        }

        #endregion 
    }
}