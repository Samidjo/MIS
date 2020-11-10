﻿#region Copyright © 2020 Vladimir Deryagin. All rights reserved
/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using Dapper;
using Microsoft.Data.SqlClient;
using MIS.Domain.Entities;
using MIS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MIS.Persistence.Repositories
{
    public class VisitItemsRepository : IVisitItemsRepository, IDisposable
    {
        private readonly IDbConnection _db;
        private readonly IDbTransaction _transaction;

        public VisitItemsRepository(String connectionString)
        {
            _db = new SqlConnection(connectionString);
            _transaction = null;
        }

        public VisitItemsRepository(IDbTransaction transaction)
        {
            _db = transaction.Connection;
            _transaction = transaction;
        }

        public Int32 Create(VisitItem item)
        {
            Int32 visitItemID = _db.QuerySingleAsync<Int32>(
                sql: "[dbo].[sp_VisitItems_Create]",
                param: new
                {
                    patientID = item.PatientID,
                    timeItemID = item.TimeItemID
                },
                commandType: CommandType.StoredProcedure,
                transaction: _transaction
            ).Result;

            return visitItemID;
        }

        public VisitItem Get(Int32 visitItemID)
        {
            VisitItem result = _db.QueryAsync<VisitItem, TimeItem, Resource, Doctor, Specialty, Room, VisitItem>(
                sql: "[dbo].[sp_VisitItems_Get]",
                map: (visitItem, timeItem, resource, doctor, specialty, room) =>
                {
                    visitItem.TimeItem = timeItem;
                    visitItem.TimeItem.Resource = resource;
                    visitItem.TimeItem.Resource.Doctor = doctor;
                    visitItem.TimeItem.Resource.Doctor.Specialty = specialty;
                    visitItem.TimeItem.Resource.Room = room;
                    visitItem.TimeItem.VisitItem = visitItem;

                    return visitItem;
                },
                param: new { visitItemID },
                commandType: CommandType.StoredProcedure,
                transaction: _transaction
            ).Result.FirstOrDefault();

            return result;
        }

        public IEnumerable<VisitItem> ToList(DateTime beginDate, DateTime endDate, Int32 patientID = 0)
        {
            IEnumerable<VisitItem> visitItems = _db.QueryAsync<VisitItem, TimeItem, Resource, Doctor, Specialty, Room, VisitItem> (
                sql: "[dbo].[sp_VisitItems_List]",
                map: (visitItem, timeItem, resource, doctor, specialty, room) =>
                {
                    visitItem.TimeItem = timeItem;
                    visitItem.TimeItem.Resource = resource;
                    visitItem.TimeItem.Resource.Doctor = doctor;
                    visitItem.TimeItem.Resource.Doctor.Specialty = specialty;
                    visitItem.TimeItem.Resource.Room = room;
                    visitItem.TimeItem.VisitItem = visitItem;

                    return visitItem;
                },
                param: new { beginDate, endDate, patientID },
                commandType: CommandType.StoredProcedure,
                transaction: _transaction
            ).Result;

            return visitItems;
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
    }
}
