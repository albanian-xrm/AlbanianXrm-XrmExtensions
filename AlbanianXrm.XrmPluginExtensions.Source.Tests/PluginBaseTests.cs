using AlbanianXrm.XrmExtensions.PluginsForTests;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace AlbanianXrm.XrmExtensions
{
    public class PluginBaseTests : FakeXrmEasyTestsBase
    {
        [Fact]
        public void Should_Write_Trace()
        {
            // Arrange

            // Act
            _context.ExecutePluginWith<TraceMessagePlugin>();

            // Assert
            Assert.Contains("Executed", _context.GetTracingService().DumpTrace());
        }

        [Fact]
        public void Should_Use_User_TimeZone()
        {
            // Arrange
            var user = new Entity("systemuser")
            {
                Id = Guid.NewGuid()
            };
            var timezonedefinition = new Entity("timezonedefinition")
            {
                Id = Guid.NewGuid(),
                ["timezonecode"] = 1,
                ["standardname"] = "Central Europe Standard Time"
            };
            var usersettings = new Entity("usersettings")
            {
                Id = Guid.NewGuid(),
                ["timezonecode"] = 1,
                ["systemuserid"] = user.ToEntityReference()
            };
            _context.Initialize(new Entity[]
            {
                user,
                timezonedefinition,
                usersettings
            });
            var pluginContext = _context.GetDefaultPluginContext();
            pluginContext.InitiatingUserId = user.Id;
            pluginContext.UserId = user.Id;

            // Act
            _context.ExecutePluginWith<UserTimezonePlugin>(pluginContext);

            // Assert
            Assert.Contains(Constants.VAR_TIMEZONES + user.Id, pluginContext.SharedVariables.Keys);
        }
    }
}
