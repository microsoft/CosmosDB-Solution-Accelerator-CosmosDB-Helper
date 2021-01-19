// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Solutions.CosmosDB
{
    /// <summary>
    /// Every Entnties have to follow this interface
    /// Unique identifier type should be string
    /// </summary>
    /// <typeparam name="TIdentifier"></typeparam>
    public interface IEntityModel<TIdentifier> 
    {
        TIdentifier id { get; set; }
        string __partitionkey { get; set; }
    }
}