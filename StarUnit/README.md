# Usage Notes

* Add the following to your (testing, not under test) mod's `.csproj`
  file: `<ProjectReference Include="..\StarUnit\StarUnit.csproj" Private="false"/>` (`Private="false"`) prevents the
  StarUnit dll being copied to your mod's output directory

* Add `Phrasefable.StarUnit` to the list of dependencies in your testing mod's `manifest.json`

