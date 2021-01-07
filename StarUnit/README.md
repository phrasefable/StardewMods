# Usage Notes

* Add the following to your (testing, not under test) mod's `.csproj`
  file: `<ProjectReference Include="..\StarUnit\StarUnit.csproj" Private="false"/>` (`Private="false"`) prevents the
  StarUnit dll being copied to your mod's output directory

* Add `Phrasefable.StarUnit` to the list of dependencies in your testing mod's `manifest.json`

## Test Result Statuses
* Passed
* Failed: Explicit failure - should only be set in test methods.
* Error: Try as much as possible to avoid having tests return this - use conditions and before/after each/all
* Skipped - Only used if a test/suite is not attempted due to a problem with a parent.
