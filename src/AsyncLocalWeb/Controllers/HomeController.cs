using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private static int Counter = 0;
        private AsyncLocal<string> _implicitContext = new AsyncLocal<string>();
        private ThreadLocal<string> _threadContext = new ThreadLocal<string>();
        private static ThreadLocal<string> _anotherThreadContext = new ThreadLocal<string>();
        public IActionResult Index()
        {
            return Content($"<p><a href={Url.Action(nameof(AsyncLocalWithHttpRequest), "Home")}>AsyncLocalWithHttpRequest</a></p>" +
            $"<p><a href={Url.Action(nameof(AsyncLocalWithTaskRun), "Home")}>AsyncLocalWithTaskRun</a></p>", "text/html");
        }

        public async Task<IActionResult> AsyncLocalWithHttpRequest()
        {
            var builder = new StringBuilder();
            var value = "AsyncLocalWithHttpRequest " + Interlocked.Increment(ref Counter);
            builder.AppendLine("Set AsyncLocal and ThreadLocal with " + value);
            _implicitContext.Value = value;
            _threadContext.Value = value;

            builder.AppendLine("Thread id is: " + Thread.CurrentThread.ManagedThreadId);
            builder.AppendLine("AsyncLocal value before async operation is: " + _implicitContext.Value);
            builder.AppendLine("ThreadLocal value before async operation is: " + _threadContext.Value);

            var httpClient = new HttpClient();

            await httpClient.GetAsync("https://codingmilitia.com").ConfigureAwait(false);

            builder.AppendLine("Thread id is: " + Thread.CurrentThread.ManagedThreadId);
            builder.AppendLine("AsyncLocal value after async operation is: " + _implicitContext.Value);
            builder.AppendLine("ThreadLocal value after async operation is: " + _threadContext.Value);
            return Content(builder.ToString());
        }

        public async Task<IActionResult> AsyncLocalWithTaskRun()
        {
            var builder = new StringBuilder();
            var value = "AsyncLocalWithTaskRun " + Interlocked.Increment(ref Counter);
            builder.AppendLine("Set AsyncLocal and ThreadLocal with " + value);
            _implicitContext.Value = value;
            _threadContext.Value = value;

            builder.AppendLine("Thread id is: " + Thread.CurrentThread.ManagedThreadId);
            builder.AppendLine("AsyncLocal value before async operation is: " + _implicitContext.Value);
            builder.AppendLine("ThreadLocal value before async operation is: " + _threadContext.Value);

            var httpClient = new HttpClient();

            await Task.Run(() =>
               {
                   _anotherThreadContext.Value = value;
                   builder.AppendLine("Thread id is: " + Thread.CurrentThread.ManagedThreadId);
                   builder.AppendLine("AsyncLocal value inside async operation is: " + _implicitContext.Value);
                   builder.AppendLine("ThreadLocal value inside async operation is: " + _threadContext.Value);
                   builder.AppendLine("Another ThreadLocal value inside async operation is: " + _anotherThreadContext.Value);
               }).ConfigureAwait(false);

            builder.AppendLine("Thread id is: " + Thread.CurrentThread.ManagedThreadId);
            builder.AppendLine("AsyncLocal value after async operation is: " + _implicitContext.Value);
            builder.AppendLine("ThreadLocal value after async operation is: " + _threadContext.Value);
            builder.AppendLine("Another ThreadLocal value after async operation is: " + _anotherThreadContext.Value);
            return Content(builder.ToString());
        }
    }
}
