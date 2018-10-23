using Deltatre.Utils.Timers;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deltatre.Utils.Tests.Timers
{
  public partial class TimerAsyncTest
  {
    [Test]
    public async Task Stop_Terminates_Execution_Of_Scheduled_Background_Workload()
    {
      // ARRANGE
      var values = new ConcurrentBag<int>();
      Func<CancellationToken, Task> action = ct =>
      {
        ct.ThrowIfCancellationRequested();
        values.Add(1);
        return Task.CompletedTask;
      };

      var target = new TimerAsync(
        action,
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromMilliseconds(500));

      // ACT
      target.Start();

      await Task.Delay(1200).ConfigureAwait(false); // in 1200 milliseconds we are sure that action is called at least twice

      await target.Stop().ConfigureAwait(false);
      var snapshot1 = values.ToArray();

      await Task.Delay(1200).ConfigureAwait(false); // in 1200 milliseconds we are sure that action is called at least twice (but now the timer is stopped)
      var snapshot2 = values.ToArray();

      // ASSERT
      Assert.GreaterOrEqual(snapshot1.Length, 2);
      Assert.IsTrue(snapshot1.All(i => i == 1));
      CollectionAssert.AreEqual(snapshot1, snapshot2);
    }

    [Test]
    public void Stop_Can_Be_Called_More_Than_Once_Without_Throwing_Exceptions()
    {
      // ARRANGE
      var target = new TimerAsync(
        _ => Task.CompletedTask,
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromMilliseconds(500));

      // ASSERT
      Assert.DoesNotThrowAsync(async () =>
      {
        target.Start();

        await Task.Delay(1200).ConfigureAwait(false);

        await target.Stop().ConfigureAwait(false);
        await target.Stop().ConfigureAwait(false);
      });
    }

    [Test]
    public async Task Stop_Can_Be_Called_More_Than_Once_Without_Changing_Expected_Behaviour()
    {
      // ARRANGE
      var values = new ConcurrentBag<int>();
      Func<CancellationToken, Task> action = ct =>
      {
        ct.ThrowIfCancellationRequested();
        values.Add(1);
        return Task.CompletedTask;
      };

      var target = new TimerAsync(
        action,
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromMilliseconds(500));

      // ACT
      target.Start();

      await Task.Delay(1200).ConfigureAwait(false); // in 1200 milliseconds we are sure that action is called at least twice

      await target.Stop().ConfigureAwait(false);
      await target.Stop().ConfigureAwait(false);
      var snapshot1 = values.ToArray();

      await Task.Delay(1200).ConfigureAwait(false); // in 1200 milliseconds we are sure that action is called at least twice (but now the timer is stopped)
      await target.Stop().ConfigureAwait(false);
      var snapshot2 = values.ToArray();

      // ASSERT
      Assert.GreaterOrEqual(snapshot1.Length, 2);
      Assert.IsTrue(snapshot1.All(i => i == 1));
      CollectionAssert.AreEqual(snapshot1, snapshot2);
    }















    [Test]
    public void Calling_Stop_Before_Start_Should_Not_Throw_Exception()
    {
      // ARRANGE
      var timer = new TimerAsync(_ => Task.FromResult(true), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));

      // ASSERT
      Assert.DoesNotThrowAsync(async () => await timer.Stop());
    }

    

    [Test]
    public async Task Calling_Stop_Before_Start_Should_Not_Change_Running_Behaviour()
    {
      // ARRANGE
      var list = new List<int>();
      Func<CancellationToken, Task> action = ct =>
      {
        ct.ThrowIfCancellationRequested();
        list.Add(1);
        return Task.FromResult(true);
      };
      var timer = new TimerAsync(action, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
      await timer.Stop();

      // ACT
      timer.Start();

      // ASSERT
      await Task.Delay(1200);
      Assert.GreaterOrEqual(2, list.Count);
      Assert.IsTrue(list.All(i => i == 1));
    }

    [Test]
    public void Calling_Start_After_Dispose_Throws_ObjectDisposedException()
    {
      // ARRANGE
      var timer = new TimerAsync(_ => Task.FromResult(true), TimeSpan.Zero, TimeSpan.Zero);

      // ACT & ASSERT
      timer.Dispose();
      Assert.Throws<ObjectDisposedException>(timer.Start);
    }

    [Test]
    public void Calling_Stop_After_Dispose_Throws_ObjectDisposedException()
    {
      // ARRANGE
      var timer = new TimerAsync(_ => Task.FromResult(true), TimeSpan.Zero, TimeSpan.Zero);

      // ACT & ASSERT
      timer.Dispose();
      Assert.ThrowsAsync<ObjectDisposedException>(timer.Stop);
    }
  }
}
