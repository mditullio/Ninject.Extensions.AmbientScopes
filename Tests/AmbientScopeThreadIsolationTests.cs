namespace Ninject.Extensions.AmbientScopes.Tests
{

    public class AmbientScopeThreadIsolationTests
    {

        public AmbientScopeManager ScopeManager { get; } = new AmbientScopeManager();

        [Fact]
        public void NewThreadsInheritCurrentScope()
        {
            using (AmbientScope parentScope = ScopeManager.BeginScope())
            {
                AmbientScope currentScopeInThread1 = null;
                AmbientScope currentScopeInThread2 = null;

                // Create a new thread
                Thread newThread1 = new Thread(() =>
                {
                    // Capture current scope
                    currentScopeInThread1 = ScopeManager.Current;
                });

                newThread1.Start();
                newThread1.Join();

                // Create a new thread
                Thread newThread2 = new Thread(() =>
                {
                    // Capture current scope
                    currentScopeInThread2 = ScopeManager.Current;
                });

                newThread2.Start();
                newThread2.Join();

                // Ensure scopes are same
                Assert.Equal(ScopeManager.Current, parentScope);
                Assert.Equal(ScopeManager.Current, currentScopeInThread1);
                Assert.Equal(ScopeManager.Current, currentScopeInThread2);
            }

            // Ensure the parent scope is now null
            Assert.Null(ScopeManager.Current);
        }

        [Fact]
        public void NewThreadsCreateNestedScopeInIsolation()
        {
            using (var parentScope = ScopeManager.BeginScope())
            {
                AmbientScope currentScopeInThread1 = null;
                AmbientScope currentScopeInThread2 = null;

                // Create a new threads
                Thread newThread1 = new Thread(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = ScopeManager.BeginScope();
                    currentScopeInThread1 = ScopeManager.Current;
                });

                newThread1.Start();
                newThread1.Join();

                Thread newThread2 = new Thread(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = ScopeManager.BeginScope();
                    currentScopeInThread2 = ScopeManager.Current;
                });

                newThread2.Start();
                newThread2.Join();

                // Ensure current scope is still the parent scope
                Assert.NotEqual(ScopeManager.Current, currentScopeInThread1);
                Assert.NotEqual(ScopeManager.Current, currentScopeInThread2);
                Assert.Equal(ScopeManager.Current, parentScope);
                Assert.False(currentScopeInThread1.IsDisposed);
                Assert.False(currentScopeInThread2.IsDisposed);
            }

            // Ensure the parent scope is now null
            Assert.Null(ScopeManager.Current);
        }

        [Fact]
        public async Task NewTasksInheritCurrentScope()
        {
            using (AmbientScope parentScope = ScopeManager.BeginScope())
            {
                AmbientScope currentScopeInTask1 = null;
                AmbientScope currentScopeInTask2 = null;

                // Create a child task
                await Task.Run(() =>
                {
                    // Capture current scope
                    currentScopeInTask1 = ScopeManager.Current;
                });

                // Create a child task
                await Task.Run(() =>
                {
                    // Capture current scope
                    currentScopeInTask2 = ScopeManager.Current;
                });

                // Ensure scopes are same
                Assert.Equal(ScopeManager.Current, parentScope);
                Assert.Equal(ScopeManager.Current, currentScopeInTask1);
                Assert.Equal(ScopeManager.Current, currentScopeInTask2);
            }

            // Ensure the parent scope is now null
            Assert.Null(ScopeManager.Current);
        }

        [Fact]
        public async Task NewTasksCreateNestedScopesInIsolation()
        {
            using (var parentScope = ScopeManager.BeginScope())
            {
                AmbientScope currentScopeInTask1 = null;
                AmbientScope currentScopeInTask2 = null;

                // Create a child task
                await Task.Run(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = ScopeManager.BeginScope();
                    currentScopeInTask1 = ScopeManager.Current;
                });

                // Create a child task
                await Task.Run(() =>
                {
                    // Create nested scope without disposing it
                    var nestedScope = ScopeManager.BeginScope();
                    currentScopeInTask2 = ScopeManager.Current;
                });

                // Ensure parent scope is still the original scope
                Assert.NotEqual(ScopeManager.Current, currentScopeInTask1);
                Assert.NotEqual(ScopeManager.Current, currentScopeInTask2);
                Assert.Equal(ScopeManager.Current, parentScope);
                Assert.False(currentScopeInTask1.IsDisposed);
                Assert.False(currentScopeInTask2.IsDisposed);
            }

            // Ensure the parent scope is now null
            Assert.Null(ScopeManager.Current);
        }
    }

}
