using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.Services;
using System.Web.UI;

namespace ASPBinding.Services
{
    public class ViewStateStorageService : IDataStorageService
    {
        private StateBag viewState;

        public ViewStateStorageService(StateBag viewState)
        {
            this.viewState = viewState;
        }
        
        public void Store(string key, object value)
        {
            viewState[key] = value;
        }

        public T Retrieve<T>(string key) where T : class
        {
            T store = viewState[key] as T;

            return store;
        }

        public T RetrieveOrCreate<T>(string key) where T : class
        {
            T store = viewState[key] as T;
            if (store == null)
            {
                store = Activator.CreateInstance<T>();
                viewState[key] = store;
            }
            return store;
        }
    }
}
