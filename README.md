# Ninject.Extensions.AmbientScopes

This library is an extension to Ninject, providing a way to define ambient scopes. 
An ambient scope is a logical container for objects that are shared during the lifetime of an ambient transaction. 
The ambient scope provides deterministic disposal of objects, ensuring proper cleanup and resource management.
This library makes it easier to manage scoped dependencies in your application when using Ninject for dependency injection.

## Table of Contents

1. [Installation](#installation)
2. [Usage](#usage)
    - [Basic Usage](#basic-usage)
    - [Nested Scopes](#nested-scopes)
    - [Threading and Task Considerations](#threading-and-task-considerations)
3. [Running the Tests](#running-the-tests)
4. [Contributing](#contributing)

## Installation

To install the package, you can reference it directly from your project.

## Usage

### Basic Usage

1. **Initialize Kernel with Ambient Scopes**
   
    ```csharp
    var kernel = new StandardKernel();
    kernel.UseAmbientScopes();
    ```

2. **Bind Services with Ambient Scope**
   
    ```csharp
    kernel.Bind<MyService>().ToSelf().InAmbientScope();
    ```

3. **Begin a New Ambient Scope**

    ```csharp
    using (kernel.BeginAmbientScope())
    {
        // Your code here
        var myService = kernel.Get<MyService>();
        // ...
    }
    // myService will be disposed here.
    ```

### Nested Scopes

You can nest ambient scopes, each new scope will be isolated from the others.

```csharp
using (var outerScope = kernel.BeginAmbientScope())
{
    // Kernel will resolve ambient objects using outerScope

    // Nested scope
    using (var innerScope = kernel.BeginAmbientScope())
    {
        // Kernel will resolve ambient objects using innerScope
    }
}
```

### Threading and Task Considerations

1. **Scope Isolation Across Tasks and Threads**

    Ambient scopes created in different tasks (or threads) are isolated. Each task or thread will have its own scope, even if they are part of the same parent scope.

    ```csharp
    using (kernel.BeginAmbientScope())
    {
        // Task 1
        await Task.Run(() => 
        {
            var nestedScope1 = kernel.BeginAmbientScope();
            // This will be isolated from the parent and sibling scopes
        });

        // Task 2
        await Task.Run(() => 
        {
            var nestedScope2 = kernel.BeginAmbientScope();
            // This will also be isolated from the parent and sibling scopes
        });
    }
    ```

2. **Scope Inheritance from Parent Task to Child Tasks**

    When you start a new child task using `Task.Run()` or create a new thread using `Thread.Start()`, the child task or thread will inherit the ambient scope from its parent.
    
    ```csharp
    using (kernel.BeginAmbientScope())
    {
        // The following child tasks will inherit the current ambient scope
        await Task.Run(() => 
        {
            // This task will have access to the ambient scope defined above
        });
    
        await Task.Run(() => 
        {
            // This task will also have access to the ambient scope defined above
        });
    }
    ```


## Running the Tests

This library comes with a comprehensive set of unit tests. To run them, either use Visual Studio Test Explorer or navigate to the root directory and execute the test runner:

```bash
dotnet test
```

## Contributing

Feel free to review the code, make pull requests, or suggestions.
Thanks for reading !
