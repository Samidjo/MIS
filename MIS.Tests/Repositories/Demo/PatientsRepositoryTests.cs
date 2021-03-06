using MIS.Demo.Repositories;
using MIS.Domain.Entities;
using MIS.Domain.Repositories;
using MIS.Tests.Fixtures.Demo;
using System;
using System.Collections.Generic;
using Xunit;

namespace MIS.Tests.Repositories.Demo
{
	public class PatientsRepositoryTests : IClassFixture<DataFixture>
	{
		private readonly IPatientsRepository _patients;

		public PatientsRepositoryTests(DataFixture dataFixture)
		{
			_patients = new PatientsRepository(dataFixture.DateTimeProvider, dataFixture.DataContext);
		}

		[Fact]
		public void First_Ok()
		{
			var actualResult = _patients.First("30000000", new DateTime(1980, 1, 1));
			var expectedResult = new Patient
			{
				Code = "30000000",
				FirstName = "Иван",
				MiddleName = "Иванович",
				BirthDate = new DateTime(1980, 1, 1),
				Gender = 0,
				Dispanserizations = new List<Dispanserization>(),
				VisitItems = new List<VisitItem>()
			};

			Assert.Equal(
				new
				{
					expectedResult.Code,
					expectedResult.FirstName,
					expectedResult.MiddleName,
					expectedResult.BirthDate,
					expectedResult.Gender
				},
				new
				{
					actualResult.Code,
					actualResult.FirstName,
					actualResult.MiddleName,
					actualResult.BirthDate,
					actualResult.Gender
				}
			);
		}

		[Fact]
		public void Get_Ok()
		{
			var patientID = _patients.First("30000000", new DateTime(1980, 1, 1)).ID;

			var actualResult = _patients.Get(patientID);
			var expectedResult = new Patient
			{
				ID = actualResult.ID,
				Code = "30000000",
				FirstName = "Иван",
				MiddleName = "Иванович",
				BirthDate = new DateTime(1980, 1, 1),
				Gender = 0,
				Dispanserizations = new List<Dispanserization>(),
				VisitItems = new List<VisitItem>()
			};

			Assert.Equal(
				new
				{
					ID = patientID,
					expectedResult.Code,
					expectedResult.FirstName,
					expectedResult.MiddleName,
					expectedResult.BirthDate,
					expectedResult.Gender
				},
				new
				{
					actualResult.ID,
					actualResult.Code,
					actualResult.FirstName,
					actualResult.MiddleName,
					actualResult.BirthDate,
					actualResult.Gender
				}
			);
		}
	}
}
