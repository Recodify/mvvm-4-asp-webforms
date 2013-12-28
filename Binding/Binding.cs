using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.ControlWrappers;
using Binding.Services;
using System.Windows.Data;

namespace Binding
{
    [Serializable]
    public class BindingCollection : List<BindingDef>
    {

        public BindingDef LastCollectionBinding
        {
            get
            {
                return this.Where(x => x.IsSourceEnumerable).LastOrDefault();
            }
        }

    }

    /// <summary>
    /// Records information about a single binding and provides convience methods for working with that information
    /// </summary>
    [Serializable]
    public class BindingDef
    {

        #region Properties

        /// <summary>
        /// The parent binding. 
        /// <remarks>
        /// Primarily used when nesting bindings in a controll tree.
        /// Will be null if the binding does not have a parent.
        /// </remarks>
        /// </summary>
        public BindingDef Parent { get; set; }


        private readonly BindingPath sourceExpression;
        /// <summary>
        /// The expression used to describe the source of the binding
        /// </summary>
        public BindingPath SourceExpression { get { return sourceExpression; } }


        private readonly BindingPath targetExpression;
        /// <summary>
        /// The expression used to describe the target of the binding
        /// </summary>
        public BindingPath TargetExpression { get { return targetExpression; } }


        private readonly Options bindingOptions;
        /// <summary>
        /// Options for how the binding should operate.
        /// </summary>
        public Options BindingOptions { get { return bindingOptions; } }


        [NonSerialized]
        private IBindingTarget container;
        /// <summary>
        /// The control which contains the binding.
        /// <remarks>Only availabe on initial bind. Not available on unbind</remarks>
        /// </summary>
        public IBindingTarget Container { get { return container; } set { container = value; } }


        [NonSerialized]
        private IControlService controlService;
        /// <summary>
        /// Used to resolve control ids to control instances and to navigate a controls tree.
        /// </summary>
        public IControlService ControlService
        {
            get { return controlService; }
            set { controlService = value; }
        }


        /// <summary>
        /// Is the source data of the binding Enumrable (a collection) 
        /// </summary>
        public bool IsSourceEnumerable { get; set; }

        /// <summary>
        /// The data item index of the source object.
        /// <remarks>Will be zero if this binidng is not the child of an Enumerable binding</remarks>
        /// </summary>
        public int DataItemIndex { get; set; }

        /// <summary>
        /// Does this binding have an associated IValueConverter
        /// </summary>
        public bool HasValueConverter
        {
            get
            {
                return this.bindingOptions.Converter != null;
            }
        }

        #endregion

        #region cTors

        /// <summary>
        /// Used to create a command binding
        /// </summary>
        /// <param name="sourceExpression"></param>
        /// <param name="targetExpression"></param>
        /// <param name="containerID"></param>
        public BindingDef(IBindingTarget targetControl, string targetPropertyName, Options bindingOptions, IControlService controlService)
        {
            this.controlService = controlService;
            this.sourceExpression = new BindingPath(bindingOptions.Path);

            string targetExpression = string.Format("{0}.{1}", targetControl.UniqueID, targetPropertyName);
            this.targetExpression = new BindingPath(targetExpression);
        }

        /// <summary>
        /// Used to create a data binding
        /// </summary>
        /// <param name="targetControl">The control to which to bind</param>
        /// <param name="targetPropertyName">The property of the control to which to bind</param>
        /// <param name="bindingOptions">Options to control the binding</param>
        /// <param name="isSourceEnumerable">Is the datasource to which you are binidng a collection</param>
        /// <param name="controlService">Service required for binding</param>
        public BindingDef(IBindingTarget targetControl, string targetPropertyName, Options bindingOptions, bool isSourceEnumerable, IControlService controlService)
        {

            this.controlService = controlService;
            this.Container = targetControl;
            this.sourceExpression = new BindingPath(bindingOptions.Path);

            string targetExpression = string.Format("{0}.{1}", targetControl.UniqueID, targetPropertyName);
            this.targetExpression = new BindingPath(targetExpression);

            this.bindingOptions = bindingOptions;
            this.IsSourceEnumerable = isSourceEnumerable;

        }

        /// <summary>
        /// Used to create a nested data binding
        /// </summary>
        /// <param name="targetControl">The control to which to bind</param>
        /// <param name="targetPropertyName">The property of the control to which to bind</param>
        /// <param name="dataItemIndex">If the parent binding is a collection, the index is the data item for this binding. Else zerp</param>
        /// <param name="owningCollection">The collection of current bindings</param>
        /// <param name="bindingOptions">Options to control the binding</param>
        /// <param name="isSourceEnumerable">Is the datasource to which you are binidng a collection</param>
        /// <param name="controlService">Service required for binding</param>
        public BindingDef(IBindingTarget targetControl, string targetPropertyName, int dataItemIndex, BindingCollection owningCollection, Options bindingOptions, bool isSourceEnumerable, IControlService controlService)
        {

            this.controlService = controlService;
            this.Container = targetControl;
            this.TrySetParent(owningCollection);
            this.sourceExpression = new BindingPath(bindingOptions.Path, dataItemIndex, GetParentExpression(), bindingOptions.PathMode);

            string targetExpression = string.Format("{0}.{1}", targetControl.UniqueID, targetPropertyName);
            this.targetExpression = new BindingPath(targetExpression);

            this.DataItemIndex = dataItemIndex;
            this.bindingOptions = bindingOptions;
            this.IsSourceEnumerable = isSourceEnumerable;
        }

        #endregion

        public IValueConverter GetValueConverterInstance()
        {
            IValueConverter valueConverter = null;
            if (this.HasValueConverter)
            {

                Type type = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a => a.GetTypes())
               .FirstOrDefault(t => t.Name == this.bindingOptions.Converter);

                valueConverter = Activator.CreateInstance(type) as IValueConverter;

            }

            return valueConverter;
        }

        public bool TrySetParent(BindingCollection bindingCollection)
        {
            return TrySetParent(bindingCollection.LastCollectionBinding);
        }

        public SourceProperty ResolveAsSource(object instance)
        {
            return this.SourceExpression.ResolveAsSource(instance);
        }

        public TargetProperty ResolveAsTarget(IBindingTarget container)
        {
            return this.TargetExpression.ResolveAsTarget(container, controlService);
        }

        public TargetEvent ResolveAsTargetEvent(IBindingTarget container)
        {
            return this.TargetExpression.ResolveAsTargetEvent(container, controlService);
        }

        private BindingPath GetParentExpression()
        {
            if (this.Parent != null)
                return this.Parent.SourceExpression;

            return null;
        }

        private bool TrySetParent(BindingDef lastBinding)
        {
            if (lastBinding != null)
            {
                //test if the current binding is a child of the previous datacontainer
                IBindingTarget child = controlService.FindControlUnique(lastBinding.Container, this.Container.UniqueID);
                if (child != null)
                    this.Parent = lastBinding;
                else if (lastBinding.Parent != null)
                    TrySetParent(lastBinding.Parent);
            }

            return this.Parent != null;
        }

    }
}
