1. **Optimize `magnitude` checks to `sqrMagnitude` in `PlayerMovementController`**
   - The `_movementInput.magnitude > 0.01f` checks (and `<=` equivalent) perform an unnecessary square root operation per frame in `FixedUpdate` and `Update` loops.
   - I will use `replace_with_git_merge_diff` to replace `_movementInput.magnitude > 0.01f` with `_movementInput.sqrMagnitude > 0.0001f` (and the `<=` equivalent).
   - I will update this in `FixedUpdate`, `UpdateAnimator`, and `ComputeMovementDirection` methods of `Assets/SystemsExtensions/AbilitySystemExtension/Scripts/PlayerMovementController.cs`.
2. **Optimize `DashAbility.cs` as well**
   - I will use `replace_with_git_merge_diff` to replace `_playerMovementController.MovementDirection.magnitude > 0.01f` with `_playerMovementController.MovementDirection.sqrMagnitude > 0.0001f` in `Assets/SystemsExtensions/AbilitySystemExtension/Runtime/Abilities/DashAbility.cs`.
3. **Run tests**
   - Execute the test suite using `run_in_bash_session` with `pwsh run_tests.ps1` to ensure no functionality is broken.
4. **Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.**
   - Run `pre_commit_instructions` and follow its directives.
5. **Submit the Pull Request**
   - Submit the changes using the `submit` tool.
   - Title: "⚡ Bolt: Replace magnitude with sqrMagnitude in movement controllers"
   - Include description mapping to Bolt's requirements.
