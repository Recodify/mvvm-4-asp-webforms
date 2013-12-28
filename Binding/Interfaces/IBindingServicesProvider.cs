using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.Services;

namespace Binding.Interfaces
{
    /// <summary>
    /// Services Container. Implement this interface on your IBindingContainer implemenation if you want to supply customized services.
    /// <remarks>Useful for injecting mocked services for testing.</remarks>
    /// </summary>
    public interface IBindingServicesProvider
    {
        IDataStorageService DataStorageService { get; }
        IControlService ControlService { get; }
    }
}
