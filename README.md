# Scientifc Calculator

### Crossplatform calculator program, created in educational purpose in 2024 by [v3ssel](https://github.com/v3ssel).
### Using .NET 7.0, AvaloniaUI 11, EFCore, SQLite on the frontend and C++ library for calculation.

## How to build
Clone repository with submodule.
```
git clone https://github.com/v3ssel/scientific-calculator
git submodule init
git submodule update
```

To build calculation module
```
cd CalculationLib
make libs
cp *.<shared_lib_extension> ../Libs/
```

To run UI
```
dotnet run
```
