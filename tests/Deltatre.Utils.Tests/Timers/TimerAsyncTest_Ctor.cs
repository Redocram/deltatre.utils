using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deltatre.Utils.Timers;
using NUnit.Framework;

namespace Deltatre.Utils.Tests.Timers
{
  [TestFixture]
  public partial class TimerAsyncTest
  {
    [Test]
    public void Ctor_Throws_When_ScheduledAction_IsNull()
    {
      // ACT
      Assert.Throws<ArgumentNullException>(
        () => new TimerAsync(
          null,
          TimeSpan.FromSeconds(1),
          TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void Ctor_Throws_When_DueTime_Is_Less_Than_Zero()
    {
      // ACT
      Assert.Throws<ArgumentOutOfRangeException>(
        () => new TimerAsync(
          _ => Task.CompletedTask,
          TimeSpan.FromMilliseconds(-6),
          TimeSpan.FromSeconds(10)));
    }

    [Test]
    public void Ctor_Throws_When_Period_Is_Less_Than_Zero()
    {
      // ACT
      Assert.Throws<ArgumentOutOfRangeException>(
        () => new TimerAsync(
          _ => Task.CompletedTask,
          TimeSpan.FromSeconds(10),
          TimeSpan.FromMilliseconds(-3)));
    }

    [Test]
    public void Ctor_Allows_To_Pass_DueTime_Zero()
    {
      // ACT
      Assert.DoesNotThrow(
        () => new TimerAsync(
          _ => Task.CompletedTask,
          TimeSpan.Zero,
          TimeSpan.FromSeconds(10)));
    }

    [Test]
    public void Ctor_Allows_To_Pass_Period_Zero()
    {
      // ACT
      Assert.DoesNotThrow(
        () => new TimerAsync(
          _ => Task.CompletedTask,
          TimeSpan.FromSeconds(5),
          TimeSpan.Zero));
    }

    [Test]
    public void Ctor_Allows_To_Pass_Infinite_DueTime()
    {
      Assert.DoesNotThrow(
        () => new TimerAsync(
          _ => Task.CompletedTask,
          Timeout.InfiniteTimeSpan,
          TimeSpan.FromSeconds(10)));
    }

    [Test]
    public void Ctor_Allows_To_Pass_Infinite_Period()
    {
      Assert.DoesNotThrow(
        () => new TimerAsync(
          _ => Task.CompletedTask,
          TimeSpan.FromSeconds(5),
          Timeout.InfiniteTimeSpan));
    }
  }
}

