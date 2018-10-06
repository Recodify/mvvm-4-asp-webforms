using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.ControlWrappers;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Binding.Services;
using System.Collections;

namespace Binding
{
    [Serializable]
    public class BindingPath
    {

        private BindingPath parent;
        public BindingPath Parent { get { return parent; } }
        
        private readonly string raw;
        public string Raw { get { return raw; } }

        private readonly string fullyQualified;
        public string FullyQualified
        {
            get { return fullyQualified; }
        }

        private readonly string indexed;
        public string Indexed { get { return indexed; } }

        private readonly PathMode mode;
        public PathMode Mode { get { return mode; } }

        public BindingPath(string raw)
        {
            this.raw = raw;
            this.indexed = raw;
            this.fullyQualified = raw;
        }

        public BindingPath(string raw, int indexer)
        {
            this.raw = raw;

            if (raw.Contains("."))
            {
                int lastDotIndex = raw.LastIndexOf('.');
                this.indexed = raw.Insert(lastDotIndex, string.Format("[{0}]", indexer.ToString()));
            }
            else
            {
                this.indexed = string.Format("[{0}].{1}",indexer.ToString(),raw);
            }

            this.fullyQualified = this.indexed;
        }

        public BindingPath(string raw, int indexer, BindingPath parent, PathMode mode)
            : this(raw, indexer)
        {
            this.parent = parent;
            
            if (mode == PathMode.Relative)
                this.fullyQualified = GetFullExpression(parent);
        }

        public BindingPath(string raw, BindingPath parent)
        {

            this.parent = parent;
            
            this.raw = raw;
            this.indexed = raw;
            if (mode == PathMode.Relative)
                this.fullyQualified = GetFullExpression(parent);
            else
                this.fullyQualified = this.raw;
        }

        public BindingPath(string raw, BindingPath parent, PathMode mode)
        {
            this.parent = parent;
            this.mode = mode;

            this.raw = raw;
            this.indexed = raw;
            if (mode == PathMode.Relative)
                this.fullyQualified = GetFullExpression(parent);
            else
                this.fullyQualified = this.raw;
        }

        public TargetProperty ResolveAsTarget(IBindingTarget container, IControlService controlService)
        {
            string[] targetExpressionSplit = this.Raw.Split('.');

            IBindingTarget control = controlService.FindControlUnique(container, targetExpressionSplit[0]);
            object rawControl = controlService.Unwrap(control);

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(rawControl).Find(targetExpressionSplit[1], true);
            
            object value = null;
            if (descriptor != null)
                value = descriptor.GetValue(rawControl);

            return new TargetProperty { Descriptor = descriptor, OwningControlRaw = rawControl, OwningControl= control, Value = value };
        }

        public TargetEvent ResolveAsTargetEvent(IBindingTarget container, IControlService controlService)
        {
            string[] targetExpressionSplit = this.Raw.Split('.');

            IBindingTarget control = controlService.FindControlUnique(container, targetExpressionSplit[0]);
            object rawControl = controlService.Unwrap(control);

            EventInfo info = rawControl.GetType().GetEvent(targetExpressionSplit[1]);

            return new TargetEvent { OwningControl = rawControl, Descriptor = info };
        }

        public SourceProperty ResolveAsSource(object container)
        {
            return GetSourceProperty(this.FullyQualified, container);
        }

        private string GetFullExpression(BindingPath parent)
        {
            return GetParentExpression(parent) + this.Indexed;
        }

        private string GetParentExpression(BindingPath parent)
        {

            string basic = string.Empty;
            if (parent != null)
            {
                basic = parent.Indexed;
                if (parent.Parent  != null)
                    basic = GetParentExpression(parent.Parent) + basic;
            }

            if (basic.Length > 0 && !this.Indexed.StartsWith("["))
                basic = basic + ".";

            return basic;
        }

        private SourceProperty GetSourceProperty(string path, object container)
        {
            string[] parts = path.Split('.');
            string leftMost = parts[0];
            string leftMostName = GetName(leftMost);
            int leftMostIndexer = 0;

            SourceProperty result = new SourceProperty { OwningInstance = container };

            if (!string.IsNullOrEmpty(leftMost))
            {
                PropertyDescriptor pDescriptor = TypeDescriptor.GetProperties(container).Find(leftMostName, true);
                if (pDescriptor != null)
                {
                    result.Descriptor = pDescriptor;

                    object pValue = pDescriptor.GetValue(container);

                    //this is the dataitem from the viewstate
                    IEnumerable enumerablePValue = pValue as IEnumerable;
                    if (enumerablePValue != null && !(pValue is string))
                    {

                        if (TryGetIndexer(leftMost, out leftMostIndexer))
                        {
                            pValue = enumerablePValue.GetByIndex(leftMostIndexer);
                        }
                    }

                    if (pValue != null)
                    {
                        result.Value = pValue;
                        //remove the current section and retest
                        path = path.TrimStart(parts[0].ToCharArray());
                        path = path.TrimStart(new char[] { '.' });
                        string[] newParts = path.Split('.');
                        if (newParts[0] != string.Empty)
                            result = GetSourceProperty(path, pValue);

                        return result;
                    }
                }
            }

            return result;
        }

        private bool TryGetIndexer(string inputString, out int index)
        {
            bool result = false;

            Match match = Regex.Match(inputString, "[[0-9]*]$");
            string strResult = match.Value;
            strResult = Regex.Match(strResult, "[0-9]").Value;
            result = int.TryParse(strResult, out index);

            return result;
        }

        private string GetName(string inputString)
        {
            Match match = Regex.Match(inputString, "^[A-Za-z]*");
            return match.Value;
        }
    }
}
