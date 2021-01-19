// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Solutions.CosmosDB
{
    public class CosmosEntityBase : IEntityModel<string>
    {
        public CosmosEntityBase()
        {
            this.id = Guid.NewGuid().ToString();
            this.__partitionkey = CosmosEntityBase.GetKey(id, 9999);
        }

        /// <summary>
        /// id will be generated automatically. you don't need to manage it by yourself
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// the partitionkey will be used for storage partitioning. you don't need to manage it by yourself
        /// </summary>
        public string __partitionkey { get; set; }

        static SHA1 _sha1;

        static CosmosEntityBase()
        {
            _sha1 = SHA1.Create();
        }

        /// <summary>
        /// Generate partitionkey for CosmosDB
        /// using SHA1 hash with id, convert it to uint and divide with number of partitions
        /// assigned default value as 1000 (1000 partition at this moment)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="numberofPartitions"></param>
        /// <returns></returns>
        public static string GetKey(string id, int numberofPartitions)
        {
            var hasedVal = _sha1.ComputeHash(Encoding.UTF8.GetBytes(id));
            var intHashedVal = BitConverter.ToUInt32(hasedVal, 0);

            var range = numberofPartitions - 1;
            var length = range.ToString().Length;

            var key = (intHashedVal % numberofPartitions).ToString();
            return key.PadLeft(length, '0');
        }

    }
}
