// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel.Tests
{
    using ApiApprover;
    using NUnit.Framework;
    using Views;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_Squirrel_HasNoBreakingChanges()
        {
            var assembly = typeof(UpdateService).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }

        [Test]
        public void Orc_Squirrel_Xaml_HasNoBreakingChanges()
        {
            var assembly = typeof(AppInstalledWindow).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}