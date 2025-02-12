using Ryujinx.Horizon.Bcat;
using Ryujinx.Horizon.Hshl;
using Ryujinx.Horizon.Ins;
using Ryujinx.Horizon.Lbl;
using Ryujinx.Horizon.LogManager;
using Ryujinx.Horizon.MmNv;
using Ryujinx.Horizon.Ngc;
using Ryujinx.Horizon.Ovln;
using Ryujinx.Horizon.Prepo;
using Ryujinx.Horizon.Psc;
using Ryujinx.Horizon.Srepo;
using Ryujinx.Horizon.Usb;
using Ryujinx.Horizon.Wlan;
using System.Collections.Generic;
using System.Threading;

namespace Ryujinx.Horizon
{
    public class ServiceTable
    {
        private int _readyServices;
        private int _totalServices;

        private readonly ManualResetEvent _servicesReadyEvent = new(false);

        public IEnumerable<ServiceEntry> GetServices(HorizonOptions options)
        {
            List<ServiceEntry> entries = new();

            void RegisterService<T>() where T : IService
            {
                entries.Add(new ServiceEntry(T.Main, this, options));
            }

            RegisterService<BcatMain>();
            RegisterService<HshlMain>();
            RegisterService<InsMain>();
            RegisterService<LblMain>();
            RegisterService<LmMain>();
            RegisterService<MmNvMain>();
            RegisterService<NgcMain>();
            RegisterService<OvlnMain>();
            RegisterService<PrepoMain>();
            RegisterService<PscMain>();
            RegisterService<SrepoMain>();
            RegisterService<UsbMain>();
            RegisterService<WlanMain>();

            _totalServices = entries.Count;

            return entries;
        }

        internal void SignalServiceReady()
        {
            if (Interlocked.Increment(ref _readyServices) == _totalServices)
            {
                _servicesReadyEvent.Set();
            }
        }

        public void WaitServicesReady()
        {
            _servicesReadyEvent.WaitOne();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _servicesReadyEvent.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
