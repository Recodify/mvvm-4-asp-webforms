using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binding.Services
{
    public interface IDataStorageService
    {
        T RetrieveOrCreate<T>(string key) where T : class;
        T Retrieve<T>(string key) where T : class;
        void Store(string key, object value);
    }
}
