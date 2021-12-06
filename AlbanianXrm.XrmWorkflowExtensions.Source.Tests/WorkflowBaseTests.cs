using AlbanianXrm.XrmExtensions.WorkflowsForTests;
using FakeXrmEasy.CodeActivities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
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
            _context.ExecuteCodeActivity<TraceMessageAction>(new Dictionary<string, object>());

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
            _context.CallerProperties.CallerId = user.ToEntityReference();

            // Act
            var result = _context.ExecuteCodeActivity<UserTimezoneAction>(new Dictionary<string, object>());

            // Assert
            Assert.Equal("Central Europe Standard Time", result[nameof(UserTimezoneAction.UserTimeZone)]);
        }
    }
}
