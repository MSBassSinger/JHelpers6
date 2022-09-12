JHelpers version 6
========

Version 6.0.1 Update - changed out the ContextHelper Dictionary<> for ConcurrentDictionary<>.

JHelpers6 is a .NET 6 library component that can be used with any .NET
project that uses .NET 6 on any OS that works with .NET 6. It
is a library of generally useful methods and functionality that can save a
developer from writing code to accomplish these mundane tasks – or worse, have
several versions of a method that does the same thing written in multiple
places. JHelpers6 is used in my JDAC6 and JLogger6 NuGet packages for that very
reason.  JHelpers6 is NOT signed as the code is open source.
Code for JHelpers6 is found at:
https://github.com/MSBassSinger/JHelpers6

Code examples are available in the sample application found at:
https://github.com/MSBassSinger/JHelpers_Demo6

NuGet package is found at:
https://www.nuget.org/packages/Jeff.Jones.JHelpers6
or by searching with NuGet package manager for "Jeff.Jones.JHelpers6"

ContextMgr
----------

ContextMgr is a thread-safe singleton that can be used to store data in a single
place accessible from anywhere, and any thread, in the application. One typical
use is to store settings and other runtime values so their sources (file,
database, etc.) only have to be read once. Anything that can be defined as a
unique String name and a value or reference type of any kind can be kept there.
Values are added as a String key name (which must be unique) and any value or
reference type, boxed as a dynamic type.

ContextMgr has no initialization, and as a singleton, does not use “new” to be
created. The actual instance is dynamically created on the first reference in
the code.

In your application’s shutdown code, I recommend adding this line so the
ContextMgr disposes of its resources without waiting on the .NET garbage
collector.

ContextMgr.Instance.Dispose();

CommonHelpers
-------------

This is a static class with a number of useful static methods and extension methods.
The JHelpers_Demo6 demo project shows the usage.

