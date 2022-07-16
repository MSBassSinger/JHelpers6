JHelpers version 6
========

JHelpers is a .NET Standard 2.0 library component that can be used with any .NET
project, whether Core or Standard, on any OS that works with .NET Standard or .NET Core. It
is a library of generally useful methods and functionality that can save a
developer from writing code to accomplish these mundane tasks – or worse, have
several versions of a method that does the same thing written in multiple
places. JHelpers is used in my JDAC and JLogger NuGet packages for that very
reason.  JHelpers is a signed assembly.

Code examples are available in the sample application found at:
https://github.com/MSBassSinger/JHelpers_Demo/

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

