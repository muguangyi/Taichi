# Taichi

Taichi Unity framework.

## TODO

* ~~Implement LiteIL to replace ILRuntime~~ (Use other solution)
* ~~Use ReorderableList to replace ListView~~
* Use C# to Lua solution to replace ILRuntime

## Develop using C# then export to Lua as asset

Introduce `CSharp.lua` to do C# to Lua code conversion since Lua VM performance is much better than ILRuntime.

Even ILRuntime has better communication between IL C# object with native C# object, ILRuntime executing IL byte code performance is too slow. So it's not suitable for implementing complex logic in IL byte code. In production, most project teams want to use scripting to implement most logic, and easy to switch between scripting code and native code, and no need to concern the performance.

For native layer, C# or C/C++ is the only choice; for scripting layer, Lua performance is the best. The only issue of Lua is that it's not strong type language (All scripting language feature), and the develop team has to handle both native programming language and scripting language.

Using native language for development stage, but exporting the hot upgrade part to scripting language is another solution.