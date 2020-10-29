﻿using MediatR;
using MIS.Application.ViewModels;
using MIS.Domain.Entities;
using MIS.Domain.Providers;
using MIS.Domain.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MIS.Application.Commands
{
    public class DispanserizationCreateHandler : IRequestHandler<DispanserizationCreateCommand, DispanserizationViewModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IDispanserizationsRepository _dispanserizations;

        public DispanserizationCreateHandler(
            IDateTimeProvider dateTimeProvider,
            IDispanserizationsRepository dispanserizations
        )
        {
            _dateTimeProvider = dateTimeProvider;
            _dispanserizations = dispanserizations;
        }

        public async Task<DispanserizationViewModel> Handle(DispanserizationCreateCommand request, CancellationToken cancellationToken)
        {
            Dispanserization dispanserization = new Dispanserization
            {
                BeginDate = request.BeginDate,
                EndDate = new DateTime(request.BeginDate.Year, 12, 31),
                PatientID = request.PatientID
            };

            Int32 dispanserizationID = _dispanserizations.Create(dispanserization);

            dispanserization = _dispanserizations.Get(dispanserizationID);

            DispanserizationViewModel viewModel = new DispanserizationViewModel
            {
                BeginDate = dispanserization.BeginDate,
                Today = _dateTimeProvider.Now.Date,
                PatientCode = request.PatientCode,
                PatientName = request.PatientName,
                IsClosed = dispanserization.IsClosed,
                IsEnabled = true,
                Analyses = dispanserization.Analyses.Select(a => a.Description).ToList()
            };

            return await Task.FromResult(viewModel);
        }
    }
}