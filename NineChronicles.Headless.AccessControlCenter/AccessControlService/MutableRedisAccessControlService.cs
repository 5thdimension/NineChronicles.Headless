using System.Collections.Generic;
using System.Linq;
using Libplanet.Crypto;
using NineChronicles.Headless.Services;

namespace NineChronicles.Headless.AccessControlCenter.AccessControlService
{
    public class MutableRedisAccessControlService : RedisAccessControlService, IMutableAccessControlService
    {

        public MutableRedisAccessControlService(string storageUri) : base(storageUri)
        {
        }

        public void DenyAccess(Address address)
        {
            _db.StringSet(address.ToString(), "0");
        }

        public void AllowAccess(Address address)
        {
            var value = _db.StringGet(address.ToString());
            if (value == "0")
            {
                _db.KeyDelete(address.ToString());
            }
        }

        public void DenyWhiteList(Address address)
        {
            var value = _db.StringGet(address.ToString());
            if (value == "1")
            {
                _db.KeyDelete(address.ToString());
            }
        }

        public void AllowWhiteList(Address address)
        {
            _db.StringSet(address.ToString(), "1");
        }

        public List<Address> ListBlockedAddresses(int offset, int limit)
        {
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
            var keys = server
                .Keys()
                .Select(k => new Address(k.ToString()))
                .ToList();

            return keys.Skip(offset).Take(limit).ToList();
        }
    }
}
