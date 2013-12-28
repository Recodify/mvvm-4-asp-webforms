using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Binding;
using DomainModel.BusinessObjects;
using DomainModel.ViewModels;

namespace SimpleBinding
{
    /// <summary>
    /// Very dirty helper to supply some data and viewmodels for the demonstration of binding
    /// </summary>
    public static class DataHelper
    {
      

        public static AddressViewModel GetDemoAddressViewModel()
        {
            return new AddressViewModel()
                {
                    UserName = "Jim Davies",
                    SelectedAddress = new Address
                    {
                        HouseNameNumber = "999",
                        Postcode = "TR240QF",
                        Street = "Timothys Corner",
                        PhoneNumbers = new List<PhoneNumber>
                        {
                            new PhoneNumber
                            {
                                 Number = 01720423056
                            },
                            new PhoneNumber
                            {
                                Number = 4455468847
                            }
                        }
                    },
                    AvailableAddresses = new NotifyPropertyCollection<Address>
                        {
                            new  Address 
                            { 
                                HouseNameNumber = "1",
                                PhoneNumbers = new List<PhoneNumber>
                                {
                                    new PhoneNumber
                                    {
                                         Number = 01720423056
                                    },
                                    new PhoneNumber
                                    {
                                        Number = 4455468847
                                    }
                                }
                            },
                            new Address
                            { 
                                HouseNameNumber ="2",
                                PhoneNumbers = new List<PhoneNumber>
                                {
                                    new PhoneNumber
                                    {
                                         Number = 01720423056
                                    },
                                    new PhoneNumber
                                    {
                                        Number = 4455468847
                                    }
                                }
                            }

                        }
                };
        }


        public static EmployeeLeaveViewModel GetEmployeeLeaveViewModel()
        {
            return new EmployeeLeaveViewModel()
            {
                SelectedEmployee = new EmployeeLeavePeriod
                {
                    EmployeeName = "Jim Smith",
                    LeavePeriods = new List<LeavePeriod>
                    {
                        new LeavePeriod
                        {
                             StartDate = new DateTime(2000,01,01),
                             EndDate = new DateTime(2000,01,20),
                             Type= LeavePeriodType.Annual 
                        },
                        new LeavePeriod
                        {
                             StartDate = new DateTime(2001,07,25),
                             EndDate = new DateTime(2001,07,30),
                             Type= LeavePeriodType.Annual 
                        },
                         new LeavePeriod
                        {
                             StartDate = new DateTime(2001,10,7),
                             EndDate = new DateTime(2001,10,15),
                             Type= LeavePeriodType.Sick
                        },
                        new LeavePeriod
                        {
                             StartDate = new DateTime(2003,05,17),
                             EndDate = new DateTime(2004,01,20),
                             Type= LeavePeriodType.Unplanned 
                        },
                        new LeavePeriod
                        {
                             StartDate = new DateTime(2004,01,01),
                             EndDate = new DateTime(2007,01,20),
                             Type= LeavePeriodType.Gardening
                        }
                    }
                }
            };
        }
    }
}