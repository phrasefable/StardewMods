# Usage Notes

* Add the following to your (testing, not under test) mod's `.csproj`
  file: `<ProjectReference Include="..\StarUnit\StarUnit.csproj" Private="false"/>` (`Private="false"`) prevents the
  StarUnit dll being copied to your mod's output directory

* Add `Phrasefable.StarUnit` to the list of dependencies in your testing mod's `manifest.json`

## Test Result Statuses

* `Passed`
* `Failed`: Explicit failure - should only be set in test methods.
* `Error`: Try as much as possible to avoid having tests return this - use conditions and before/after each/all
* `Skipped` - Only used if a test/suite is not attempted due to a problem with a parent.

## Todo

1. Split into own solution, with projects: (1) SMAPI Mod / SMAPI adaptor; and (2) Framework.
    - Thus consumers should reference and could bundle the framework assembly, and may fake the mod's API.
    - _? possibly also split engine implementation from smapi adaptor ?_
2. Write unit tests of engine implementation.
3. Write in-game-tests of the smapi adaptor.
3. Make a demo/skeleton project.

## Issues

### 1. Delayed Conditions
Conditions on a node don't evaluate until that node's delay is elapsed, but delay does not pass unless Player1 is free. This means that if a node with conditions (such as WorldReady) runs in the main menu, any delay will suspend it until the world is loaded.
If a node has the condition WorldReady, it shouldn't be delayed itself, but instead have a delay before all its children. (i.e. don't set `ITraversableBuilder.Delay`, use`ITestFixtureBuilder.BeforeAllDelay` instead)
   