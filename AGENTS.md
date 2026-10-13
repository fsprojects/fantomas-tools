# fantomas-tools

Before calling a change done, run the check stages of the `Build` pipeline in `build.fsx` (format, client lint, client tests) and `dotnet fsi build.fsx -- -p Analyze`. Analyze exits 0 on warnings, which still land on the PR, so read `analysis.sarif`: it must have no results.
