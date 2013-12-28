using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.Services;
using System.Web.UI;
using Binding.ControlWrappers;
using ASPBinding.ControlWrappers;
using System.Collections;
using System.Diagnostics;
using Binding;
using System.Text.RegularExpressions;
using System.Reflection;
using Binding.Interfaces;

namespace ASPBinding.Services
{
    public class WebformsControlService : IControlService
    {
        private BindingSequenceManager sequenceManager;
        private BindingSequenceManager SequenceManager
        {
            get
            {
                if (sequenceManager == null)
                    sequenceManager = new BindingSequenceManager();

                return sequenceManager;
            }
            set
            {
                sequenceManager = value;
            }
        }

        public IBindingTarget FindControlUnique(IBindingTarget root, string unique)
        {
            if (root.UniqueID == unique)
                return root;

            foreach (Control ctl in this.GetChildren(root))
            {
                IBindingTarget webFormControl = new WebformControl(ctl);
                IBindingTarget foundCtl = FindControlUnique(webFormControl, unique);
                if (foundCtl != null)
                    return foundCtl;
            }

            return null;
        }

        public  IBindingTarget FindControlRecursive(IBindingTarget root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control ctl in this.GetChildren(root))
            {
                IBindingTarget webFormControl = new WebformControl(ctl); 
                IBindingTarget foundCtl = FindControlRecursive(webFormControl, id);
                if (foundCtl != null)
                    return foundCtl;
            }

            return null;
        }

        public  IBindingTarget FindControlRecursive(IBindingTarget root, Type type)
        {

            Control resolvedRoot = Resolve(root);
            if (resolvedRoot.GetType() == type)
                return root;

            foreach (Control ctl in this.GetChildren(root))
            {
                IBindingTarget webFormControl = new WebformControl(ctl);
                IBindingTarget foundCtl = FindControlRecursive(webFormControl, type);
                if (foundCtl != null)
                    return foundCtl;
            }

            return null;
        }

        public object Unwrap(IBindingTarget bindingTarget)
        {
            if (bindingTarget == null)
                throw new ArgumentException("Cannot unwrap null object", "bindingTarget");

            WebformControl webControl = bindingTarget as WebformControl;
            if (webControl == null)
                throw new ArgumentException(string.Format("Unable to unwrap objects of type {0}", bindingTarget.GetType().FullName), "bindingTarget");

            if (webControl.Control == null)
                throw new ArgumentException("Unwrapped object is null", "bindingTarget");

            return webControl.Control;
        }

        public IEnumerable GetChildren(IBindingTarget parent)
        {
            Control ctrl = Resolve(parent);
            if (ctrl == null)
                throw new ArgumentException("parent must be of type System.Web.UI.Control in order for its children to be resolved by this service", "parent");

            return ctrl.Controls;
        }

        public IBindingTarget GetParent(IBindingTarget child)
        {

            Control ctrl = Resolve(child);

            if (ctrl == null)
                throw new ArgumentException("child must be of type System.Web.UI.Control in order for its parent to be resolved by this service", "child");

            return new WebformControl(ctrl.Parent);
        }

        public bool TryGetTargetPropertyName(IBindingTarget target, Options options, out string targetPropertyName)
        {
            targetPropertyName = string.Empty;
            try
            {
                //get method body
                StackFrame frame = GetBindingFrame();
                string methodName = frame.GetMethod().Name;

                if (!this.SequenceManager.Sequences.Keys.Contains(methodName))
                {
                    this.SequenceManager.Sequences.Add(methodName, new BindingSequnce() { Instructions = GetBindingInstructionSets(frame) });
                }
                else
                {
                    this.SequenceManager.Sequences[methodName].MoveNext();
                }

                targetPropertyName = this.SequenceManager.Sequences[methodName].CurrentInstruction.GetAssignedProperty();
            }
            catch (Exception exp)
            {
                Logger.Trace(exp);
            }

            return !string.IsNullOrEmpty(targetPropertyName);

        }

        private static StackFrame GetBindingFrame()
        {
            StackTrace trace = new StackTrace(true);
            StackFrame frame = null;
            
            for (int i = 0; i < 100; i++)
            {
                frame = trace.GetFrame(i);
                if (frame.GetMethod().Name.StartsWith("__DataBinding")) 
                    break;
            }
            
            return frame;
        }

        private static List<IBindingInstruction> GetBindingInstructionSets(StackFrame frame)
        {

            List<IBindingInstruction> instructions = new List<IBindingInstruction>();

            try
            {

                //get method body
                string methodName = frame.GetMethod().Name;

                //Get instruction offset
                int xOffset = frame.GetILOffset();
                //and convert from hex to decimal
                int dOffset = int.Parse(xOffset.ToString(), System.Globalization.NumberStyles.HexNumber);

                //Get IL Instructions
                SDILReader.Globals.LoadOpCodes();
                SDILReader.MethodBodyReader ILReader = new SDILReader.MethodBodyReader((MethodInfo)frame.GetMethod());
                string s = ILReader.GetBodyCode();

                //Get binding statement blocks based on pattern
                MatchCollection matches = Regex.Matches(s, "\n[0-9]* : call System.Object Binding.BindingHelpers::Bind[0-9a-zA-Z\n(): . _]*[0-9]* : callvirt instance[^()]*");

                foreach (Match match in matches)
                {
                    instructions.Add(new DataBindingInstruction { RawSection = match.Value });
                }

                matches = Regex.Matches(s, "ldstr \"On[a-zA-Z0-9_]*\"[\n0-9 : \".a-zA-Z]*BindC()");

                foreach (Match match in matches)
                {
                    instructions.Add(new CommandBindingInstruction { RawSection = match.Value });
                }


            }
            catch (Exception e)
            {
                Logger.Trace(e);
            }


            return instructions;
        }

        private Control Resolve(IBindingTarget iTarget)
        {
            Control target = iTarget as Control;
            if (target == null)
            {
                WebformControl webformControl = iTarget as WebformControl;
                if (webformControl != null)
                    target = webformControl.Control;
            }

            return target;
        }
    }
}
