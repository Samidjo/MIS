#region Copyright © 2020 Vladimir Deryagin. All rights reserved
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

using MIS.Demo.DataContexts;
using MIS.Domain.Entities;
using MIS.Domain.Providers;
using MIS.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MIS.Demo.Repositories
{
	public class ResourcesRepository : IResourcesRepository
	{
		private readonly DemoDataContext _dataContext;
		private readonly IDateTimeProvider _dateTimeProvider;

		public ResourcesRepository(
			IDateTimeProvider dateTimeProvider,
			DemoDataContext dataContext
		)
		{
			_dateTimeProvider = dateTimeProvider;
			_dataContext = dataContext;
		}

		public IEnumerable<Resource> ToList()
		{
			return _dataContext.Resources
				.Where(r => r.Doctor.Specialty.ID > 0)
				.ToList();
		}

		public IEnumerable<Resource> GetDispanserizations()
		{
			return _dataContext.Resources
				.Where(r => r.Doctor.Specialty.ID == 0)
				.ToList();
		}
	}
}