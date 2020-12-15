﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Roll20Stats.ApplicationLayer.Commands.AddPlayerStatistic;
using Roll20Stats.InfrastructureLayer.DAL.Entities;
using Roll20Stats.PresentationLayer.DataTransferObjects;
using Roll20Stats.Tests.Shared;
using Xunit;

namespace Roll20Stats.Tests.PlayerStatisticTests
{
    public class PlayerStatisticsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public PlayerStatisticsControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = TestDatabaseManager.SetupInMemoryDatabase(factory, "playerstatistics-database");
        }

        [Fact]
        public async Task Gets_Single_PlayerStatistic()
        {
            var stat = new PlayerStatistic
            {
                CharacterId = "Id",
                CharacterName = "Testosteron"
            };
            TestDatabaseManager.SeedDatabase(_factory, stat);
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/playerstatistics/Id");

            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<PlayerStatistic>(await response.Content.ReadAsStringAsync());
            responseObject.Should().BeEquivalentTo(stat, option
                => option.Excluding(statistic => statistic.Id));
        }

        [Fact]
        public async Task Returns_Not_Found_When_Statistic_Does_Not_Exist()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/playerstatistics/Id");

            var responseObject = JsonConvert.DeserializeObject<ResponseWithMetaData<PlayerStatistic>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseObject.ErrorMessage.Should().Be(@"Player with id ""Id"" was not found.");
        }

        [Fact]
        public async Task Gets_All_PlayerStatistic()
        {
            var stats = new[]
            {
                new PlayerStatistic
                {
                    CharacterId = "Id",
                    CharacterName = "Testosteron"
                },
                new PlayerStatistic
                {
                    CharacterId = "Id1",
                    CharacterName = "Testosteron1"
                }
            };
            TestDatabaseManager.SeedDatabase(_factory, stats);
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/playerstatistics");

            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<PlayerStatistic[]>(await response.Content.ReadAsStringAsync());
            responseObject.Should().BeEquivalentTo(stats, option
                => option.Excluding(statistic => statistic.Id));
        }

        [Fact]
        public async Task Creates_PlayerStatistic()
        {
            var client = _factory.CreateClient();
            var request = new AddPlayerStatisticCommand
            {
                CharacterId = "Id",
                CharacterName = "Name",
                DamageDealt = 1,
                DamageTaken = 2
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(request));
            requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync("/api/playerstatistics", requestBody);
            var getResponse = await client.GetAsync("/api/playerstatistics/Id");

            response.EnsureSuccessStatusCode();
            var getResponseObject = JsonConvert.DeserializeObject<GetPlayerStatisticRequest>(await getResponse.Content.ReadAsStringAsync());
            var createResponseObject = JsonConvert.DeserializeObject<GetPlayerStatisticRequest>(await response.Content.ReadAsStringAsync());
            getResponseObject.Should().BeEquivalentTo(request);
            createResponseObject.Should().BeEquivalentTo(request);
        }

        [Fact]
        public async Task Adds_to_Existing_Player_Statistic()
        {
            var client = _factory.CreateClient();
            var stat = new PlayerStatistic
            {
                CharacterId = "Id",
                CharacterName = "Testosteron",
                DamageDealt = 1,
                DamageTaken = 2
            };
            TestDatabaseManager.SeedDatabase(_factory, stat);
            var request = new AddPlayerStatisticCommand
            {
                CharacterId = "Id",
                CharacterName = "Name",
                DamageDealt = 1,
                DamageTaken = 2
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(request));
            requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync("/api/playerstatistics", requestBody);
            var getResponse = await client.GetAsync("/api/playerstatistics/Id");

            response.EnsureSuccessStatusCode();
            var getResponseObject = JsonConvert.DeserializeObject<GetPlayerStatisticRequest>(await getResponse.Content.ReadAsStringAsync());
            getResponseObject.Should().BeEquivalentTo(new GetPlayerStatisticRequest
            {
                CharacterId = "Id",
                CharacterName = "Testosteron",
                DamageDealt = 2,
                DamageTaken = 4
            });

        }

        [Fact]
        public async Task Deletes_PlayerStatistic()
        {
            var stats = new[]
            {
                new PlayerStatistic
                {
                    CharacterId = "Id",
                    CharacterName = "Testosteron"
                },
                new PlayerStatistic
                {
                    CharacterId = "Id1",
                    CharacterName = "Testosteron1"
                }
            };
            TestDatabaseManager.SeedDatabase(_factory, stats);
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync("/api/playerstatistics/Id");
            var getResponse = await client.GetAsync("/api/playerstatistics");

            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<PlayerStatistic[]>(await getResponse.Content.ReadAsStringAsync());
            responseObject.Should().HaveCount(1);
            responseObject.First().Should().BeEquivalentTo(stats[1], option
                => option.Excluding(statistic => statistic.Id));
        }
    }
}
