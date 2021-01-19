// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Solutions.CosmosDB.Mongo.TODO.Service.Models
{
    public class ToDo : CosmosEntityBase
    {
        public string title { get; set; }
        public Status status { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string notes { get; set; }

        private int _percentComplete;

        public int percentComplete
        {
            get { return _percentComplete; }
            set
            {
                if ((percentComplete < 0) || (percentComplete > 100))
                {
                    throw new OverflowException("percent value should be between 0 to 100");
                }
                else
                {
                    _percentComplete = percentComplete;
                }
            }
        }
    }

    public enum Status { New, InProcess, Done }
}
