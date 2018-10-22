using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deltatre.Utils.Tests.Timers
{
  public partial class TimerAsyncTest
  {
    [Test]
    public async Task Calling_Stop_Should_Cancel_Scheduled_Action()
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
      timer.Start();
      await Task.Delay(1200);
      var count = list.Count;

      // ACT     
      await timer.Stop();

      // ASSERT
      Assert.LessOrEqual(list.Count, count + 1);
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
    public async Task Calling_Stop_More_Than_Once_Should_Not_Throw_Exception()
    {
      // ARRANGE
      var timer = new TimerAsync(_ => Task.FromResult(true), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
      timer.Start();

      // ASSERT
      await timer.Stop();
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
