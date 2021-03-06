﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roll20Stats.InfrastructureLayer.DAL.Context;
using Roll20Stats.InfrastructureLayer.DAL.Entities;
using Roll20Stats.PresentationLayer.DataTransferObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Roll20Stats.ApplicationLayer.PlayerStatistics.Commands.AddPlayerStatistic
{
    public class AddPlayerStatisticCommandHandler : IRequestHandler<AddPlayerStatisticCommand, ResponseWithMetaData<PlayerStatisticDto>>
    {
        private readonly IApplicationContext _dbContext;
        private readonly IMapper _mapper;

        public AddPlayerStatisticCommandHandler(IApplicationContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResponseWithMetaData<PlayerStatisticDto>> Handle(AddPlayerStatisticCommand request, CancellationToken _)
        {
            return await GetExistingPlayerStatistic(request) is { } playerStatistic
                ? UpdatePlayerStatistic(request, playerStatistic)
                : await CreatePlayerStatistic(request);
        }

        private async Task<ResponseWithMetaData<PlayerStatisticDto>> CreatePlayerStatistic(AddPlayerStatisticCommand request)
        {
            var newPlayerStatistic = _mapper.Map<PlayerStatistic>(request);
            newPlayerStatistic.Game = await _dbContext.Games.SingleOrDefaultAsync(Game => Game.Name == request.GameName)
                ?? await CreateNewGame(request.GameName);
            var dbEntry = await _dbContext.PlayerStatistics.AddAsync(newPlayerStatistic);
            _dbContext.SaveChanges();
            return _mapper.Map<ResponseWithMetaData<PlayerStatisticDto>>(dbEntry.Entity);
        }

        private async Task<Game> CreateNewGame(string gameName)
        {
            var newGame = new Game { Name = gameName };
            return (await _dbContext.Games.AddAsync(newGame)).Entity;
        }

        private ResponseWithMetaData<PlayerStatisticDto> UpdatePlayerStatistic(AddPlayerStatisticCommand request, PlayerStatistic playerStatistic)
        {
            playerStatistic.DamageDealt += request.DamageDealt;
            playerStatistic.DamageTaken += request.DamageTaken;
            playerStatistic.HealingDone += request.HealingDone;
            var dbEntry = _dbContext.PlayerStatistics.Update(playerStatistic);
            _dbContext.SaveChanges();
            return _mapper.Map<ResponseWithMetaData<PlayerStatisticDto>>(dbEntry.Entity);
        }

        private Task<PlayerStatistic> GetExistingPlayerStatistic(AddPlayerStatisticCommand request)
        {
            return _dbContext.PlayerStatistics
                .Include(playerStatistic => playerStatistic.Game)
                .SingleOrDefaultAsync(statistic => statistic.CharacterId == request.CharacterId && statistic.Game.Name == request.GameName);
        }
    }
}