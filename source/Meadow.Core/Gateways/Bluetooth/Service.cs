using System;
using System.Collections.ObjectModel;

namespace Meadow.Gateways.Bluetooth
{
    public class Service : IService
    {
        private bool disposedValue;

        /// <summary>
        /// Id of the Service.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// Name of the service.
        /// </summary>
		public string? Name { get; set; }

        //TODO: do we want to try and pull the name from the known services?
        // maybe if it's null? 
        //public string Name => this.Name ?? KnownServices.Lookup(Id).Name;

        /// <summary>
        /// Indicates whether the type of service is primary or secondary.
        /// </summary>
		public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets the characteristics of the service.
        /// </summary>
        //ObservableCollection<ICharacteristic> Characteristics { get; } = new ObservableCollection<ICharacteristic>();
        public ObservableDictionary<Guid, ICharacteristic> Characteristics { get; } = new ObservableDictionary<Guid, ICharacteristic>();

        public Service(Guid id)
        {
            this.ID = id;
        }

        // TODO: all the dispose stuff.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Service()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
