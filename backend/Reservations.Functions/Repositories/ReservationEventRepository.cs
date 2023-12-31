﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Reservations.Functions.Utils;

namespace Reservations.Functions.Repositories
{
    public interface IReservationEventRepository
    {
        Task<Guid> AddAsync(string connectionId, string invocationId, string message, DateTime createdAtUtc,
            CancellationToken cancellationToken);

        Task AcknowledgeAsync(string connectionId, string invocationId, string eventId,
            CancellationToken cancellationToken);
    }

    public class ReservationEventRepository : IReservationEventRepository
    {
        private readonly TableServiceClient _tableServiceClient;
        private TableClient _tableClient;
        private readonly string _tableName = "ReservationEvent";

        public ReservationEventRepository(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        private async Task<TableClient> GetTableClientAsync(CancellationToken cancellationToken)
        {
            if (_tableClient == null)
            {
                await _tableServiceClient.CreateTableIfNotExistsAsync(_tableName, cancellationToken);
                _tableClient = _tableServiceClient.GetTableClient(_tableName);
            }

            return _tableClient;
        }

        public async Task<Guid> AddAsync(string connectionId, string invocationId, string message, DateTime createdAtUtc,
            CancellationToken cancellationToken)
        {
            var tableClient = await GetTableClientAsync(cancellationToken);

            var id = Guid.NewGuid();
            var tableEntity = new TableEntity($"{connectionId}_{invocationId}", id.ToString("D"))
            {
                { "Type", "type" },
                { "Message", message },
                { "Acknowledged", false },
                { "CreatedAtUtc", createdAtUtc }
            };
            var response = await tableClient.AddEntityAsync(tableEntity, cancellationToken);
            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return id;
        }

        public async Task AcknowledgeAsync(string connectionId, string invocationId, string eventId,
            CancellationToken cancellationToken)
        {
            var tableClient = await GetTableClientAsync(cancellationToken);

            var tableEntity = new TableEntity($"{connectionId}_{invocationId}", eventId)
            {
                { "Acknowledged", true }
            };
            await tableClient.UpsertEntityAsync(tableEntity, TableUpdateMode.Merge, cancellationToken);
        }
    }
}