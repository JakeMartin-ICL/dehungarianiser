# Dehungarianiser for Rider and ReSharper

[![Rider](https://img.shields.io/jetbrains/plugin/v/21601-dehungarianiser.svg?label=Rider&colorB=0A7BBB&style=for-the-badge&logo=rider)](https://plugins.jetbrains.com/plugin/21601-dehungarianiser)
[![ReSharper](https://img.shields.io/jetbrains/plugin/v/21601-dehungarianiser.svg?label=ReSharper&colorB=0A7BBB&style=for-the-badge&logo=resharper)](https://plugins.jetbrains.com/plugin/21601-dehungarianiser)

This plugin highlights declarations of variables using Hungarian notation and offers a quick fix to remove the Hungarian notation and executes the rename refactoring. This can be applied to a single variable or to the entire file or project.

## What is Hungarian notation?

Hungarian notation is the practice of preceding variable names with hints about the type of the variable. For example, instead of `var people = new List<Person>`, the variable would be named `plstPeople`. The `p` indicates it's a local variable (or parameter), while the `lst` part indicates it's a list.

### Notation standard

This plugin currently detects only one Hungarian notation standard - `p` for parameter/local or `m` for field, followed by a type shorthand such as `bln`, `lst`, `dbl`, etc.

This is subject to change, possibly with support for custom standards in future.

## Why remove Hungarian notation?

Hungarian notation is widely regarded as a bad idea. Though it originally served a purpose in times before strongly typed languages and IDEs, it has no place in modern codebases.

- It's confusing - It makes code harder to read/understand, especially for developers who don't use it, such as developers new to a codebase
- It's redundant - Variable name, context and IDEs all give the type in better ways anyway
- It's inconsistent - People frequently do it differently and/or incorrectly
- It's limiting - Changing the type of a variable also requires a name change or leaves the name incorrect
- It's verbose - It makes variable names unpronounceable and makes the code overall longer, meaning more to read
- It's unnecessary - C# is a typed language anyway, even without all the type info you can't really do something illegal accidentally
- It can be misleading - If the type has changed or the name is wrong, developers can be misled by the name
- It's distracting - Similar to points 1 and 5, however it's important to stress that it really does distract from the important code