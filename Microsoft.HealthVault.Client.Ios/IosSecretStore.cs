using Foundation;
using System;
using System.Threading.Tasks;
using Security;
using System.Threading;
using System.IO;

namespace Microsoft.HealthVault.Client
{
    internal class IosSecretStore : NSObject, ISecretStore
    {
        public IosSecretStore() :
            base ()
        {}

        public Task DeleteAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            SecStatusCode status = SecKeyChain.Remove(this.NewSecRecordForKey(key));

            if (status != SecStatusCode.Success && status != SecStatusCode.ItemNotFound)
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionDelete, key, status.ToString()));
            }

            return Task.CompletedTask;
        }

        public Task<string> ReadAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            SecRecord record = this.ExistingSecRecordForKey(key);

            if (record != null)
            {
                return Task.FromResult(this.ExistingSecRecordForKey(key).ValueData.ToString());
            }

            return Task.FromResult<string>(null);
        }

        public Task WriteAsync(string key, string contents)
        {
            if (string.IsNullOrEmpty(key) || contents == null)
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            SecStatusCode status = SecStatusCode.IO;

            try
            {
                SecRecord record = this.ExistingSecRecordForKey(key);

                SecRecord newRecord = this.NewSecRecordForKey(key);

                if (record == null)
                {
                    newRecord.ValueData = NSData.FromString(contents);

                    status = SecKeyChain.Add(newRecord);
                }
                else
                {
                    SecRecord update = new SecRecord()
                    {
                        ValueData = NSData.FromString(contents)
                    };

                    status = SecKeyChain.Update(newRecord, update);
                }
            }
            catch (Exception e)
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionWrite, key), e);
            }

            if (status != SecStatusCode.Success)
            {
                throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionWrite, key, status.ToString()));
            }

            return Task.CompletedTask;
        }

        private SecRecord NewSecRecordForKey(string key)
        {
            return new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
                Generic = NSData.FromString(key),
                Account = key,
                Accessible = SecAccessible.AfterFirstUnlockThisDeviceOnly
            }; 
        }

        private SecRecord ExistingSecRecordForKey(string key)
        {
            SecStatusCode status;

            SecRecord record = SecKeyChain.QueryAsRecord(this.NewSecRecordForKey(key), out status);

            if (status == SecStatusCode.Success)
            {
                return record;
            }
            else if (status == SecStatusCode.ItemNotFound)
            {
                return null;
            }

            throw new IOException(string.Format(ClientResources.FileAccessErrorMessage, ClientResources.FileAccessActionRead, key, status.ToString()));
        }
    }
}
